using System;
using UnityEngine;
using Random = UnityEngine.Random;
using UniRx;
using SamuraiSoccer.Event;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.SoccerGame
{
    /// <summary>
    /// ボールに対するアクションをするクラス．（ボールにくっつける．）
    /// </summary>
    public class BallAction : MonoBehaviour
    {
        [NonSerialized]
        public Rigidbody rb;
        public FieldManager info;
        public ScoreManager scoreManager;
        private static readonly float sqrt3 = Mathf.Sqrt(3);
        private static readonly float sqrt2 = Mathf.Sqrt(2);
        private static readonly float gravity = 9.8f;

        private Vector3 velocity;
        private Vector3 angularVelocity;

        private int calledNum = 0; // ゴールイベントが複数呼び出されたか監視する番号

        private static Subject<BallActionCommand> commandStream = new();
        private static Subject<PassInfo> passStream = new();

        public static IObservable<PassInfo> PassInfo { get => passStream; }

        [NonSerialized]
        public bool last_touch;
        [NonSerialized]
        public GameObject owner;

        private bool isPause; // ポーズ中かどうか

        /// <summary>
        /// ボールに対するアクションを行う．
        /// </summary>
        /// <param name="command">コマンド．</param>
        public static void CommandStreamOnNext(BallActionCommand command)
        {
            commandStream.OnNext(command);
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            InGameEvent.Play.Subscribe(Play).AddTo(this);
            InGameEvent.Pause.Subscribe(Pause).AddTo(this);
            commandStream.ThrottleFirst(TimeSpan.FromSeconds(0.1), Scheduler.MainThreadFixedUpdate).Subscribe(Command).AddTo(this);
        }

        private void Update()
        {
            rb.angularDrag = info.info.Getdrag(gameObject.transform.position);
            var vel = rb.velocity;
            if (vel.sqrMagnitude < 1)
            {
                if (transform.position.x <= 0.5)
                {
                    vel.x += 1;
                }
                else if (transform.position.x >= 49.5)
                {
                    vel.x -= 1;
                }
                if (transform.position.z <= 0.5)
                {
                    vel.z += 1;
                }
                else if(transform.position.z >= 99.5)
                {
                    vel.z -= 1;
                }
                rb.velocity = vel;
            }
        }

        private void Pause(bool isPause)
        {
            if (isPause)
            {
                velocity = rb.velocity;
                rb.velocity = Vector3.zero;
                angularVelocity = rb.angularVelocity;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            else
            {
                Play(new Unit());
            }
            this.isPause = isPause;
        }

        private void Play(Unit _)
        {
            rb.isKinematic = false;
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
            // ゴールイベントが呼び出された数を初期化
            calledNum = 0;
            isPause = false;
        }

        /// <summary>
        /// コマンドの処理を振り分け．
        /// </summary>
        /// <param name="command">コマンド．</param>
        private void Command(BallActionCommand command)
        {
            switch (command) {
                case TrapCommand c: Trap(c); break;
                case PassCommand c: Pass(c); break;
                case ShootCommand c: Shoot(c); break;
                default: throw new ArgumentException("未実装のコマンドです．");
            }
        }


        /// <summary>
        /// ドリブルっぽいもの。StartCorutineじゃなくてイテレーターで操作してほしい。
        /// </summary>
        /// <returns></returns>
        public async UniTask Dribble(DribbleCommand c)
        {
            last_touch = c.m_status.ally;
            rb.AddForce(new Vector3(Mathf.Sin(owner.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * 4, 0, Mathf.Cos(owner.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * 4), ForceMode.Impulse);
            await UniTask.Yield();
            for (int i = 0; i < 4; i++)
            {
                await UniTask.Yield();
            }
            Vector3 pos = transform.position;
            for (int i = 0; i < 5; i++)
            {
                pos.x = Mathf.Lerp(pos.x, owner.transform.position.x, i / 20.0f);
                pos.z = Mathf.Lerp(pos.z, owner.transform.position.z, i / 20.0f);
                transform.position = pos;

                await UniTask.Yield();
            }
        }

        /// <summary>
        /// ボールを盗むときの関数
        /// </summary>
        /// <param name="holder">ボールを持っている人の能力値</param>
        /// <param name="tryer">ボールを奪う人の能力値</param>
        /// <param name="self">ボール奪う人</param>
        private void Steal(GameObject self, PersonalStatus holder = default, PersonalStatus tryer = default)
        {
            if (SuccessSteal(holder, tryer))
            {

            }
        }

        private bool SuccessSteal(PersonalStatus holder, PersonalStatus tryer)
        {
            //TODO: 盗む判定
            return true;
        }


        /// <summary>
        /// トラップするとき呼ぶ関数
        /// </summary>
        /// <param name="self">能力値</param>
        private void Trap(TrapCommand command)
        {
            if (SuccessTrap(command.m_status))
            {
                rb.velocity = Vector3.zero;
            }
        }

        /// <summary>
        /// トラップが成功するかの判定
        /// </summary>
        /// <param name="self">能力値</param>
        /// <returns>成功のときtrue</returns>
        private bool SuccessTrap(PersonalStatus self)
        {
            //ToDo: トラップ判定
            return true;
        }

        /// <summary>
        /// パスをするとき呼ぶ関数
        /// </summary>
        private void Pass(PassCommand command)
        {
            if (gameObject.transform.position.y < 1 && (command.m_sender - gameObject.ToVector2Int()).sqrMagnitude < 4 && rb.velocity.sqrMagnitude < 25)
            {

                if (command.m_status == default)
                {
                    command.m_status.power = 30;
                }

                switch (command.m_passHeight)
                {
                    case PassHeight.Low: CalcLowPass(command.m_sender, command.m_recever, command.m_status); break;
                    case PassHeight.Middle: CalcMiddlePass(command.m_sender, command.m_recever, command.m_status); break;
                    case PassHeight.High: CalcHighPass(command.m_sender, command.m_recever, command.m_status); break;
                }
            }
        }

        private void CalcLowPass(Vector2 sender, GameObject recever, PersonalStatus self)
        {
            var recevePos = recever.ToVector2Int();
            Vector2 dest = (recevePos - sender).normalized;
            var t = (recevePos - sender).magnitude / self.power;
            var passInfo = new PassInfo(recever, recevePos, rb, DateTime.Now, DateTime.Now.AddSeconds(t));
            passStream.OnNext(passInfo);
            rb.AddForce(self.power * dest, ForceMode.Impulse);
        }

        private void CalcMiddlePass(Vector2 sender, GameObject recever, PersonalStatus self)
        {
            var recevePos = recever.ToVector2Int();
            Vector2 dest = recevePos - sender;
            float distance = dest.magnitude;
            float power = Mathf.Sqrt(3 * gravity * distance) / 2;
            if (power > self.power)
            {
                power = self.power;
            }
            dest = dest.normalized;
            var t = distance / (power / 2.0);
            var passInfo = new PassInfo(recever, recevePos, rb, DateTime.Now, DateTime.Now.AddSeconds(t));
            passStream.OnNext(passInfo);
            rb.AddForce(power * new Vector3(2 / sqrt3 * dest.x, 1.0f / sqrt3, 2 / sqrt3 * dest.y), ForceMode.Impulse);
        }

        private void CalcHighPass(Vector2 sender, GameObject recever, PersonalStatus self)
        {
            var recevePos = recever.ToVector2Int();
            Vector3 dest = recevePos - sender;
            float distance = dest.magnitude;
            float power = Mathf.Sqrt(gravity * distance);
            if (power > self.power)
            {
                power = self.power;
            }
            dest = dest.normalized;
            var t = distance / (power / sqrt2);
            var passInfo = new PassInfo(recever, recevePos, rb, DateTime.Now, DateTime.Now.AddSeconds(t));
            passStream.OnNext(passInfo);
            rb.AddForce(power * new Vector3(dest.x / sqrt2, 1.0f / sqrt2, dest.z / sqrt2), ForceMode.Impulse);
        }

        /// <summary>
        /// シュートを撃つ関数
        /// </summary>
        private void Shoot(ShootCommand command)
        {
            if (rb.velocity.sqrMagnitude < 25)
            {
                Vector3 dest;
                if (command.m_status.ally)
                {
                    dest = (info.AdaptPosition(Constants.OppornentGoalPoint + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.0f, 2.5f), 0)) - command.m_sender.position).normalized;
                }
                else
                {
                    dest = (info.AdaptPosition(Constants.OurGoalPoint + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(0.0f, 2.5f), 0)) - command.m_sender.position).normalized;
                }

                rb.AddForce(Mathf.Min(10.0f, command.m_status.power) * dest, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// 特殊なイベント発生
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Goal"))
            {
                if (System.Threading.Interlocked.Increment(ref calledNum) != 1) return;
                var isTeammateGoal = transform.position.z > (Constants.OppornentGoalPoint.z + Constants.OurGoalPoint.z) / 2;
                scoreManager.Goal(isTeammateGoal);
            }
            else if (other.gameObject.CompareTag("OutBall"))
            {
                var vel = rb.velocity;
                if (transform.position.x <= 1 || transform.position.x >= 49)
                {
                    vel.x *= -1;
                }
                if (transform.position.z <= 1 || transform.position.z >= 99)
                {
                    vel.z *= -1;
                }
                rb.velocity = vel;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Goal"))
            {
                if (System.Threading.Interlocked.Increment(ref calledNum) != 1) return;
                if (isPause) return;
                var isTeammateGoal = transform.position.z > (Constants.OppornentGoalPoint.z + Constants.OurGoalPoint.z) / 2;
                scoreManager.Goal(isTeammateGoal);
            }
            else if (other.gameObject.CompareTag("OutBall"))
            {
                var vel = rb.velocity;
                if (transform.position.x <= 1 || transform.position.x >= 49)
                {
                    vel.x *= -1;
                }
                if (transform.position.z <= 1 || transform.position.z >= 99)
                {
                    vel.z *= -1;
                }
                rb.velocity = vel;
            }
        }
    }
}