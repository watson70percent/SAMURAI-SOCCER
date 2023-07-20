using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using System.IO;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

using SamuraiSoccer.Event;
using SamuraiSoccer.StageContents;

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

        public List<GameObject> team = new List<GameObject>();
        public Team team_stock = new Team();
        public List<GameObject> opp = new List<GameObject>();
        public Team opp_stock = new Team();

        public Transform team_p;
        public Transform opp_p;
        public BallAction ball;
        private GameObject teammate;
        private string teammateName;
        private GameObject opponent;
        private string oppName;
        [NonSerialized]
        public GameObject near_team;
        [NonSerialized]
        public GameObject near_opp;

        public Dictionary<GameObject, Rigidbody> rbs = new Dictionary<GameObject, Rigidbody>();

        private FieldManager field;

        public AudioSource audioSource;
        public AudioClip goalSound;
        public AudioClip startSound;

        private bool m_isPause = true;

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
                _ = FinBoost(isTeam, coeff, finishTime);
            }
        }

        private async UniTask FinBoost(bool isTeam, float coeff, int fin)
        {
            await UniTask.Delay(fin * 1000);
            Boost(isTeam, 1 / coeff);
        }


        void Awake()
        {
            var client = new InMemoryDataTransitClient<string>();
            var oppType = client.Get(StorageKey.KEY_OPPONENT_TYPE);
            teammate = Resources.Load<GameObject>("Teammate");
            teammateName = "our";
            if (oppType == "opponent_Tutorial") teammateName = "ourtutorial";
            opponent = Resources.Load<GameObject>(oppType);
            oppName = oppType;
            client.Set(StorageKey.KEY_OPPONENT_TYPE, oppType);

            field = GetComponent<FieldManager>();
        }

        private void Start()
        {
            InGameEvent.Reset.Subscribe(async u => await LoadMember()).AddTo(this);
            InGameEvent.Pause.Subscribe(Pause).AddTo(this);
            InGameEvent.Play.Subscribe(Play).AddTo(this);
            InGameEvent.Goal.Subscribe(async u => await GoalAction(u)).AddTo(this);
            InGameEvent.Standby.Subscribe(Standby).AddTo(this);
        }

        private void Update()
        {
            (GameObject, float) team_tmp = (default, float.MaxValue);
            foreach(var t in team)
            {
                var d = (t.transform.position - ball.transform.position).sqrMagnitude;
                if (team_tmp.Item2 > d)
                {
                    team_tmp = (t, d);
                }
            }

            (GameObject, float) opp_tmp = (default, float.MaxValue);
            foreach (var o in opp)
            {
                var d = (o.transform.position - ball.transform.position).sqrMagnitude;
                if (opp_tmp.Item2 > d)
                {
                    opp_tmp = (o, d);
                }
            }
            near_team = team_tmp.Item1; 
            near_opp = opp_tmp.Item1;
        }

        private void Pause(bool isPause)
        {
            m_isPause = isPause;
            SetAnimatorSpeed(isPause ? 0 : 1);
        }

        private void Play(Unit _)
        {
            m_isPause = false;
            SetAnimatorSpeed(1);
        }

        private void Standby(Unit _)
        {
            m_isPause = true;
            SetAnimatorSpeed(0);
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
            UIEffectEvent.BlackOutOnNext(5f);
            await UniTask.Delay(4000);
            InGameEvent.StandbyOnNext();
            Init(ball.transform.position.z < (Constants.OppornentGoalPoint.z + Constants.OurGoalPoint.z) / 2);
            await UniTask.Delay(3000);
            audioSource.PlayOneShot(startSound);
            InGameEvent.PlayOnNext();
        }

        private async UniTask LoadMember()
        {
            var file_path1 = Path.Combine(Application.streamingAssetsPath, teammateName + ".json");
            string json = "";
            Debug.Log("filepath is " + file_path1);
            if (file_path1.Contains("://"))
            {
                var www1 = UnityEngine.Networking.UnityWebRequest.Get(file_path1);
                await www1.SendWebRequest();
                json = www1.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(file_path1);
            }
            team_stock = JsonUtility.FromJson<Team>(json);

            var file_path2 = Path.Combine(Application.streamingAssetsPath, oppName + ".json");
            print(Application.streamingAssetsPath);
            if (file_path2.Contains("://"))
            {
                var www2 = UnityEngine.Networking.UnityWebRequest.Get(file_path2);
                await www2.SendWebRequest();
                json = www2.downloadHandler.text;
            }
            else
            {
                json = File.ReadAllText(file_path2);
            }
            opp_stock = JsonUtility.FromJson<Team>(json);

            Init();
        }

        /// <summary>
        /// 選手を殺す。一応瞬時復活もさせる。
        /// </summary>
        /// <param name="dead">死ぬ対象の選手</param>
        public async UniTask Kill(GameObject dead)
        {
            await UniTask.Delay(1000);

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
                else if(!opp.Any())
                {
                    var client = new InMemoryDataTransitClient<GameResult>();
                    client.Set(StorageKey.KEY_WINORLOSE, GameResult.Win);
                    InGameEvent.FinishOnNext();
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
            setting.SetPause(m_isPause);
            setting.rb = temp.GetComponent<Rigidbody>();
            setting.status = status;
            setting.SetMass();

            rbs.Add(temp, setting.rb);

            var enemySlashed = temp.GetComponent<EnemySlashed>();
            if (enemySlashed != null)
            {
                enemySlashed.EasyCPUManager = this;
            }
            
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

            SetAnimatorSpeed(0);
        }

    }
}