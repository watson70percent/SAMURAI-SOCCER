using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SamuraiSoccer.StageContents.China
{
    /// <summary>
    /// キョンシーシーンにおいてカメラにポストエフェクトをかける
    /// </summary>
    public class JiangshiPostProcessing : MonoBehaviour
    {
        [SerializeField]
        private Material m_material;
        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, m_material);
        }

        private void Start()
        {
            Camera camera = GetComponent<Camera>();
            camera.depthTextureMode |= DepthTextureMode.Depth;
        }
    }

}
