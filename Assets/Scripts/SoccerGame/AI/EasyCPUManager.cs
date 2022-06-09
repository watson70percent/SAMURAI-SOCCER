using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.IO;
using Unity.Collections;
using System;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

using SamuraiSoccer.Event;


namespace SamuraiSoccer.SoccerGame.AI
{
    /// <summary>
    /// CPUを操作するクラス
    /// </summary>
    [DefaultExecutionOrder(5)]
    [RequireComponent(typeof(FieldManager))]
    public class EasyCPUManager : MonoBehaviour
    {
        public GameObject samurai;
        public GameObject referee;

        public List<GameObject> team;
        public Team team_stock;
        public List<GameObject> opp;

        public Team opp_stock;

        public Transform team_p;
        public Transform opp_p;
        public BallAction ball;
        private GameObject teammate;
        private GameObject opponent;
        [NonSerialized]
        public GameObject near_team;
        [NonSerialized]
        public GameObject near_opp;

        public Dictionary<GameObject, Rigidbody> rbs = new Dictionary<GameObject, Rigidbody>();

        private FieldManager field;

        public AudioSource audioSource;
        public AudioClip goalSound;
        public AudioClip startSound;
        public Image goalImage;

        /// <summary>
        /// 味方の人数
        /// </summary>
        public int TeamMemberCount
        {
            get
            {
                return team.Count + team_stock.member.Count;
            }
        }

        /// <summary>
        /// 敵の人数
        /// </summary>
        public int OpponentMemberCount
        {
            get
            {
                return opp.Count + opp_stock.member.Count;
            }
        }

        /// <summary>
        /// ブースト
        /// </summary>
        /// <param name="isTeam">味方にブーストか</param>
        /// <param name="coeff">倍率</param>
        /// <param name="finishTime">ブースト終了時間</param>
        public void Boost(bool isTeam, float coeff, int finishTime = 0)
        {
            if (isTeam)
            {
                foreach (var member in team)
                {
                    var s = member.GetComponent<EasyCPU>().status;
                    s.fast *= coeff;
                    s.power *= coeff;
                }

                foreach (var member in team_stock.member)
                {
                    member.fast *= coeff;
                    member.power *= coeff;
                }
            }
            else
            {
                foreach (var member in opp)
                {
                    var s = member.GetComponent<EasyCPU>().status;
                    s.fast *= coeff;
                    s.power *= coeff;
                }

                foreach (var member in opp_stock.member)
                {
                    member.fast *= coeff;
                    member.power *= coeff;
                }
            }

            if (finishTime != 0)
            {
                StartCoroutine(FinBoost(isTeam, coeff, finishTime));
            }
        }

        private IEnumerator FinBoost(bool isTeam, float coeff, int fin)
        {
            yield return new WaitForSeconds(fin);
            Boost(isTeam, 1 / coeff);
        }


        void Awake()
        {
            teammate = Resources.Load<GameObject>("Teammate");
            opponent = Resources.Load<GameObject>(OpponentName.name);

            field = GetComponent<FieldManager>();
            // opponent = Resources.Load<GameObject>(OpponentName.name);
            StartCoroutine(LoadMember());
        }

        private void Start()
        {
            InGameEvent.Pause.Subscribe(Pause);
            InGameEvent.Standby.Subscribe(_ => SetAnimatorSpeed(0));
        }

        private void Update()
        {
            near_team = team.Aggregate((cur, next) => { return (ball.transform.position - cur.transform.position).sqrMagnitude > (ball.transform.position - next.transform.position).sqrMagnitude ? next : cur; });
            near_opp = opp.Aggregate((cur, next) => { return (ball.transform.position - cur.transform.position).sqrMagnitude > (ball.transform.position - next.transform.position).sqrMagnitude ? next : cur; });
        }

        private void Pause(bool isPause)
        {
            SetAnimatorSpeed(isPause ? 0 : 1);
        }

        private void SetAnimatorSpeed(float speed)
        {
            foreach (var t in team)
            {
                t.GetComponentInChildren<Animator>().speed = speed;
            }

            foreach (var t in opp)
            {
                t.GetComponentInChildren<Animator>().speed = speed;
            }
        }

        private async UniTask GoalAction(Unit _)
        {
            audioSource.PlayOneShot(goalSound);
            var __ = GoalBlack();
            await UniTask.Delay(4000);
            InGameEvent.StandbyOnNext();
            Init(ball.transform.position.z < (Constants.OppornentGoalPoint.z + Constants.OurGoalPoint.z) / 2);
            await UniTask.Delay(2000);
            audioSource.PlayOneShot(startSound);
            InGameEvent.PlayOnNext();
        }

        private async UniTask GoalBlack()
        {
            float time = 0;
            while (time < 5)
            {
                goalImage.color = new Color(0, 0, 0, 1 - Mathf.Abs(4 - time) * 2);
                await UniTask.Yield();
                time += Time.deltaTime;
            }
        }


