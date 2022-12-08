using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// 巻物をなめらかに移動させる
    /// </summary>
    public class ScrollScript : MonoBehaviour
    {
        [SerializeField] RectTransform rectra;//動かす巻物のRectTransform
        [SerializeField] float slideTime;//移動にかける時間
        [SerializeField] GameObject ScrollObject;//巻物の3Dオブジェクト
        [SerializeField] float rotSpeed;//回転速度
        [SerializeField] float startX, goalX;//巻物の初期位置と最終位置のX座標


        /// <summary>
        /// 巻物を初期位置から線対称の位置に移動させる
        /// </summary>
        /// <returns></returns>
        public async UniTask ScrollSlide()
        {
            float elapsedTime = 0;
            float y = rectra.anchoredPosition.y;
            float rotSign;//回転の向き
            if (startX > goalX)
            {
                rotSign = -1;
            }
            else
            {
                rotSign = 1;
            };
            while (elapsedTime < slideTime)
            {
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalX, startX, slideTime);
                rectra.anchoredPosition = new Vector2(x, y);
                ScrollObject.transform.eulerAngles = new Vector3(0, rotSign * x * rotSpeed, 0);
                await UniTask.Delay(1);
                Debug.Log(elapsedTime);
            }
        }

        /// <summary>
        /// イージングの計算
        /// </summary>
        /// <param name="t">現在の時刻</param>
        /// <param name="goal">最終地点のx座標</param>
        /// <param name="start">t=0のx座標</param>
        /// <param name="goalTime">最終地点の時刻</param>
        /// <returns>現在のx座標</returns>
        float easeOutCubic(float t, float goal, float start, float goalTime)
        {
            return (goal - start) * (1 - Mathf.Pow(goalTime - t, 3)) + start;
        }
    }
}
