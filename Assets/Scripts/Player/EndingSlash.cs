using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.SoccerGame.AI;
using UniRx.Triggers;
using UniRx;
using SamuraiSoccer.SoccerGame;
using System.Linq;

namespace SamuraiSoccer.Player
{
    public class EndingSlash : MonoBehaviour
    {

        public Animator animator;
        ParticleSystem.MainModule particle;
        float time;
        float alpha;
        public AudioClip slash;
        bool m_isAttackBall = true;
        // Start is called before the first frame update
        void Start()
        {
            particle = GetComponent<ParticleSystem>().main;
            alpha = particle.startColor.color.a;

            this.OnCollisionEnterAsObservable().Subscribe(collision => OnHit(collision));

        }


        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            particle.startColor = new Color(particle.startColor.color.r, particle.startColor.color.g, particle.startColor.color.b, alpha - time);


            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Finish"))
            {
                Destroy(transform.root.gameObject);
            }


        }


        private void OnHit(Collision collision)
        {
            
            var slashnormal = -transform.forward; //スラッシュの法線
            print(collision.contacts[0].point);
            print(slashnormal);

            Mesh[] meshes;
            //Vector3 vec = Vector3.up * 3;
            meshes = MeshCut.CutMesh(collision.gameObject, collision.contacts[0].point , slashnormal); //切断
            collision.gameObject.GetComponent<MeshFilter>().mesh = meshes[1];//衝突したオブジェクトのメッシュを切断したものに変更
            collision.gameObject.GetComponent<MeshCollider>().sharedMesh= meshes[1];
            GameObject fragment = new GameObject("fragment", typeof(MeshFilter), typeof(MeshRenderer),typeof(MeshCollider)); //切断した方片方のメッシュを入れるGameObject
            fragment.transform.position = collision.gameObject.transform.position;
            fragment.transform.rotation = collision.gameObject.transform.rotation;
            fragment.transform.localScale = collision.gameObject.transform.localScale;
            fragment.GetComponent<MeshFilter>().mesh = meshes[0];
            fragment.GetComponent<MeshRenderer>().materials = collision.gameObject.GetComponent<MeshRenderer>().materials;
            fragment.GetComponent<MeshCollider>().sharedMesh= meshes[0];
            fragment.GetComponent<MeshCollider>().convex = true;
            //Destroy(fragment.GetComponent<BoxCollider>());

            fragment.AddComponent<Rigidbody>().AddForce(new Vector3(0, -0.01f, 0) * 20, ForceMode.Impulse);
        }
    }
}