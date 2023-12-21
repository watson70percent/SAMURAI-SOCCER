using UnityEngine;
using UniRx;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.USA
{
    public class CallStatueOfLiberty : MonoBehaviour
    {
        [SerializeField]
        private GameObject StatuePrefab;//自由の女神プレハブ 

        private void Start()
        {
            //一定間隔で自由の女神を生成
            InGameEvent.UpdateDuringPlay.ThrottleFirst(System.TimeSpan.FromSeconds(6)).Subscribe(_ =>
            {
                Instantiate(StatuePrefab, new Vector3(60f, -70.0f, 50 + Random.Range(-40f, 40f)), Quaternion.identity);
            }).AddTo(this);
        }

    }
}

