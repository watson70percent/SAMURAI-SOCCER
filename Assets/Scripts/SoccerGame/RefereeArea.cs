using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.SoccerGame
{
    public class RefereeArea : MonoBehaviour
    {
        [Tooltip("審判の視界の視野角")]
        float maxang = 60;
        public float MaxAngle { get { return maxang; } }
        public void SerMaxAngle(float newValue) { maxang = newValue; }
        [Tooltip("審判の視界の距離")]
        float areaSize = 10;
        public float AreaSize { get { return areaSize; } }
        public void SerAreaSize(float newValue) { areaSize = newValue; }

        public ParticleSystem surprisedMark;
        public Button attackButton;
        public Penalty penaltyManager;
        private MeshFilter meshFilter;
        [Space(10)]
        public bool useObstacles = false;
        Transform player;

        
        enum State
        {
            Playing,Else
        }
        State state;

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.Play.Subscribe(x=> { state = State.Playing; }).AddTo(this);
            InGameEvent.Standby.Subscribe(x=> { state = State.Else; }).AddTo(this);
            InGameEvent.Goal.Subscribe(x=> { state = State.Else; }).AddTo(this);

            //MeshMaker();

            PlayerEvent.Attack.Subscribe(x=> { FoulCheck(); }).AddTo(this);
            
            meshFilter = GetComponent<MeshFilter>();
            player = GameObject.FindGameObjectWithTag("Player").transform;

        }

        // Update is called once per frame
        void Update()
        {
            if (useObstacles) DynamicMeshMaker();
        }


        public void MeshMaker()
        {



            var mesh = new Mesh();
            var verticles = new List<Vector3> { Vector3.zero };

            float deltaang = maxang / 30;
            float ang;
            int count = 0;

            Vector3 vec;

            for (ang = -maxang; ang <= maxang; ang += deltaang)
            {

                count++;
                vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI)) * areaSize;
                verticles.Add(vec);
            }


            vec = new Vector3(Mathf.Sin(maxang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(maxang / 360 * 2 * Mathf.PI)) * areaSize;
            verticles.Add(vec);
            count++;


            mesh.SetVertices(verticles);

            int[] triangles = new int[(count - 1) * 3];
            for (int i = 0; i < count - 1; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            mesh.SetTriangles(triangles, 0);

            var meshFilter = GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
        }

        void DynamicMeshMaker()
        {
            var mesh = new Mesh();
            var verticles = new List<Vector3> { Vector3.zero };

            float deltaang = maxang / 80;
            float ang;
            int count = 0;

            for (ang = -maxang; ang <= maxang; ang += deltaang)
            {

                count++;
                var vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI));
                Vector3 vec2;
                Ray ray = new Ray(transform.position, transform.TransformDirection(vec));
                // Debug.DrawRay(transform.position, transform.TransformDirection(vec));
                RaycastHit hit;
                int layerMask = 1 << 8;
                vec2 = (Physics.Raycast(ray, out hit, areaSize, layerMask)) ? vec * hit.distance : vec * areaSize;

                verticles.Add(vec2);
            }
            mesh.SetVertices(verticles);

            int[] triangles = new int[(count - 1) * 3];
            for (int i = 0; i < count - 1; i++)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
            mesh.SetTriangles(triangles, 0);


            meshFilter.mesh = mesh;
        }

        void FoulCheck()
        {

            if (state != State.Playing)
            {
                return;
            }

            var vec = player.position - transform.position;//審判からプレイヤーまでのベクトル
            if (vec.magnitude > areaSize || Vector3.Dot(vec.normalized, transform.forward) < Mathf.Cos(maxang / 360 * 2 * Mathf.PI)) { return; }
            print(areaSize);

            print(vec.magnitude > areaSize);
            Ray ray = new Ray(transform.position + Vector3.up, vec);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, areaSize) ? (hit.collider.tag == "Player") : false)
            {
                surprisedMark.Play();
                attackButton.enabled = false;
                Invoke("PenaltyRemoval", 1);
                penaltyManager.YellowCard();
            }

        }
        void PenaltyRemoval()
        {
            attackButton.enabled = true;
        }
    }

}