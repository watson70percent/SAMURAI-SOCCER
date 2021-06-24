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
    public Text enemyNumber;
    public Animator textAnimator;
    public Animator arrowAnimator;
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
        tutorialText.text = "サムライは残された日本の希望だ";
        yield return new WaitForSeconds(3f);
        Vector3 destination = new Vector3(10f, 3f, 30f);
        var enemyPrefab = Instantiate(enemy,destination,Quaternion.identity);
        //カメラを生成された選手に向ける
        spotCamera.Follow = enemyPrefab.transform;
        spotCamera.LookAt = enemyPrefab.transform;
        spotCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        tutorialText.text = "まずはここまで行こう";
        yield return new WaitForSeconds(3f);
        //カメラをもとに戻す
        spotCamera.Priority = 9;
        samuraiCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        textAnimator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        gameManager.StateChangeSignal(GameState.Playing);
        //敵に一定距離近づくまで待機
        while ((samurai.transform.position-destination).sqrMagnitude > 100)
        {
            yield return null;
        }
        gameManager.StateChangeSignal(GameState.Standby);
        //テキスト表示2
        textAnimator.SetTrigger("ReturnText");
        tutorialText.text = "サムライが行うのはただ斬ることのみ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "試しにこいつをひとおもいに斬れ";
        yield return new WaitForSeconds(3f);
        textAnimator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        gameManager.StateChangeSignal(GameState.Playing);
        //敵を切り倒して行って距離移動するまで待機
        while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400)
        {
            yield return null;
        }
        gameManager.StateChangeSignal(GameState.Pause);
        enemyNumber.text = 1.ToString();
        //テキスト表示2
        textAnimator.SetTrigger("ReturnText");
        tutorialText.text = "よし、それでいい";
        yield return new WaitForSeconds(3f);
        //ボールの位置を移動する
        destination = new Vector3(50f, 3f, 60f);
        ball.transform.position = destination;
        enemyPrefab = Instantiate(enemy, destination, Quaternion.identity);
        yield return new WaitForSeconds(1f);
        //カメラを生成された選手に向ける
        spotCamera.Follow = enemyPrefab.transform;
        spotCamera.LookAt = enemyPrefab.transform;
        spotCamera.Priority = 11;
        samuraiCamera.Priority = 9;
        yield return new WaitForSeconds(3f);
        tutorialText.text = "次はこいつを斬れ";
        yield return new WaitForSeconds(3f);
        //カメラをもとに戻す
        spotCamera.Priority = 9;
        samuraiCamera.Priority = 11;
        yield return new WaitForSeconds(3f);
        textAnimator.SetTrigger("SlideText");
        yield return new WaitForSeconds(2f);
        //テキストを非表示に
        tutorialText.text = "";
        gameManager.StateChangeSignal(GameState.Playing);
        //イエローカードが出るまで待機
        while (!firstYellowCard.activeSelf)
        {
            yield return null;
        }
        gameManager.StateChangeSignal(GameState.Pause);
        //テキスト表示3
        textAnimator.SetTrigger("ReturnText");
        tutorialText.text = "しまった！れふぇりーに見られてしまった";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "もう一度見つかると退場だ、気をつけろ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "最後に確認する";
        yield return new WaitForSeconds(3f);
        arrowAnimator.gameObject.SetActive(true);
        tutorialText.text = "これは残りの敵数だ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "0になれば日本の勝利だ";
        yield return new WaitForSeconds(3f);
        arrowAnimator.SetTrigger("MoveArrow");
        yield return new WaitForSeconds(1f);
        tutorialText.text = "これは残り時間だ";
        yield return new WaitForSeconds(2f);
        tutorialText.text = "無くなるまでにクリアせよ";
        yield return new WaitForSeconds(3f);
        tutorialText.text = "それではちゅーとりあるを終わる";
        yield return new WaitForSeconds(3f);
        arrowAnimator.gameObject.SetActive(false);
        tutorialText.text = "必ず日本に勝利を持ち帰れ";
        PlayerPrefs.SetInt("DoneTutorial", 1);
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("StageSelect");
    }
}
