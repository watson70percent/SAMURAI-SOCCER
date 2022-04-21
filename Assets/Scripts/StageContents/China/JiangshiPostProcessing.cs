using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