        private IEnumerator LoadMember()
        {
            var file_path1 = Path.Combine(Application.streamingAssetsPath, "our.json");
            string json = "";
            Debug.Log("filepath is " + file_path1);
            if (file_path1.Contains("://"))
            {
                var www1 = UnityEngine.Networking.UnityWebRequest.Get(file_path1);
                yield return www1.SendWebRequest();
                json = www1.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(file_path1);
            }
            team_stock = JsonUtility.FromJson<Team>(json);

            var file_path2 = Path.Combine(Application.streamingAssetsPath, OpponentName.name + ".json");
            print(Application.streamingAssetsPath);
            if (file_path2.Contains("://"))
            {
                var www2 = UnityEngine.Networking.UnityWebRequest.Get(file_path2);
                yield return www2.SendWebRequest();
                json = www2.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(file_path2);
            }
            opp_stock = JsonUtility.FromJson<Team>(json);

            Init();

            yield return null;

        }

        /// <summary>
        /// 選手を殺す。一応瞬時復活もさせる。
        /// </summary>
        /// <param name="dead">死ぬ対象の選手</param>
        public void Kill(GameObject dead)
        {
            bool ally = dead.GetComponent<EasyCPU>().status.ally;
            opp.Remove(dead);
            team.Remove(dead);
            Destroy(dead);

            rbs.Remove(dead);

            if (ally)
            {
                if (team_stock.member.Any())
                {
                    Sporn(team_stock.member[0], field.AdaptPosition(Constants.TeammateSpornPoint));
                    team_stock.member.RemoveAt(0);
                }
            }
            else
            {
                if (opp_stock.member.Any())
                {
                    Sporn(opp_stock.member[0], field.AdaptPosition(Constants.OppornentSpornPoint));
                    opp_stock.member.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// 選手復活
        /// </summary>
        /// <param name="status">ステータス</param>
        /// <param name="pos">復活場所</param>
        /// <return>復活した選手</return>
        public GameObject Sporn(PersonalStatus status, Vector3 pos)
        {
            GameObject temp;
            if (status.ally)
            {
                temp = Instantiate(teammate, pos, Quaternion.identity * field.rotation.rotation, team_p);
            }
            else
            {
                temp = Instantiate(opponent, pos, Quaternion.LookRotation(Vector3.back, Vector3.up) * field.rotation.rotation, opp_p);
            }

            var setting = temp.GetComponent<EasyCPU>();
            setting.ball = ball.transform;
            setting.manager = this;
            setting.field = field;
            setting.rb = temp.GetComponent<Rigidbody>();
            setting.status = status;
            setting.SetMass();

            rbs.Add(temp, setting.rb);

            if (status.ally)
            {
                team.Add(temp);
            }
            else
            {
                opp.Add(temp);
            }

            return temp;
        }

        /// <summary>
        /// 初期化。選手の生成をしてる。
        /// </summary>
        public void Init(bool centerIsOppornent = true)
        {
            foreach (var t in team)
            {
                team_stock.member.Insert(0, t.GetComponent<EasyCPU>().status);

                Destroy(t);
            }

            team.Clear();

            foreach (var t in opp)
            {

                opp_stock.member.Insert(0, t.GetComponent<EasyCPU>().status);

                Destroy(t);
            }

            opp.Clear();


            int teamCount = team_stock.member.Count > 11 ? 11 : team_stock.member.Count;
            int oppCount = opp_stock.member.Count > 11 ? 11 : opp_stock.member.Count;

            if (centerIsOppornent)
            {
                for (int i = 0; i < teamCount; i++)
                {
                    Sporn(team_stock.member[0], field.AdaptPosition(Constants.TeammateInitialSpornPointCenterOppornent[i]));
                    team_stock.member.RemoveAt(0);
                }

                for (int i = 0; i < oppCount; i++)
                {
                    Sporn(opp_stock.member[0], field.AdaptPosition(Constants.OpprnentInitialSpornPointCenterOppornent[i]));
                    opp_stock.member.RemoveAt(0);
                }
            }
            else
            {
                for (int i = 0; i < teamCount; i++)
                {
                    Sporn(team_stock.member[0], field.AdaptPosition(Constants.TeammateInitialSpornPointCenterTeam[i]));
                    team_stock.member.RemoveAt(0);
                }

                for (int i = 0; i < oppCount; i++)
                {
                    Sporn(opp_stock.member[0], field.AdaptPosition(Constants.OpprnentInitialSpornPointCenterTeam[i]));
                    opp_stock.member.RemoveAt(0);
                }
            }


            ball.gameObject.transform.position = (Constants.OppornentGoalPoint + Constants.OurGoalPoint) / 2 + new Vector3(0, 0.5f, 0);
            ball.rb.velocity = Vector3.zero;
            samurai.transform.position = new Vector3(35.7f, 0, 59.6f);
            referee.transform.position = new Vector3(38, 0, 69);
        }

    }
}