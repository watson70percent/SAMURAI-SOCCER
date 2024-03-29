using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;

namespace SamuraiSoccer.UK
{
    public class PanjanRoll : MonoBehaviour
    {

        [SerializeField] float moveSpeed;
        [SerializeField] float rotSpeed;
        bool exploded, playing = true;
        [SerializeField] GameObject rot;
        [SerializeField] int partMax;
        [SerializeField] Rigidbody rb;
        [SerializeField] AudioSource soundEffect;
        [SerializeField] CapsuleCollider capsuleCollider;

        PanjanExplode panjanExplode;
        GameObject fire;
        Transform player;

        // Start is called before the first frame update

        private void Start()
        {
            InGameEvent.Pause.Subscribe(x =>
            {
                playing = !x;
            }).AddTo(this);
            InGameEvent.Play.Subscribe(_ =>
            {
                playing = true;
            }).AddTo(this);
            InGameEvent.Goal.Subscribe(_ =>
            {
                selfExplode();
            }).AddTo(this);
            this.OnTriggerEnterAsObservable()
            .Select(hit => hit.gameObject.tag)
            .Where(tag => tag == "Player" || tag == "Slash")
            .Subscribe(_ =>
            {
                Explode();
                exploded = true;
            }).AddTo(this);
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

        void Explode()
        {
            soundEffect.Play();
            rb.isKinematic = false;
            rb.AddForce(0, 2000, 0);
            capsuleCollider.enabled = false;
            int index = 0;
            foreach (Transform part in rot.transform)
            {
                if (index < 15)
                {
                    panjanExplode = part.gameObject.GetComponent<PanjanExplode>();
                    panjanExplode.gameObject.SetActive(true);
                    panjanExplode.SetFireObject(Instantiate(fire, part.position, Quaternion.identity, part));
                }
                Rigidbody rbPart = part.gameObject.GetComponent<Rigidbody>();
                rbPart.isKinematic = false;
                rbPart.AddForce(transform.forward * 1000);
                index++;
            }
            Destroy(gameObject, 4.0f);
        }

        void selfExplode()
        {
            soundEffect.Play();
            exploded = true;
            foreach (Transform part in rot.transform)
            {
                Rigidbody rbPart = part.gameObject.GetComponent<Rigidbody>();
                rbPart.isKinematic = false;
                rbPart.AddForce(transform.up * 1000);
            }
            Destroy(gameObject, 4.0f);
        }

        public void SetObjects(GameObject fire, Transform player)
        {
            this.fire = fire;
            this.player = player;
        }

    }
}