using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/Cloud Shadow")]
    public sealed class CloudShadowVolume : PostProcessVolumeComponent
    {
        public FloatParameter ShadowStrength = new FloatParameter(0f);
        public FloatRangeParameter SmoothStepRange = new FloatRangeParameter(new Vector2(.45f, .55f), 0, 1);

        //public TextureParameter NoiseTexture1 = new TextureParameter(null);
        public Vector2Parameter NoiseScale1 = new Vector2Parameter(Vector2.one);
        public FloatParameter NoiseTime1= new FloatParameter(1);

        //public TextureParameter NoiseTexture2 = new TextureParameter(null);
        //public Vector2Parameter NoiseScale2 = new Vector2Parameter(Vector2.one);
        //public FloatParameter NoiseTime2 = new FloatParameter(1);

        public BoolParameter DebugMode = new BoolParameter(false);

        Material material;

        public override bool visibleInSceneView => true;
        public override InjectionPoint InjectionPoint => InjectionPoint.AfterOpaqueAndSky;
        public override bool IsActive()
        {
            return ShadowStrength.value > 0;
        }


        public override void Setup()
        {
            material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Process/Cloud Shadow");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            material.SetMatrix("_InverseView", camera.cameraToWorldMatrix);


            material.SetFloat("_ShadowStrength", ShadowStrength.value);
            material.SetVector("_SmoothStepRange", SmoothStepRange.value);

            //material.SetTexture("_NoiseTexture1", NoiseTexture1.value);
            material.SetVector("_NoiseScale1", NoiseScale1.value);
            material.SetFloat("_NoiseTime1", NoiseTime1.value);

            //material.SetTexture("_NoiseTexture2", NoiseTexture2.value);
            //material.SetVector("_NoiseScale2", NoiseScale2.value);
            //material.SetFloat("_NoiseTime2", NoiseTime2.value);

            material.SetKeyWord("DEBUG_MODE", DebugMode.value);

            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(material, destination);
        }
    }
}