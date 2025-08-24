using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Render Graph
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class LumaClampFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader Shader;
        public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;
        [Range(0.05f, 4f)] public float MaxLuminance = 1.0f;
        [Range(0f, 2f)] public float Softness = 0.25f;
        [Range(0f, 2f)] public float DebugMul = 0.9f; // set 0.9 to see a slight darken; set 1.0 later
    }

    class Pass : ScriptableRenderPass
    {
        static readonly int _MaxLuma = Shader.PropertyToID("_MaxLuma");
        static readonly int _Softness = Shader.PropertyToID("_Softness");
        static readonly int _DebugMul = Shader.PropertyToID("_DebugMul");

        readonly Material _mat;

        // compatibility path
        RTHandle _tmp;

        public float maxLuma, softness, debugMul;

        public Pass(Material m, RenderPassEvent evt)
        {
            _mat = m;
            renderPassEvent = evt;
        }

        // --------- Legacy path (if Compatibility Mode is ON) ---------
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData data)
        {
            ConfigureInput(ScriptableRenderPassInput.Color);
            var desc = data.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0; desc.msaaSamples = 1;
            RenderingUtils.ReAllocateIfNeeded(ref _tmp, desc, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_LumaClampTmp");
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData data)
        {
            if (_mat == null) return;
            var cmd = CommandBufferPool.Get("LumaClamp.Execute");
            _mat.SetFloat(_MaxLuma, maxLuma);
            _mat.SetFloat(_Softness, softness);
            _mat.SetFloat(_DebugMul, debugMul);

            var src = data.cameraData.renderer.cameraColorTargetHandle;
            Blitter.BlitCameraTexture(cmd, src, _tmp, _mat, 0);
            Blitter.BlitCameraTexture(cmd, _tmp, src);

            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) { }

        public void Release() { _tmp?.Release(); _tmp = null; }

        // --------- Render Graph path (Unity 6 / URP 17) ---------
        public override void RecordRenderGraph(RenderGraph rg, ContextContainer frame)
        {
            if (_mat == null) return;

            var res = frame.Get<UniversalResourceData>();
            var cam = frame.Get<UniversalCameraData>();

            // Pick a valid source. If the "active" isn’t ready yet or is backbuffer, fall back to cameraColor.
            var src = (res.activeColorTexture.IsValid() && !res.isActiveTargetBackBuffer)
                        ? res.activeColorTexture
                        : res.cameraColor;

            // We’ll write the result back into the same handle we read from.
            var dst = src;

            // Temp RT
            var desc = cam.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            desc.msaaSamples = 1;
            var tmp = UniversalRenderer.CreateRenderGraphTexture(rg, desc, "_LumaClampTmp", false);

            // Material params
            _mat.SetFloat(_MaxLuma, maxLuma);
            _mat.SetFloat(_Softness, softness);
            _mat.SetFloat(_DebugMul, debugMul); // set to 0.5 once to prove it runs

            // src -> tmp (apply effect)
            rg.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(src, tmp, _mat, 0),
                           "LumaClamp: Apply");

            // tmp -> dst (write BACK to the same handle)
            rg.AddBlitPass(new RenderGraphUtils.BlitMaterialParameters(tmp, dst, _mat, 0),
                           "LumaClamp: Composite");
        }


    }

    public Settings settings = new Settings();
    Material _mat;
    Pass _pass;

    public override void Create()
    {
        if (!settings.Shader)
            settings.Shader = Shader.Find("Hidden/LumaClamp");

        if (settings.Shader)
            _mat = new Material(settings.Shader);

        _pass = new Pass(_mat, settings.Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData data)
    {
        if (_pass == null || _mat == null) return;
        _pass.maxLuma = settings.MaxLuminance;
        _pass.softness = settings.Softness;
        _pass.debugMul = settings.DebugMul;
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pass?.Release();
            if (_mat)
            {
                if (Application.isPlaying) Destroy(_mat);
                else DestroyImmediate(_mat);
            }
        }
    }
}
