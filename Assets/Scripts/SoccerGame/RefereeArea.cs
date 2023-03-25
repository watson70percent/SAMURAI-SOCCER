using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.SoccerGame
{
    public class RefereeArea : MonoBehaviour
    {
        [Tooltip("審判の視界の視野角")]
        float m_maxang = 60;
        public float MaxAngle { get { return m_maxang; } }
        public void SetMaxAngle(float newValue) { m_maxang = newValue; }
        [Tooltip("審判の視界の距離")]
        float m_areaSize = 10;
        public float AreaSize { get { return m_areaSize; } }
        public void SetAreaSize(float newValue) { m_areaSize = newValue; }

        public ParticleSystem surprisedMark;
        public Button attackButton;
        private MeshFilter m_meshFilter;
        [Space(10)]
        public bool useObstacles = false;
        Transform m_player;

        private int m_penaltyCount = 0; //反則回数 0:警告, 1:退場


       
        private IReadOnlyReactiveProperty<bool> m_isPlaying =
            Observable.Merge(
                    InGameEvent.Play.Select(x => true),
                    InGameEvent.Pause.Select(isPause => !isPause),
                    InGameEvent.Goal.Select(x => false)
                ).ToReactiveProperty(false);

        // Start is called before the first frame update
        void Start()
        {

            PlayerEvent.Attack.Subscribe(x=> {
                FoulCheck().Forget();
            }).AddTo(this);
            
            m_meshFilter = GetComponent<MeshFilter>();
            m_player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        // Update is called once per frame
        void Update()
        {
            if (useObstacles) DynamicMeshMaker();
        }


        public void MeshMaker() //最初に審判の視界を表す赤い弧を生成
        {
            var mesh = new Mesh();
            var verticles = new List<Vector3> { Vector3.zero };

            float deltaang = m_maxang / 30;
            float ang;
            int count = 0;

            Vector3 vec;

            for (ang = -m_maxang; ang <= m_maxang; ang += deltaang)
            {

                count++;
                vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI)) * m_areaSize;
                verticles.Add(vec);
            }


            vec = new Vector3(Mathf.Sin(m_maxang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(m_maxang / 360 * 2 * Mathf.PI)) * m_areaSize;
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

        void DynamicMeshMaker() //動的に審判の視界を生成、障害物があるときに有効
        {
            var mesh = new Mesh();
            var verticles = new List<Vector3> { Vector3.zero };

            float deltaang = m_maxang / 80;
            float ang;
            int count = 0;

            for (ang = -m_maxang; ang <= m_maxang; ang += deltaang)
            {

                count++;
                var vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI));
                Vector3 vec2;
                Ray ray = new Ray(transform.position, transform.TransformDirection(vec));
                // Debug.DrawRay(transform.position, transform.TransformDirection(vec));
                RaycastHit hit;
                int layerMask = 1 << 8;
                vec2 = (Physics.Raycast(ray, out hit, m_areaSize, layerMask)) ? vec * hit.distance : vec * m_areaSize;

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


            m_meshFilter.mesh = mesh;
        }


        async UniTask FoulCheck()
        {
            for(int i = 0; i < 4; i++) // たまにRaycastをすり抜けるので4回くらいチェックを行う
            {
                bool result = Check();
                if (result) return;

                await UniTask.DelayFrame(1);
            }
        }

        bool Check() //審判に攻撃が見られていないかチェック
        {
            
            if (!m_isPlaying.Value) //プレイ中じゃなければreturn
            {
                return false;
            }
            

            var vec = m_player.position + Vector3.up - (transform.position + Vector3.up*0.5f);//審判からプレイヤーまでのベクトル
            vec = vec.normalized;
            if (vec.magnitude > m_areaSize || Vector3.Dot(vec.normalized, transform.forward) < Mathf.Cos(m_maxang / 360 * 2 * Mathf.PI)) { return false; } //審判の視界の範囲外ならfalse

            Ray ray = new Ray(transform.position + Vector3.up*0.5f, vec);

            //プレイヤー以外の障害物がある場合にはプレイヤーが死角に入っていないか確認
            RaycastHit hit;
            int layerMask = 1 << 8 | 1 << 10; 
            if (Physics.Raycast(ray, out hit, m_areaSize,layerMask) ? (hit.collider.tag == "Player") : false)
            {
                InGameEvent.PenaltyOnNext(m_penaltyCount);
                m_penaltyCount = 1; //ペナルティ回数は0or1のため強制的に1に変更
                surprisedMark.Play();
                attackButton.enabled = false;
                Invoke("PenaltyRemoval", 1);

                return true;
            }
            


            return false;

        }
        void PenaltyRemoval()
        {
            attackButton.enabled = true;
        }
    }

}