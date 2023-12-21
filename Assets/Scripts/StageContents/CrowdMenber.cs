using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents
{
    public class CrowdMenber : MonoBehaviour
    {
        [SerializeField]
        private Texture[] m_crowdTextures;
        [SerializeField]
        private Renderer[] m_crowdRenderers;
        [SerializeField]
        private Material m_crowdMaterial;
        // Start is called before the first frame update
        void Start()
        {
            var material = new Material(m_crowdMaterial);
            for (int i = 0; i < 10; i++)
            {
                string texturename = "_Person" + (i + 1).ToString();
                material.SetTexture(texturename, m_crowdTextures[i % (m_crowdTextures.Length)]);
            }

            for (int i = 0; i < m_crowdRenderers.Length; i++)
            {
                m_crowdRenderers[i].material = material;
            }
        }

    }
}
