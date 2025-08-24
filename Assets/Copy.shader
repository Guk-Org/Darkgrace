Shader "Hidden/Copy"
{
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "Copy"
            HLSLPROGRAM
            #pragma target 3.0
            #pragma vertex   Vert
            #pragma fragment Frag

            // 1) URP core first (defines XR & texture macros in many versions)
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            // 2) Fallback shims in case your packages don't define these yet
            #ifndef TEXTURE2D_X
                #define TEXTURE2D_X(tex)                TEXTURE2D(tex)
                #define SAMPLE_TEXTURE2D_X(tex,s,u)     SAMPLE_TEXTURE2D(tex,s,u)
            #endif
            #ifndef TransformTriangleVertexToUV
            float2 TransformTriangleVertexToUV(float2 uv, float4 scaleBias)
            {
                // scaleBias.xy = scale, .zw = bias
                return uv * scaleBias.xy + scaleBias.zw;
            }
            #endif

            // 3) Blit utilities (binds _BlitTexture, sampler_LinearClamp, _BlitScaleBias)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            struct VIn  { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct VOut { float4 positionHCS: SV_Position; float2 uv : TEXCOORD0; };

            VOut Vert (VIn v)
            {
                VOut o;
                o.positionHCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = TransformTriangleVertexToUV(v.uv, _BlitScaleBias);
                return o;
            }

            float4 Frag (VOut i) : SV_Target
            {
                // _BlitTexture, sampler_LinearClamp provided by Blit.hlsl
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, i.uv);
            }
            ENDHLSL
        }
    }
}
