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
            if(afterSkyboxPass != null)
                renderer.EnqueuePass(afterSkyboxPass);
                
            if(beforeNativePostProcessPass != null)
                renderer.EnqueuePass(beforeNativePostProcessPass);

            if(afterNativePostProcessPass != null)
                renderer.EnqueuePass(afterNativePostProcessPass);
        }

        public override void Create()
        {
#if UNITY_EDITOR
            if (config == null)
                return;
            config.OnDataChange = Create;
#endif

            if (config.afterSkybox.Count > 0)
            {
                afterSkyboxPass = new PostProcessRenderPass(RenderPassEvent.AfterRenderingSkybox, config);
            }

            if (config.beforePostProcess.Count > 0)
            {
                beforeNativePostProcessPass = new PostProcessRenderPass(RenderPassEvent.BeforeRenderingPostProcessing, config);
            }

            if (config.afterPostProcess.Count > 0)
            {
                afterNativePostProcessPass = new PostProcessRenderPass(RenderPassEvent.AfterRenderingPostProcessing, config);
            }
        }
    }
}