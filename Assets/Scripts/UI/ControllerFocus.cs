using UnityEngine;

namespace SamuraiSoccer.UI
{
    public class ControllerFocus : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_controller;

        private void Start()
        {
            transform.position = m_controller.transform.position;
        }
    }
}
