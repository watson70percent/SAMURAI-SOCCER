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

    private float _fireDuration = 10.0f;//弾を撃つ間隔
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

        //弾を撃つ間隔を指定
        _fireDuration = Random.Range(6.0f,10.0f);
        StartCoroutine(FireBulletSycle());
    }

    /// <summary>
    /// 一定時間ごとに弾を撃つ
    /// </summary>
    /// <returns></returns>
    IEnumerator FireBulletSycle()
    {
        yield return new WaitForSeconds(_fireDuration+10f);
        while (_gameManager.CurrentGameState != GameState.Finish)
        {
            //弾を撃つ間隔を再指定
            _fireDuration = Random.Range(6.0f, 10.0f);
            if (_gameManager.CurrentGameState == GameState.Playing)
            {
                //侍との相対座標
                Vector3 firedirection = _Samurai.transform.position - gameObject.transform.position;
                //敵選手が向く方向を計算し、反映
                Quaternion firelotation = Quaternion.LookRotation(firedirection);
                gameObject.transform.localRotation = firelotation;
                //弾を生成し、発射
                GameObject Bullet = Instantiate(HypnoticBullets);
                Bullet.GetComponent<Rigidbody>().velocity = gameObject.transform.forward * 10;
                Bullet.transform.position = gameObject.transform.position;
            }
            yield return new WaitForSeconds(_fireDuration);
        }
    }
}
