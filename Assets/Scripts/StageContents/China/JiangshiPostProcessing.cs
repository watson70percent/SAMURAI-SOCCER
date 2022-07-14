using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiSoccer.StageContents.China
{
    public class JiangshiPostProcessing : MonoBehaviour
    {
        public Material material;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {

            Graphics.Blit(src, dest, material);
        }

        private void Start()
        {
            Camera camera = GetComponent<Camera>();
            camera.depthTextureMode |= DepthTextureMode.Depth;
        }
    }

}
