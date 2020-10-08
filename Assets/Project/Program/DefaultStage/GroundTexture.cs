using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTexture : MonoBehaviour
{
    [SerializeField] Texture groundTexture;
    [SerializeField] Texture groundNormalMap;

    private void Start()
    {
        var groundMaterial = new Material(GetComponent<Renderer>().material);
        if (groundTexture != null) groundMaterial.mainTexture = groundTexture;
        if (groundNormalMap != null) groundMaterial.SetTexture("_BumpMap", groundNormalMap);
        GetComponent<Renderer>().material=groundMaterial;
    }
}
