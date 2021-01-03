using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace SleeplessOwl.URPPostProcessing
{

    [Serializable, VolumeComponentMenu("SleeplessOwl PostProcessing/Glitch")]
    public class GlitchVolume : PostProcessVolumeComponent
    {
        public FloatParameter Speed = new FloatParameter(10f);
        public FloatParameter BlockSize = new FloatParameter(0f);
        public FloatParameter BlockSizeJitter = new FloatParameter(0f);
        public FloatParameter MaxRGBSplitX = new FloatParameter(1f);
        public FloatParameter MaxRGBSplitY = new FloatParameter(1f);

        Material _material;

        static class IDs
        {
            internal static readonly int Speed = Shader.PropertyToID("_Speed");
            internal static readonly int BlockSize = Shader.PropertyToID("_BlockSize");
            internal static readonly int MaxRGBSplitX = Shader.PropertyToID("_MaxRGBSplitX");
            internal static readonly int MaxRGBSplitY = Shader.PropertyToID("_MaxRGBSplitY");
            internal static readonly int Seed = Shader.PropertyToID("_Seed");
        }

        public override bool IsActive() => BlockSize.value > 0;

        public override bool visibleInSceneView => false;


        public override void Initialize()
        {
            _material = CoreUtils.CreateEngineMaterial("SleeplessOwl/Post-Processing/Glitch");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            if (_material == null)
                return;

            var time = Time.time;
            _material.SetInt(IDs.Seed, (int)(time * 10000));

            float jitter = BlockSizeJitter.value * .5f;
            float size = Random.Range(BlockSize.value - jitter, BlockSize.value + jitter);
            _material.SetFloat(IDs.Speed, Speed.value);
            _material.SetFloat(IDs.BlockSize, size);
            _material.SetFloat(IDs.MaxRGBSplitX, MaxRGBSplitX.value);
            _material.SetFloat(IDs.MaxRGBSplitY, MaxRGBSplitY.value);

            cb.SetPostProcessSourceTexture(source);
            cb.DrawFullScreenTriangle(_material, dest);
        }
    }
}