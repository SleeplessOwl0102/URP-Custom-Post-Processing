using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SleeplessOwl.URPPostProcessing
{
    public class PostProcessRenderFeature : ScriptableRendererFeature
    {
        public PostProcessOrderConfig config;

        private CustomPostProcessRenderPass afterSkyboxPass;
        private CustomPostProcessRenderPass beforeNativePostProcessPass;
        private CustomPostProcessRenderPass afterNativePostProcessPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(afterSkyboxPass);
            renderer.EnqueuePass(beforeNativePostProcessPass);
            renderer.EnqueuePass(afterNativePostProcessPass);
        }

        public override void Create()
        {
            

#if UNITY_EDITOR
            config.OnDataChange = Create;
#endif
            afterSkyboxPass = new CustomPostProcessRenderPass(RenderPassEvent.AfterRenderingSkybox, config);
            beforeNativePostProcessPass = new CustomPostProcessRenderPass(RenderPassEvent.BeforeRenderingPostProcessing, config);
            afterNativePostProcessPass = new CustomPostProcessRenderPass(RenderPassEvent.AfterRenderingPostProcessing, config);
        }
    }
}