using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer;

namespace SamuraiSoccer.Player
{
    /// <summary>
    /// ゴールを斬撃で切れるように
    /// </summary>
    public class GoalCut : MonoBehaviour
    {

        private bool m_cutted;

        private void OnTriggerEnter(Collider other)
        {
            print(other.transform.root.name);
            if (!m_cutted)
            {
                if (other.transform.root.tag == "Slash")
                {
                    Cut();
                    m_cutted = true;
                }
            }
        }

        void Cut()
        {
            Mesh[] meshes;
            Vector3 vec = Vector3.up * 3;
            meshes = MeshCut.CutMesh(this.gameObject, transform.position + vec, Vector3.up);
            GetComponent<MeshFilter>().mesh = meshes[1];

            GameObject fragment = new GameObject("fragment", typeof(MeshFilter), typeof(MeshRenderer));
            fragment.transform.position = this.transform.position;
            fragment.transform.rotation = this.transform.rotation;
            fragment.transform.localScale = this.transform.localScale;
            fragment.GetComponent<MeshFilter>().mesh = meshes[0];
            fragment.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
            Destroy(fragment.GetComponent<BoxCollider>());

            fragment.AddComponent<Rigidbody>().AddForce(new Vector3(0, 0.6f, 0) * 20, ForceMode.Impulse);


        }
    }

}