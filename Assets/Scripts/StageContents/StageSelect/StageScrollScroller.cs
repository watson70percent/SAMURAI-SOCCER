using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer.Event;
using UniRx;

namespace SamuraiSoccer.StageContents.StageSelect
{
    public class StageScrollScroller : ScrollRect
    {
        int m_pageNumber; //全ページ数

        int m_slideCount = 0; //どれだけスクロールしたかを表す数値

        bool m_isDrugging = false;

        //スライダー調整をキャンセルするトークン
        CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();


        private static ReactiveProperty<Stage> m_selectingStageReactiveProperty = new ReactiveProperty<Stage>(Stage.Japan);

        /// <summary>
        /// 選択中のStageが変更されたときに発行するReactiveProperty
        /// </summary>
        public static IReadOnlyReactiveProperty<Stage> SelectedStage
        {
            get { return m_selectingStageReactiveProperty; }
        }


        /// <summary>
        /// スライダーの中心にある国を取得
        /// </summary>
        /// <returns></returns>
        private Stage GetSelectingStage()
        {
            int centerStageNum = (int)Mathf.Round(horizontalNormalizedPosition * (m_pageNumber-1));
            GameObject centerChild = content.GetChild(centerStageNum).gameObject;

            return centerChild.GetComponent<StageScroll>().Stage;
        }


        private void Update()
        {
            m_selectingStageReactiveProperty.Value = GetSelectingStage();
        }




        protected override void Start()
        {
            m_pageNumber = content.childCount;
            horizontalNormalizedPosition = 0.5f;
            setScrollPos();

            

        }


        private void setScrollPos()
        {
            //現在のクリア状況から中央に配置する巻物を判断
            InFileTransmitClient<SaveData> stageNumberTransitionClient = new InFileTransmitClient<SaveData>();
            int stageNumber = stageNumberTransitionClient.Get(StorageKey.KEY_STAGENUMBER).m_stageData;

            Stage stage = Stage.Japan;

            switch (stageNumber / 3)
            {
                case 0:stage = Stage.UK;break;
                case 1:stage = Stage.China; break;
                case 2: stage = Stage.USA; break;
                case 3: stage = Stage.Russian; break;
                default:break;
            }

            

            int count = 0;

            while(GetSelectingStage() != stage) //巻物の順番を入れ替えていって目的の位置に来たらループを抜ける
            {
                content.GetChild(0).SetSiblingIndex(100);

                if (count > 100) break;//バグか何かでうまくいかないときはとりあえずループを抜ける
                count++;
            }

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
            ScrollRoop();
        }


        void ScrollRoop()
        {
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


        async UniTask SliderAdjust( CancellationToken cancellationToken)
        {
            //スライダーから手を離したら一定時間かけてキリのいい位置へ移動する
            //移動途中にもう一度スライダーに触ったら中断


            //一番近いページを取得
            float targetPos = (Mathf.Round(horizontalNormalizedPosition * (m_pageNumber - 1))) / (m_pageNumber - 1);

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

            


            m_cancellationTokenSource = new CancellationTokenSource();
            SliderAdjust( m_cancellationTokenSource.Token);

        }
    }
}
