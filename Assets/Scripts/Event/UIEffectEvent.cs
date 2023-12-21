using UniRx;
using System;

namespace SamuraiSoccer.Event
{
    public static class UIEffectEvent
    {
        private static Subject<float> m_blackOutSubject = new Subject<float>();
        private static IObservable<float> m_blackOutShareObservable = m_blackOutSubject.Share();

        /// <summary>
        /// UI�Ó]�C�x���g��Subscribe��
        /// </summary>
        public static IObservable<float> BlackOut
        {
            get { return m_blackOutShareObservable; }
        }

        /// <summary>
        /// UI�Ó]�C�x���g�𔭍s
        /// </summary>
        /// <param name="totalsec">�Ó]�̑S�̎���</param>
        public static void BlackOutOnNext(float totalsec)
        {
            m_blackOutSubject.OnNext(totalsec);
        }
    }
}
