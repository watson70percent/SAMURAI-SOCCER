using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.Player
{
    //攻撃イベント発生時にSlashプレハブを設置
    public class PlayerAttack : MonoBehaviour
    {
        public GameObject slash;
        // Start is called before the first frame update
        void Start()
        {
            PlayerEvent.Attack.Subscribe(x => { Attack(); }).AddTo(this);
        }


        void Attack()
        {
            Instantiate(slash, transform.position, transform.rotation);
            PlayerEvent.FaulCheckOnNext();//審判によるファールチェック
        }
    }
}
