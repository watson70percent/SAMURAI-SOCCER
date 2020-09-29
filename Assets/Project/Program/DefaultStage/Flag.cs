using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    [SerializeField] Texture texture;
    [SerializeField] Shader shader;
    [SerializeField] MeshRenderer[] meshes;
    // Start is called before the first frame update
    void Start()
    {
        Material material = new Material(shader);
        material.SetTexture("_Texture", texture);

        for (int i = 0; i < 4; i++)
        {
            meshes[i].material = material;
        }


    }


}
