using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadeInStatue : MonoBehaviour
{
    [SerializeField]
    private RiseStatue _riseStatue;
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private float time = 0f;

    private void Start()
    {
        StartCoroutineForPlayingState.AddTaskIEnumrator(ShadeStatueCoroutine());
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0.1f, gameObject.transform.position.z);
    }

    IEnumerator ShadeStatueCoroutine()
    {        
        while (_riseStatue.CurrentStatueMode == StatueMode.Rise)
        {
            time += Time.deltaTime;
            yield return 0.1f;
            try
            {
                _spriteRenderer.color = new Color(1f, 1f, 1f, 0.4f*(1f+Mathf.Sin(10*time*time)));
            }
            catch
            {
                yield break;
            }
        }
        while (_riseStatue.CurrentStatueMode == StatueMode.FallDown)
        {
            _spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
        }
    }
}
