using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMenber : MonoBehaviour
{

    public Texture[] crowdTextures;
    public Renderer[] crowdRenderers;
    public Material crowdMaterial;
    // Start is called before the first frame update
    void Start()
    {
        var material = new Material(crowdMaterial);
        for(int i = 0; i < 10; i++)
        {
            string texturename = "_Person" + (i + 1).ToString();
            material.SetTexture(texturename, crowdTextures[i%(crowdTextures.Length)]);
        }

        for(int i = 0; i < crowdRenderers.Length; i++)
        {
            crowdRenderers[i].material = material;
        }
    }

}
