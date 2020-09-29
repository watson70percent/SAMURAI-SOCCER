using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTexture : MonoBehaviour
{
    [SerializeField] Texture texture;
    [SerializeField] Texture normalMap;

    private void Start()
    {
        var material = new Material(GetComponent<Renderer>().material);
        if (texture != null) material.mainTexture = texture;
        if (normalMap != null) material.SetTexture("_BumpMap", normalMap);
        GetComponent<Renderer>().material=material;
    }
}
