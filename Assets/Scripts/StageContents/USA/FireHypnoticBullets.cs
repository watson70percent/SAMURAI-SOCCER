using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.USA
{
    /// <summary>
    /// 敵選手に付けて弾を撃たせる
    /// </summary>
    public class FireHypnoticBullets : MonoBehaviour
    {
        [SerializeField]
        private GameObject HypnoticBullets; //撃つ催眠弾

        [SerializeField]
        private float m_fireDuration; //弾を撃つ間隔(初期値はInspectorで指定)

        [SerializeField]
        private GameObject m_samurai; //侍のオブジェクト

        // Start is called before the first frame update
        void Start()
        {
            //一定間隔で弾丸を発射
            InGameEvent.UpdateDuringPlay.ThrottleFirst(System.TimeSpan.FromSeconds(m_fireDuration)).Subscribe(_ =>
            {
                FireBullet();
            }).AddTo(this);
        }


        /// <summary>
        /// 弾を撃つ、次の発射間隔をランダム指定
        /// </summary>
        /// <returns></returns>
        void FireBullet()
        {
            //弾を撃つ間隔を再指定
            m_fireDuration = Random.Range(15.0f, 15.5f);
            //侍との相対座標
            Vector3 firedirection = (m_samurai.transform.position - gameObject.transform.position).normalized;
            firedirection.y = 0f;
            //敵選手が向く方向を計算し、反映
            Quaternion firelotation = Quaternion.LookRotation(firedirection);
            gameObject.transform.localRotation = firelotation;
            //弾を生成し、発射
            GameObject Bullet = Instantiate(HypnoticBullets);
            Bullet.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 8;
            Bullet.transform.position = gameObject.transform.position + gameObject.transform.forward * 3f + new Vector3(0f, -0.5f, 0f);
        }
    }

}
