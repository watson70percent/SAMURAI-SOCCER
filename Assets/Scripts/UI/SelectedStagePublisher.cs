using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;

namespace SamuraiSoccer.UI
{
    public class SelectedStagePublisher : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("�I�������X�e�[�W�̍����ǂ�����ݒ�")]
        private Stage m_stage;

        /// <summary>
        /// �I�������X�e�[�W�����Ƃɏ�ԑJ�ڂ𔭍s
        /// </summary>
        public void OnClick()
        {
            StageSelectEvent.StageOnNext(m_stage);
        }
    }
}
