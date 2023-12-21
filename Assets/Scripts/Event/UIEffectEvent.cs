using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    public static class UIEffectEvent
    {
        private static Subject<float> m_blackOutSubject = new Subject<float>();
        private static IObservable<float> m_blackOutShareObservable = m_blackOutSubject.Share();

        /// <summary>
        /// UI暗転イベントのSubscribe先
        /// </summary>
        public static IObservable<float> BlackOut
        {
            get { return m_blackOutShareObservable; }
        }

        /// <summary>
        /// UI暗転イベントを発行
        /// </summary>
        /// <param name="totalsec">暗転の全体時間</param>
        public static void BlackOutOnNext(float totalsec)
        {
            m_blackOutSubject.OnNext(totalsec);
        }
    }
}
