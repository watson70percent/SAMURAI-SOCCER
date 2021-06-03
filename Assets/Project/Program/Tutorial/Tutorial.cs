using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameManager))]
public class Tutorial : MonoBehaviour
{

    private GameManager gameManager;
    public GameObject samurai;
    public GameObject ball;
    public GameObject firstYellowCard;
    public GameObject enemy;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        StartCoroutine(Runner());
    }

    IEnumerator Runner()
    {
        gameManager.StateChangeSignal(GameState.Pause);
        /* TODO : UIを突っ込む*/
        yield return new WaitForSeconds(5f);
        Vector3 destination = new Vector3(10f, 3f, 30f);
        var enemyPrefab = Instantiate(enemy,destination,Quaternion.identity);
        /* TODO : UIを突っ込む*/
        /* TODO : カメラを生成された選手に向ける*/
        yield return new WaitForSeconds(5f);
        gameManager.StateChangeSignal(GameState.Playing);
        //敵に一定距離近づくまで待機
        while ((samurai.transform.position-destination).sqrMagnitude > 100)
        {
            yield return null;
        }
        gameManager.StateChangeSignal(GameState.Standby);
        //UIを突っ込む
        Debug.Log("たどり着いた!");
        yield return new WaitForSeconds(5f);
        gameManager.StateChangeSignal(GameState.Playing);
        //敵を切り倒して行って距離移動するまで待機
        while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400)
        {
            yield return null;
        }
        //UIを突っ込む
        Debug.Log("斬れた!");
        yield return new WaitForSeconds(5f);
        //ボールの位置を移動する
        destination = new Vector3(50f, 3f, 60f);
        ball.transform.position = destination;
        Instantiate(enemy, destination, Quaternion.identity);
        /* TODO : カメラを生成された選手に向ける*/
        yield return new WaitForSeconds(5f);
        gameManager.StateChangeSignal(GameState.Playing);
        //イエローカードが出るまで待機
        while (firstYellowCard.activeSelf)
        {
            yield return null;
            break;
        }
        //UIを突っ込む
        Debug.Log("審判に見つかった！");
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("StageSelect");
    }
}
