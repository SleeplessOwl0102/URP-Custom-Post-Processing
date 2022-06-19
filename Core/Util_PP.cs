using UnityEngine;
using UnityEngine.Rendering;

namespace SleeplessOwl.URPPostProcessing
{
    public static class Util_PP
    {
        public static readonly int PostBufferID = Shader.PropertyToID("_PostSource");

        public static void SetPostProcessSourceTexture(this CommandBuffer cb, RenderTargetIdentifier identifier)
        {
            cb.SetGlobalTexture(PostBufferID, identifier);
        }

        public static void DrawFullScreenTriangle(this CommandBuffer cb, Material material, RenderTargetIdentifier destination, int shaderPass = 0)
        {
            CoreUtils.SetRenderTarget(cb, destination);
            cb.DrawProcedural(Matrix4x4.identity, material, shaderPass, MeshTopology.Triangles, 3, 1, null);
        }

        public static void SetKeyWord(this Material mat, string keyWord, bool active)
        {
            if (active)
                mat.EnableKeyword(keyWord);
            else
                mat.DisableKeyword(keyWord);
        }
    }
}