using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRX;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UK
{
    public class PanjanRoll : MonoBehaviour
    {
        [SerializedField] Transform player;
        [SerializedField] float moveSpeed;
        [SerializedField] float rotSpeed;
        bool exploded, playing;
        [SerializedField] GameObject rot;
        [SerializedField] GameObject fire;
        [SerializedField] int partMax;
        [SerializedField] RigidBody rb;
        [SerializedField] AudioSource audio;
        [SerializedField] CapsuleCollider capsuleCollider;

        GameObject fire;
        PanjanExplode panjanExplode;

        // Start is called before the first frame update

        private void Start()
        {
            InGameEvent.Reset.Subscribe(_ =>
            {

            });
            InGameEvent.Standby.Subscribe(_ =>
            {

            });
            InGameEvent.Pause.Subscribe(_ =>
            {
                playing = false;
            });
            InGameEvent.Play.Subscribe(_ =>
            {
                playing = true;
            });
            InGameEvent.Finish.Subscribe(_ =>
            {

            });
            InGameEvent.Goal.Subscribe(_ =>
            {
                selfExplode();
            });
        }

        // Update is called once per frame
        void Update()
        {
            if (!playing) return;
            if (!exploded)
            {
                // 制限なしの回転を求め...
                var rotation = Quaternion.LookRotation(player.transform.position + Vector3.up * 2 - transform.position);

                // その回転角を_maxAngleまでに制限した回転を作り、それをrotationにセットする
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotSpeed * Time.deltaTime);

                rot.transform.Rotate(moveSpeed, 0, 0);
                transform.position += transform.forward * moveSpeed * Time.deltaTime;
            }
        }



        private void OnTriggerEnter(Collider other)
        {
            if (!exploded)
            {
                if (other.transform.root.tag == "Slash" || other.gameObject.tag == "Player")
                {
                    Explode();
                    exploded = true;
                }
            }
        }

        void Explode()
        {
            audio.Play();
            rb.isKinematic = false;
            rb.AddForce(0, 2000, 0);
            capsuleCollider.enabled = false;
            int index = 0;
            foreach (Transform part in rot.transform)
            {
                if (index < 15)
                {
                    Instantiate(fire, part.position, Quaternion.identity, part);
                    panjanExplode = part.gameObject.GetComponent<PanjanExplode>();
                    panjanExplode.SetActive(true);
                    panjanExplode.SetFireObject();
                }
                RigidBody rbPart = part.gameObject.GetComponent<RigidBody>();
                rbPart.isKinematic = false;
                rbPart.AddForce(transform.forward * 1000);
                index++;
            }
            Destroy(gameObject, 4.0f);
        }

        void selfExplode()
        {
            audio.Play();
            exploded = true;
            foreach (Transform part in rot.transform)
            {
                RigidBody rbPart = part.gameObject.GetComponent<RigidBody>();
                rbPart.isKinematic = false;
                rbPart.AddForce(transform.up * 1000);
            }
            Destroy(gameObject, 4.0f);
        }

        void SetFireObject(GameObject fire){
            this.fire = fire;
        }

    }
}