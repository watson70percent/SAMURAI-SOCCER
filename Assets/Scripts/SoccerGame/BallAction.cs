using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

using UniRx;

using SamuraiSoccer.Event;

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
        private static readonly float sqrt3 = Mathf.Sqrt(3);
        private static readonly float sqrt2 = Mathf.Sqrt(2);
        private static readonly float gravity = 9.8f;

        private Vector3 velocity;
        private Vector3 angularVelocity;

        private static Subject<BallActionCommand> commandStream = new();

        [NonSerialized]
        public bool last_touch;
        [NonSerialized]
        public GameObject owner;

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
            InGameEvent.Play.Subscribe(Play);
            InGameEvent.Pause.Subscribe(Pause);
            commandStream.ThrottleFirst(TimeSpan.FromSeconds(0.1)).Subscribe(Command);
        }

        private void Update()
        {
            rb.angularDrag = info.info.Getdrag(gameObject.transform.position);
        }

        private void Pause(bool isPause)
        {
            if (isPause)
            {
                velocity = rb.velocity;
                rb.velocity = Vector3.zero;
                angularVelocity = rb.angularVelocity;
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                Play(new Unit());
            }
        }

        private void Play(Unit _)
        {
            rb.velocity = velocity;
            rb.angularVelocity = angularVelocity;
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
        public IEnumerator Dribble(DribbleCommand c)
        {
            last_touch = c.m_status.ally;
            rb.AddForce(new Vector3(Mathf.Sin(owner.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * 4, 0, Mathf.Cos(owner.transform.rotation.eulerAngles.y * Mathf.Deg2Rad) * 4), ForceMode.Impulse);
            yield return null;
            for (int i = 0; i < 4; i++)
            {
                yield return null;
            }
            Vector3 pos = transform.position;
            for (int i = 0; i < 5; i++)
            {
                pos.x = Mathf.Lerp(pos.x, owner.transform.position.x, i / 20.0f);
                pos.z = Mathf.Lerp(pos.z, owner.transform.position.z, i / 20.0f);
                transform.position = pos;

                yield return null;
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

        private void CalcLowPass(Vector2 sender, Vector2 recever, PersonalStatus self)
        {
            Vector2 dest = (recever - sender).normalized;

            rb.AddForce(self.power * dest, ForceMode.Impulse);
        }

        private void CalcMiddlePass(Vector2 sender, Vector2 recever, PersonalStatus self)
        {
            Vector2 dest = (recever - sender);
            float distance = dest.magnitude;
            float power = Mathf.Sqrt(3 * gravity * distance) / 2;
            if (power > self.power)
            {
                power = self.power;
            }
            dest = dest.normalized;
            rb.AddForce(power * new Vector3(2 / sqrt3 * dest.x, 1.0f / sqrt3, 2 / sqrt3 * dest.y), ForceMode.Impulse);
        }

        private void CalcHighPass(Vector2 sender, Vector2 recever, PersonalStatus self)
        {
            Vector3 dest = recever - sender;
            float distance = dest.magnitude;
            float power = Mathf.Sqrt(gravity * distance);
            if (power > self.power)
            {
                power = self.power;
            }
            dest = dest.normalized;
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

                    dest = (info.AdaptPosition(Constants.OppornentGoalPoint + new Vector3(Random.Range(-10, 10), Random.Range(0.0f, 2.0f), 0)) - command.m_sender.position).normalized;
                }
                else
                {
                    dest = (info.AdaptPosition(Constants.OurGoalPoint + new Vector3(Random.Range(-10, 10), Random.Range(0.0f, 2.0f), 0)) - command.m_sender.position).normalized;

                }

                rb.AddForce(command.m_status.power * dest, ForceMode.Impulse);
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
                InGameEvent.GoalOnNext();
            }
            else if (other.gameObject.CompareTag("OutBall"))
            {

            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Goal"))
            {
                InGameEvent.GoalOnNext();
            }
            else if (other.gameObject.CompareTag("OutBall"))
            {

            }
        }
    }
}