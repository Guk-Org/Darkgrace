Shader "Hidden/LumaClamp"
{
    Properties
    {
        _MaxLuma ("Max Luminance", Range(0.05, 4.0)) = 1.0
        _Softness("Softness (Knee)", Range(0.0, 2.0)) = 0.25
        _DebugMul("Debug Brightness Mult", Range(0.0, 2.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "LumaClamp"
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   Vert
            #pragma fragment Frag

            // 1) URP core (defines TransformObjectToHClip and friends)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 2) Fallback shims: define XR-safe macros if not present yet (prevents TEXTURE2D_X errors)
            #ifndef TEXTURE2D_X
                #define TEXTURE2D_X(tex)                TEXTURE2D(tex)
                #define SAMPLE_TEXTURE2D_X(tex,s,u)     SAMPLE_TEXTURE2D(tex,s,u)
            #endif
            #ifndef TransformTriangleVertexToUV
            float2 TransformTriangleVertexToUV(float2 uv, float4 scaleBias)
            {
                // scaleBias.xy = scale, .zw = bias (set by the blit utility)
                return uv * scaleBias.xy + scaleBias.zw;
            }
            #endif

            // 3) Blit helpers (provides _BlitTexture, sampler_LinearClamp, _BlitScaleBias on most URP versions)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // 4) Luminance helper
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            // Material params
            float _MaxLuma, _Softness, _DebugMul;

            struct VIn  { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct VOut { float4 positionHCS: SV_Position; float2 uv : TEXCOORD0; };

            VOut Vert (VIn v)
            {
                VOut o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TransformTriangleVertexToUV(v.uv, _BlitScaleBias);
                return o;
            }

            // Smooth highlight compression (soft knee)
            float SoftClamp(float y, float cap, float knee)
            {
                if (y <= cap) return y;
                float x = y - cap;
                float k = max(knee, 1e-6);
                float t = x / (x + k);
                return lerp(y, cap, t);
            }

            float3 ClampLumaPreserveChroma(float3 rgb, float cap, float knee)
            {
                float y  = Luminance(rgb);           // Rec709
                if (y <= 0.0) return rgb;
                float y2 = SoftClamp(y, cap, knee);
                return rgb * (y2 / y);
            }

            float4 Frag (VOut i) : SV_Target
            {
                float4 c = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.uv);
                c.rgb *= 0.5;            // <-- TEMP: force 50% darker to prove draw happens
                return c;

            }
            ENDHLSL
        }
    }
}
