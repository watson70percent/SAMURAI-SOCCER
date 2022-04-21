using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugiTemp : MonoBehaviour
{
    [SerializeField] Texture texture;
    [SerializeField] Shader shader;
    [SerializeField] MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        // renderer.materials[0].SetTexture("_Texture", texture);
        Material material = new Material(shader);
        renderer.material = material;
        //renderer.material.SetTexture("_Texture", texture);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
