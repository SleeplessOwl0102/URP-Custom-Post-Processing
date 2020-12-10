using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    public static class Util_PP
    {
        public static readonly int BlitTexID = Shader.PropertyToID("_BlitTex");
        public static readonly int PostSourceID = Shader.PropertyToID("_PostSource");
        public static readonly RenderTargetIdentifier ColorBufferId = Shader.PropertyToID("_CameraColorTexture");
        public static readonly RenderTargetIdentifier DepthBufferId = Shader.PropertyToID("_CameraDepthTexture");

        public static void SetSourceTexture(this CommandBuffer cb, RenderTargetIdentifier identifier)
        {
            cb.SetGlobalTexture(PostSourceID, identifier);
        }

        public static void DrawFullScreenTriangle(this CommandBuffer cb, Material material, RenderTargetIdentifier destination, int shaderPass = 0)
        {
            CoreUtils.SetRenderTarget(cb, destination);
            cb.DrawProcedural(Matrix4x4.identity, material, shaderPass, MeshTopology.Triangles, 3, 1, null);
        }

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static void SetKeyWord(this Material mat, string keyWord, bool active)
        {
            if (active)
                mat.EnableKeyword(keyWord);
            else
                mat.DisableKeyword(keyWord);
        }

        public static bool ImplementsInterface(this Type type, Type interfaceType)
        {
            Type[] intfs = type.GetInterfaces();
            for (int i = 0; i < intfs.Length; i++)
            {
                if (intfs[i] == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }
    }
}