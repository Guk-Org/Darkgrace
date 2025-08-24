using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

public class CopyFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        public Shader Shader;
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingTransparents;
    }

    class CopyPass : ScriptableRenderPass
    {
        readonly Material _mat;

        public CopyPass(Material mat, RenderPassEvent evt)
        {
            _mat = mat;
            renderPassEvent = evt;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_mat == null) return;

            var resources = frameData.Get<UniversalResourceData>();
            var camData = frameData.Get<UniversalCameraData>();

            if (resources.isActiveTargetBackBuffer)
                return;

            var src = resources.activeColorTexture;

            var desc = camData.cameraTargetDescriptor;
            desc.msaaSamples = 1;
            desc.depthBufferBits = 0;

            var tmp = UniversalRenderer.CreateRenderGraphTexture(
                renderGraph,
                desc,
                "_CopyTmp",
                false
            );

            // src -> tmp
            var p0 = new RenderGraphUtils.BlitMaterialParameters(src, tmp, _mat, 0);
            renderGraph.AddBlitPass(p0, "CopyFeature: BlitA");

            // tmp -> src
            var p1 = new RenderGraphUtils.BlitMaterialParameters(tmp, src, _mat, 0);
            renderGraph.AddBlitPass(p1, "CopyFeature: BlitB");
        }
    }

    public Settings settings = new Settings();
    Material _mat;
    CopyPass _pass;

    public override void Create()
    {
        if (!settings.Shader)
            settings.Shader = Shader.Find("Hidden/Copy");

        if (settings.Shader)
            _mat = new Material(settings.Shader);

        _pass = new CopyPass(_mat, settings.Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_pass != null)
            renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        if (_mat)
        {
            if (Application.isPlaying) Destroy(_mat);
            else DestroyImmediate(_mat);
        }
    }
}
