using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

[RequireComponent(typeof(GameManager))]
public class Tutorial : MonoBehaviour
{

    private GameManager gameManager;
    public GameObject samurai;
    public GameObject ball;
    public GameObject firstYellowCard;
    public GameObject enemy;
    public Text tutorialText;
    public Animator animator;
    public CinemachineVirtualCamera spotCamera; //何か焦点を当てるためのカメラ
    public CinemachineVirtualCamera samuraiCamera; //侍を追尾するためのカメラ

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GetComponent<GameManager>();
        StartCoroutine(Runner());
    }

    IEnumerator Runner()
    {
        gameManager.StateChangeSignal(GameState.Pause);
        //テキスト表示1
        yield return new WaitForSeconds(5f);
        tutorialText.text = "ここではこの世界で戦うちゅーとりあるを行う";
        yield return new WaitForSeconds(3f); 
        tutorialText.text = "貴殿は残された日本の希望だ";
        yield return new WaitForSeconds(3f);
        Vector3 destination = new Vector3(10f, 3f, 30f);
        var enemyPrefab = Instantiate(enemy,destination,Quaternion.identity);
        /* TODO : UIを突っ込む*/
        //カメラを生成された選手に向ける
        spotCamera.Follow = enemyPrefab.transform;
        spotCamera.LookAt = enemyPrefab.transform;
        spotCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        tutorialText.text = "まずはそこまで行こう";
        yield return new WaitForSeconds(3f);
        //カメラをもとに戻す
        spotCamera.Priority = 9;
        samuraiCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        animator.SetTrigger("ReturnText");
        gameManager.StateChangeSignal(GameState.Playing);
        //敵に一定距離近づくまで待機
        while ((samurai.transform.position-destination).sqrMagnitude > 100)
        {
            yield return null;
        }
        gameManager.StateChangeSignal(GameState.Standby);
        //テキスト表示2
        tutorialText.text = "貴殿が行うのはただ斬ることのみ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "試しにこいつをひとおもいに斬れ";
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        animator.SetTrigger("ReturnText");
        gameManager.StateChangeSignal(GameState.Playing);
        //敵を切り倒して行って距離移動するまで待機
        while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400)
        {
            yield return null;
        }
        //テキスト表示2
        tutorialText.text = "よし、それでいい";
        yield return new WaitForSeconds(3f);
        //ボールの位置を移動する
        destination = new Vector3(50f, 3f, 60f);
        ball.transform.position = destination;
        enemyPrefab = Instantiate(enemy, destination, Quaternion.identity);
        //カメラを生成された選手に向ける
        samuraiCamera.Priority = 9;
        spotCamera.Follow = enemyPrefab.transform;
        spotCamera.LookAt = enemyPrefab.transform;
        spotCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        tutorialText.text = "次はこいつを斬れ";
        yield return new WaitForSeconds(3f);
        //カメラをもとに戻す
        spotCamera.Priority = 9;
        samuraiCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        animator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        animator.SetTrigger("ReturnText");
        gameManager.StateChangeSignal(GameState.Playing);
        //イエローカードが出るまで待機
        while (!firstYellowCard.activeSelf)
        {
            Debug.Log(firstYellowCard.activeSelf);
            yield return null;
            break;
        }
        //テキスト表示3
        tutorialText.text = "しまった審判に見られてしまった";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "もう一度見つかると退場になってしまう、気をつけろ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "最後に確認する";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "これは残りの敵数だ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "0になれば貴方の勝ちだ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "これは残り時間だ";
        yield return new WaitForSeconds(2f);
        tutorialText.text = "無くなるまでにクリアせよ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "それではちゅーとりあるを終了する";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "必ず日本に勝利を持ち帰れ";
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("StageSelect");
    }
}
