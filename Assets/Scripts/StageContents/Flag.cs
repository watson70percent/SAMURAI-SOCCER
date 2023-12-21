using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents
{
    public class Flag : MonoBehaviour
    {
        [SerializeField]
        private Texture m_texture;
        [SerializeField]
        private Shader m_shader;
        [SerializeField]
        private MeshRenderer[] m_meshes;
        // Start is called before the first frame update
        void Start()
        {
            Material material = new Material(m_shader);
            material.SetTexture("_Texture", m_texture);

            for (int i = 0; i < 4; i++)
            {
                m_meshes[i].material = material;
            }
        }
    }
}
