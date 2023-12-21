using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SamuraiSoccer.UI
{
    public class TouchProvider : MonoBehaviour
    {
        private ReactiveProperty<bool> m_reactiveProperty;

        public IReadOnlyReactiveProperty<bool> IsTouchingReactiveProperty => m_reactiveProperty;

        private void Awake()
        {
            m_reactiveProperty = new ReactiveProperty<bool>(Input.touchCount > 0);
        }

        // Update is called once per frame
        void Update()
        {
            m_reactiveProperty.Value = Input.touchCount > 0;
        }
    }
}
