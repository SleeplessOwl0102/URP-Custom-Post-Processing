using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/Outline")]
    public sealed class OutlineVolume : PostProcessVolumeComponent
    {
        public FloatParameter SampleDistance = new FloatParameter(0);
        public FloatParameter StrengthPow = new FloatParameter(30);

        Material material;
        public override bool visibleInSceneView => true;
        public override InjectionPoint InjectionPoint => InjectionPoint.AfterOpaqueAndSky;
        public override bool IsActive()
        {
            return SampleDistance.value != 0;
        }

        static class IDs
        {
            internal readonly static int _sampleDistance = Shader.PropertyToID("_sampleDistance");
            internal readonly static int _strengthPow = Shader.PropertyToID("_strengthPow");
        }

        public void UpdateParameter()
        {
            material.SetFloat(IDs._sampleDistance, SampleDistance.value);
            material.SetFloat(IDs._strengthPow, StrengthPow.value);
        }

        public override void Initialize()
        {
            material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Processing/Outline");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            UpdateParameter();
            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}