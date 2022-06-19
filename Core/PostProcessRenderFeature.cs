using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine;
namespace SleeplessOwl.URPPostProcessing
{
    public class PostProcessRenderFeature : ScriptableRendererFeature
    {
        public PostProcessOrderConfig config;

        private PostProcessRenderPass afterSkyboxPass;
        private PostProcessRenderPass beforeNativePostProcessPass;
        private PostProcessRenderPass afterNativePostProcessPass;

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            
            renderer.EnqueuePass(afterSkyboxPass);
            renderer.EnqueuePass(beforeNativePostProcessPass);
            renderer.EnqueuePass(afterNativePostProcessPass);
        }

        public override void Create()
        {
#if UNITY_EDITOR
            if (config == null)
                return;
            config.OnDataChange = Create;
#endif
            afterSkyboxPass = new PostProcessRenderPass(RenderPassEvent.AfterRenderingSkybox, config);
            beforeNativePostProcessPass = new PostProcessRenderPass(RenderPassEvent.BeforeRenderingPostProcessing, config);
            afterNativePostProcessPass = new PostProcessRenderPass(RenderPassEvent.AfterRenderingPostProcessing, config);
        }
    }
}