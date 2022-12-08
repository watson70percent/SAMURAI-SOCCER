using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageScrollScroller : ScrollRect
    {
        int m_pageNumber; //全ページ数

        int m_slideCount = 0; //どれだけスクロールしたかを表す数値

        bool m_isDrugging = false;

        //スライダー調整をキャンセルするトークン
        CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// スライダーの中心にある国を取得
        /// </summary>
        /// <returns></returns>
        public Stage GetSelectingStage()
        {
            int centerStageNum = (int)Mathf.Floor(horizontalNormalizedPosition * m_pageNumber);
            GameObject centerChild = content.GetChild(centerStageNum).gameObject;

            return centerChild.GetComponent<StageScroll>().Stage;
        }




        protected override void Start()
        {
            m_pageNumber = content.childCount;
            horizontalNormalizedPosition = 0.5f; //最初はスライダーを真ん中に
        }


        public override void OnBeginDrag(PointerEventData data)
        {
            base.OnBeginDrag(data);
            m_isDrugging = true;
            //スライダー調整中ならキャンセル
            if (m_cancellationTokenSource.Token.CanBeCanceled) m_cancellationTokenSource.Cancel();

        }



        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

            //一定以上スライドさせると端っこのオブジェクトをもう一方の端へ移す(ヒエラルキーの順番を変える)
            if (horizontalNormalizedPosition > 0.7f + m_slideCount * 1.0f / (m_pageNumber - 1))
            {
                m_slideCount += 1;
                content.GetChild(0).SetSiblingIndex(100);

            }
            if (horizontalNormalizedPosition < 0.3f + m_slideCount * 1.0f / (m_pageNumber - 1))
            {
                m_slideCount -= 1;
                content.GetChild(content.childCount - 1).SetSiblingIndex(0);

            }
            //ヒエラルキーの順番を変えるだけだとスライダーの位置が変わってしまうので調整
            horizontalNormalizedPosition -= m_slideCount * 1.0f / (m_pageNumber - 1);

        }


        async UniTask SliderAdjust(float targetPos, CancellationToken cancellationToken)
        {
            //スライダーから手を離したら一定時間かけてキリのいい位置へ移動する
            //移動途中にもう一度スライダーに触ったら中断
            while (!cancellationToken.IsCancellationRequested)
            {
                float nowPos = horizontalNormalizedPosition;
                float max_step = 0.01f;
                float diff = targetPos - nowPos;

                if (Mathf.Abs(diff) < max_step)
                {
                    horizontalNormalizedPosition = targetPos;
                    break;
                }
                else
                {
                    horizontalNormalizedPosition = nowPos + Mathf.Sign(diff) * max_step;
                }
                await UniTask.Delay(10);
            }
        }

        public override void OnEndDrag(PointerEventData data)
        {
            base.OnEndDrag(data);
            m_isDrugging = false;
            m_slideCount = 0;

            //一番近いページを取得
            float nextpage = (Mathf.Round(horizontalNormalizedPosition * (m_pageNumber - 1))) / (m_pageNumber - 1);


            m_cancellationTokenSource = new CancellationTokenSource();
            SliderAdjust(nextpage, m_cancellationTokenSource.Token);

        }
    }
}
