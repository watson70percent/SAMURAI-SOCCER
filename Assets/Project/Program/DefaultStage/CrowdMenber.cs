using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdMenber : MonoBehaviour
{

    [SerializeField] Texture[] textures;
    [SerializeField] Renderer[] renderers;
    [SerializeField] Material mat;
    // Start is called before the first frame update
    void Start()
    {
        var material = new Material(mat);
        for(int i = 0; i < 10; i++)
        {
            string texturename = "_Person" + (i + 1).ToString();
            material.SetTexture(texturename, textures[i%(textures.Length)]);
        }

        for(int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = material;
        }
    }

}
