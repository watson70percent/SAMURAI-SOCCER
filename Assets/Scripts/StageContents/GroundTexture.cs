using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents
{
    public class GroundTexture : MonoBehaviour
    {
        [SerializeField]
        private Texture m_groundTexture;
        [SerializeField]
        private Texture m_groundNormalMap;

        private void Start()
        {
            var groundMaterial = new Material(GetComponent<Renderer>().material);
            if (m_groundTexture != null) groundMaterial.mainTexture = m_groundTexture;
            if (m_groundNormalMap != null) groundMaterial.SetTexture("_BumpMap", m_groundNormalMap);
            GetComponent<Renderer>().material = groundMaterial;
        }
    }
}