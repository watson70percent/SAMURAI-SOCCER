using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    public static class StageSelectEvent
    {
        private static Subject<Unit> m_world = new Subject<Unit>();
        private static IObservable<Unit> m_worldShareObservable = m_world.Share();
        /// <summary>
        /// WorldイベントのSubcribe先
        /// </summary>
        public static IObservable<Unit> World
        {
            get { return m_worldShareObservable; }
        }
        /// <summary>
        /// Worldのイベントを発行
        /// </summary>
        public static void WorldOnNext()
        {
            m_world.OnNext(Unit.Default);
        }

        private static Subject<Stage> m_stage = new Subject<Stage>();
        private static IObservable<Stage> m_stageShareObservable = m_stage.Share();
        /// <summary>
        /// StageイベントのSubscribe先
        /// </summary>
        public static IObservable<Stage> Stage
        {
            get { return m_stageShareObservable; }
        }
        /// <summary>
        /// Stageイベントを発行
        /// </summary>
        /// <param name="stage">ステージ(国名)</param>
        public static void StageOnNext(Stage stage)
        {
            m_stage.OnNext(stage);
        }

        private static Subject<int> m_preview = new Subject<int>();
        private static IObservable<int> m_previewShareObservable = m_preview.Share();
        /// <summary>
        /// PreviewイベントのSubscribe先
        /// </summary>
        public static IObservable<int> Preview
        {
            get { return m_previewShareObservable; }
        }
        /// <summary>
        /// PreviewイベントのSubscribe先
        /// </summary>
        /// <param name="number">ステージ番号</param>
        public static void PreviewOnNext(int number)
        {
            m_preview.OnNext(number);
        }
    }

    /// <summary>
    /// ステージ(国名)
    /// </summary>
    public enum Stage
    {
        UK,
        China,
        USA,
        Rossia
    }
}
