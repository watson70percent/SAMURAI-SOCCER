using UnityEngine;

namespace SamuraiSoccer.UI
{
    public class ScrollHighlight : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] m_highlightObjects;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                foreach (GameObject obj in m_highlightObjects)
                {
                    obj.SetActive(false);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                foreach (GameObject obj in m_highlightObjects)
                {
                    obj.SetActive(true);
                }
            }
        }
    }
}
