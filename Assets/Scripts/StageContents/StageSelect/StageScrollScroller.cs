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
        int m_pageNumber; //�S�y�[�W��

        int m_slideCount = 0; //�ǂꂾ���X�N���[����������\�����l

        bool m_isDrugging = false;

        //�X���C�_�[�������L�����Z������g�[�N��
        CancellationTokenSource m_cancellationTokenSource = new CancellationTokenSource();


        private static ReactiveProperty<Stage> m_selectingStageReactiveProperty = new ReactiveProperty<Stage>(Stage.Japan);

        /// <summary>
        /// �I�𒆂�Stage���ύX���ꂽ�Ƃ��ɔ��s����ReactiveProperty
        /// </summary>
        public static IReadOnlyReactiveProperty<Stage> SelectedStage
        {
            get { return m_selectingStageReactiveProperty; }
        }


        /// <summary>
        /// �X���C�_�[�̒��S�ɂ��鍑���擾
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
            //���݂̃N���A�󋵂��璆���ɔz�u���銪���𔻒f
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

            while(GetSelectingStage() != stage) //�����̏��Ԃ����ւ��Ă����ĖړI�̈ʒu�ɗ����烋�[�v�𔲂���
            {
                content.GetChild(0).SetSiblingIndex(100);

                if (count > 100) break;//�o�O�������ł��܂������Ȃ��Ƃ��͂Ƃ肠�������[�v�𔲂���
                count++;
            }

        }


        public override void OnBeginDrag(PointerEventData data)
        {
            base.OnBeginDrag(data);
            m_isDrugging = true;
            //�X���C�_�[�������Ȃ�L�����Z��
            if (m_cancellationTokenSource.Token.CanBeCanceled) m_cancellationTokenSource.Cancel();

        }



        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            ScrollRoop();
        }


        void ScrollRoop()
        {
            //���ȏ�X���C�h������ƒ[�����̃I�u�W�F�N�g����������̒[�ֈڂ�(�q�G�����L�[�̏��Ԃ�ς���)
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
            //�q�G�����L�[�̏��Ԃ�ς��邾�����ƃX���C�_�[�̈ʒu���ς���Ă��܂��̂Œ���
            horizontalNormalizedPosition -= m_slideCount * 1.0f / (m_pageNumber - 1);
        }


        async UniTask SliderAdjust( CancellationToken cancellationToken)
        {
            //�X���C�_�[�����𗣂������莞�Ԃ����ăL���̂����ʒu�ֈړ�����
            //�ړ��r���ɂ�����x�X���C�_�[�ɐG�����璆�f


            //��ԋ߂��y�[�W���擾
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
