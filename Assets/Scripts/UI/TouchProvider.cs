using UnityEngine;
using UniRx;

namespace SamuraiSoccer.UI
{
    public class TouchProvider : MonoBehaviour
    {
        private ReactiveProperty<bool> m_reactiveProperty;
        private int m_prevTouchCount;

        public IReadOnlyReactiveProperty<bool> IsTouchingReactiveProperty => m_reactiveProperty;

        private void Awake()
        {
            m_reactiveProperty = new ReactiveProperty<bool>(Input.touchCount > 0);
        }

        // Update is called once per frame
        void Update()
        {
            m_reactiveProperty.Value = Input.touchCount > m_prevTouchCount;
            m_prevTouchCount = Input.touchCount;
        }
    }
}
