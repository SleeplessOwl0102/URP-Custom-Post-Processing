using SleeplessOwl.URPPostProcessing;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.Volume
{
    [Serializable, VolumeComponentMenu("Custom Post-processing/Outline")]
    public sealed class OutlineVolume : CustomVolumeComponent
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

        public void UpdateParameter()
        {
            material.SetFloat("_sampleDistance", SampleDistance.value);
            material.SetFloat("_strengthPow", StrengthPow.value);
        }

        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("URP Custom PostEffect/Outline");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            UpdateParameter();
            cb.SetSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}