using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Profiling;
using UnityEngine.Rendering.RendererUtils;

public class DepthNormalsFeature : ScriptableRendererFeature
{
    private class DepthNormalsPass : ScriptableRenderPass
    {
        private int kDepthBufferBits = 32;
        private RTHandle depthAttachmentHandle;
        private int tempRTId;

        internal RenderTextureDescriptor descriptor { get; private set; }

        private Material depthNormalsMaterial = null;
        private FilteringSettings m_FilteringSettings;
        private string m_ProfilerTag = "DepthNormals Prepass";
        private ShaderTagId m_ShaderTagId = new ShaderTagId("DepthOnly");

        public DepthNormalsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material)
        {
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            depthNormalsMaterial = material;
        }

        public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle depthAttachmentHandle)
        {
            this.depthAttachmentHandle = depthAttachmentHandle;
            baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
            baseDescriptor.depthBufferBits = kDepthBufferBits;
            descriptor = baseDescriptor;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            tempRTId = Shader.PropertyToID(depthAttachmentHandle.name);
            cmd.GetTemporaryRT(tempRTId, descriptor, FilterMode.Point);
            ConfigureTarget(depthAttachmentHandle);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagId, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref renderingData.cameraData;
                Camera camera = cameraData.camera;
                if (cameraData.xr.enabled)
                    context.StartMultiEye(camera);

                drawSettings.overrideMaterial = depthNormalsMaterial;

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);

                cmd.SetGlobalTexture("_CameraDepthNormalsTexture", tempRTId);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (depthAttachmentHandle != null)
            {
                cmd.ReleaseTemporaryRT(tempRTId);
                depthAttachmentHandle = null;
            }
        }
    }

    private DepthNormalsPass depthNormalsPass;
    private RTHandle depthNormalsTexture; // Changed to RTHandle
    private Material depthNormalsMaterial;

    public override void Create()
    {
        depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        depthNormalsPass = new DepthNormalsPass(RenderQueueRange.opaque, -1, depthNormalsMaterial);
        depthNormalsPass.renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
        depthNormalsTexture = RTHandles.Alloc("_CameraDepthNormalsTexture");
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, depthNormalsTexture);
        renderer.EnqueuePass(depthNormalsPass);
    }
}