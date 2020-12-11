using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace SleeplessOwl.URPPostProcessing
{
    public class CustomPostProcessRenderPass : ScriptableRenderPass
    {
        private string displayName;
        private int cycleRT_1 = Shader.PropertyToID("cycleRT_1");
        private int cycleRT_2 = Shader.PropertyToID("cycleRT_2");
        private List<Type> volumeTypeList;
        private List<CustomVolumeComponent> activeVolumeList;
        private GraphicsFormat defaultHDRFormat;
        private GraphicsFormat currentFormat;

        public CustomPostProcessRenderPass(RenderPassEvent passEvent, PostProcessOrderConfig config)
        {
            displayName = $"CustomPostProcessPass {passEvent}";
            renderPassEvent = passEvent;

            activeVolumeList = new List<CustomVolumeComponent>();
            volumeTypeList = new List<Type>();

            var piplineAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (SystemInfo.IsFormatSupported(GraphicsFormat.B10G11R11_UFloatPack32, FormatUsage.Linear | FormatUsage.Render)
                && piplineAsset.supportsHDR)
            {
                defaultHDRFormat = GraphicsFormat.B10G11R11_UFloatPack32;
            }
            else
            {
                defaultHDRFormat = QualitySettings.activeColorSpace == ColorSpace.Linear
                    ? GraphicsFormat.R8G8B8A8_SRGB
                    : GraphicsFormat.R8G8B8A8_UNorm;
            }

            //collect all volume belong this timing
            var allVolumeTypes = VolumeManager.instance.baseComponentTypes;
            foreach (var volumeName in config.GetVolumeList((InjectionPoint)renderPassEvent))
            {
                var volumeType = allVolumeTypes.ToList().Find((t) => { return t.ToString() == volumeName; });

                //check volume type is valid
                if (volumeType == null)
                {
                    Debug.LogError($"Can't find Volume : [{volumeName}] , Remove it from config", config);
                    continue;
                }

                volumeTypeList.Add(volumeType);
            }
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (volumeTypeList.Count == 0)
                return;

            if (renderingData.cameraData.postProcessEnabled == false)
                return;

            //collect current frame active volume
            activeVolumeList.Clear();
            bool isSceneViewCamera = renderingData.cameraData.isSceneViewCamera;
            foreach (var item in volumeTypeList)
            {
                var volumeComp = VolumeManager.instance.stack.GetComponent(item) as CustomVolumeComponent;

                if (volumeComp.IsActive() == false)
                    continue;
                if (isSceneViewCamera && volumeComp.visibleInSceneView == false)
                    continue;

                activeVolumeList.Add(volumeComp);
                volumeComp.SetupIfNeed();
            }

            if (activeVolumeList.Count <= 0)
                return;

            CommandBuffer cb = CommandBufferPool.Get();
            cb.name = displayName;

            var pixelRect = renderingData.cameraData.camera.pixelRect;
            int width = (int)(pixelRect.width);
            int height = (int)(pixelRect.height);
            cb.GetTemporaryRT(cycleRT_1, width, height, 0, FilterMode.Bilinear, defaultHDRFormat);
            cb.GetTemporaryRT(cycleRT_2, width, height, 0, FilterMode.Bilinear, defaultHDRFormat);
            var target = cycleRT_1;
            var source = cycleRT_2;

            for (int i = 0; i < activeVolumeList.Count; i++)
            {
                var volumeComp = activeVolumeList[i];

                if (i == 0)
                {
                    cb.Blit(BuiltinRenderTextureType.CurrentActive, source);
                }
                else
                {
                    Util_PP.Swap(ref target, ref source);
                }

                RenderTargetIdentifier renderTarget;
                bool isFinalVolume = i == activeVolumeList.Count - 1;
                if (isFinalVolume)
                {
                    bool renderToDefaultColorTexture =
                        renderPassEvent == RenderPassEvent.BeforeRenderingPostProcessing
                        || renderPassEvent == RenderPassEvent.AfterRenderingSkybox;

                    if (renderToDefaultColorTexture)
                        renderTarget = Util_PP.ColorBufferId;
                    else
                        renderTarget = BuiltinRenderTextureType.CameraTarget;
                }
                else
                {
                    renderTarget = target;
                }
                cb.SetRenderTarget(renderTarget);

                cb.BeginSample(volumeComp.displayName);
                volumeComp.Render(cb, renderingData.cameraData.camera, source, renderTarget);
                cb.EndSample(volumeComp.displayName);
            }

            cb.ReleaseTemporaryRT(source);
            cb.ReleaseTemporaryRT(target);
            context.ExecuteCommandBuffer(cb);
            cb.Clear();
            CommandBufferPool.Release(cb);
        }
    }
}