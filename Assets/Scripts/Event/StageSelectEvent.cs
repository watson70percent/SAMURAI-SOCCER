using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    public static class StageSelectEvent
    {
        private static Subject<Stage> m_stage = new Subject<Stage>();
        private static IObservable<Stage> m_stageShareObservable = m_stage.Share();
        /// <summary>
        /// Stage�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<Stage> Stage
        {
            get { return m_stageShareObservable; }
        }
        /// <summary>
        /// Stage�C�x���g�𔭍s
        /// </summary>
        /// <param name="stage">�X�e�[�W(����)</param>
        public static void StageOnNext(Stage stage)
        {
            m_stage.OnNext(stage);
        }

        private static Subject<int> m_preview = new Subject<int>();
        private static IObservable<int> m_previewShareObservable = m_preview.Share();
        /// <summary>
        /// Preview�C�x���g�𔭍s
        /// </summary>
        public static IObservable<int> Preview
        {
            get { return m_previewShareObservable; }
        }
        /// <summary>
        /// Preview�C�x���g��Subscribe��
        /// </summary>
        /// <param name="number">�X�e�[�W�ԍ�</param>
        public static void PreviewOnNext(int number)
        {
            m_preview.OnNext(number);
        }
    }

    /// <summary>
    /// �X�e�[�W(����or�S��)
    /// </summary>
    public enum Stage
    {
        World,
        UK,
        China,
        USA,
        Russian,
        Japan
    }
}
