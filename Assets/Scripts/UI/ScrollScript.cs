using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    public class ScrollScript : MonoBehaviour
    {
        [SerializeField] RectTransform rectra;
        [SerializeField] float slideTime;
        Vector3 rotVec;
        Vector2 startVec, goalVec;

        // Start is called before the first frame update
        void Start()
        {
            startVec = rectra.anchoredPosition;
            goalVec = new Vector2(-startVec.x, startVec.y);
        }

        public async UniTask ScrollSlide()
        {
            float elapsedTime = 0;
            while (elapsedTime < slideTime)
            {
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalVec.x, startVec.x, slideTime);
                rectra.anchoredPosition = new Vector2(x, startVec.y);
                await UniTask.Delay(1);
                Debug.Log(elapsedTime);
            }
        }

        float easeOutCubic(float t, float goal, float start, float goalTime)
        {
            return (goal - start) * (1 - Mathf.Pow(goalTime - t, 3)) + start;
        }
    }
}
