using SleeplessOwl.URPPostProcessing;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace SleeplessOwl.URPPostProcessing
{

    [Serializable, VolumeComponentMenu("Owl/Glitch")]
    public class Glitch : CustomVolumeComponent
    {
        public ClampedFloatParameter block = new ClampedFloatParameter(0, 0, 1);
        

        Material _material;
        float _prevTime;
        float _blockTime;
        int _blockSeed1 = 71;
        int _blockSeed2 = 113;
        int _blockStride = 1;

        static class ShaderIDs
        {
            internal static readonly int BlockSeed1 = Shader.PropertyToID("_BlockSeed1");
            internal static readonly int BlockSeed2 = Shader.PropertyToID("_BlockSeed2");
            internal static readonly int BlockStrength = Shader.PropertyToID("_BlockStrength");
            internal static readonly int BlockStride = Shader.PropertyToID("_BlockStride");
            internal static readonly int Seed = Shader.PropertyToID("_Seed");
        }

        public override bool IsActive() => block.value > 0;

        public override bool visibleInSceneView => false;


        public override void Setup()
        {
            _material = CoreUtils.CreateEngineMaterial("Owl Post-Processing/Glitch");
        }

        public override void Render(CommandBuffer cb, Camera camera, RenderTargetIdentifier source, RenderTargetIdentifier dest)
        {
            if (_material == null)
                return;

            // Update the time parameters.
            var time = Time.time;
            var delta = time - _prevTime;
            _prevTime = time;

            // Block parameters
            var block3 = block.value * block.value * block.value;

            // Shuffle block parameters every 1/30 seconds.
            _blockTime += delta * 60;
            if (_blockTime > 1)
            {
                if (Random.value < 0.09f) _blockSeed1 += 251;
                if (Random.value < 0.29f) _blockSeed2 += 373;
                if (Random.value < 0.25f) _blockStride = Random.Range(1, 32);
                _blockTime = 0;
            }

            // Invoke the shader.
            _material.SetInt(ShaderIDs.Seed, (int)(time * 10000));
            _material.SetFloat(ShaderIDs.BlockStrength, block3);
            _material.SetInt(ShaderIDs.BlockStride, _blockStride);
            _material.SetInt(ShaderIDs.BlockSeed1, _blockSeed1);
            _material.SetInt(ShaderIDs.BlockSeed2, _blockSeed2);

            cb.SetSourceTexture(source);
            cb.DrawFullScreenTriangle(_material, dest);
        }


    }
}