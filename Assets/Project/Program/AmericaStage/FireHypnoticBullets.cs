using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵選手に付けて弾を撃たせる
/// </summary>
public class FireHypnoticBullets : MonoBehaviour
{
    [SerializeField]
    private GameObject HypnoticBullets;//撃つ催眠弾

    [SerializeField]
    private float _fireDuration;//弾を撃つ間隔(初期値はInspectorで指定)
    private GameManager _gameManager;//Scene内のGameManagerクラス
    private GameObject _Samurai;//侍のオブジェクト

    // Start is called before the first frame update
    void Start()
    {
        //GameManagerなかったらエラーなのでこの先は動かさない。
        if (!GameObject.FindGameObjectWithTag("GameManager"))
        {
            Debug.LogError("GameManagerが存在しません");
        }
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        _Samurai = GameObject.FindGameObjectWithTag("Player");

        StartCoroutineForPlayingState.AddTaskIEnumrator(FireBulletSycle());
    }

    /// <summary>
    /// 一定時間ごとに弾を撃つ
    /// </summary>
    /// <returns></returns>
    IEnumerator FireBulletSycle()
    {
        yield return _fireDuration+4f;
        while (_gameManager.CurrentGameState != GameState.Finish)
        {
            //弾を撃つ間隔を再指定
            _fireDuration = Random.Range(15.0f, 15.5f);
            if (_gameManager.CurrentGameState == GameState.Playing)
            {
                //侍との相対座標
                Vector3 firedirection = (_Samurai.transform.position - gameObject.transform.position).normalized;
                firedirection.y = 0f;
                //敵選手が向く方向を計算し、反映
                Quaternion firelotation = Quaternion.LookRotation(firedirection);
                gameObject.transform.localRotation = firelotation;
                //弾を生成し、発射
                GameObject Bullet = Instantiate(HypnoticBullets);
                Bullet.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 8;
                Bullet.transform.position = gameObject.transform.position + gameObject.transform.forward *3f + new Vector3(0f,-0.5f,0f);
            }
            yield return _fireDuration;
        }
    }
}
