using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer;
using SamuraiSoccer.SoccerGame;
using SamuraiSoccer.StageContents;
using SamuraiSoccer.Event;
using SamuraiSoccer.SoccerGame.AI;
using System.Collections.Generic;

namespace Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        public EasyCPUManager easyCPUManager;
        public BlackoutPanel blackoutPanel;
        public GameObject samurai;
        public GameObject ball;
        public GameObject enemy; //召喚する敵
        public GameObject teamGroup; //味方チームがまとまって入っているオブジェクト
        public GameObject enemyGroup; //敵チームがまとまって入っているオブジェクト
        public Text tutorialText; //チュートリアルで流れるテキスト
        public Text enemyNumber; //残り敵数(動かない敵は手動で変更)
        public Animator textAnimator; //テキストを動かして画面外までスライドするアニメーター
        public Animator arrowAnimator; //チュートリアル中に表示される矢印用のアニメーター
        public CinemachineVirtualCamera spotCamera; //何か焦点を当てるためのカメラ
        public CinemachineVirtualCamera samuraiCamera; //侍を追尾するためのカメラ
        public CinemachineVirtualCamera wholeviewCamera; //サッカーコート全体を見せるカメラ
        public GameObject exclamationMark; //敵の位置を指示してくれる！マーク
        [Tooltip("ホイッスル開始音")]
        public int whistleSENumber;

        private bool isThreeOnThreeFinished; // 3対3のミニゲームが終了したかどうか

        private void Awake()
        {
            // TODO : 後で消す(チュートリアル用の敵選手を呼び出すテストコード)
            var cliant = new InMemoryDataTransitClient<string>();
            cliant.Set(StorageKey.KEY_OPPONENT_TYPE, "opponent_Tutorial");
        }

        // Start is called before the first frame update
        private void Start()
        {
            // 3対3のミニゲーム終了時にフラグをtrueにする
            InGameEvent.Finish.Subscribe(_ =>{ isThreeOnThreeFinished = true; });
            var token = this.GetCancellationTokenOnDestroy();
            //勝手に動くボールを一時停止
            ball.SetActive(false);
            Runner(token).Forget();
        }

        private async UniTask Runner(CancellationToken cancellation_token = default)
        {
            //オープニングの会話
            await Opening(cancellation_token);

            //3対3
            await ThreeOnThree(cancellation_token);
            while (!CheckThreeOnThreeCleared())
            {
                await RetryThreeOnThree(cancellation_token);
            }

            //UIの説明
            await UIDescription(cancellation_token);

            PlayerPrefs.SetInt("DoneTutorial", 1);
            await UniTask.Delay(5000);
            SceneManager.LoadScene("StageSelect");
        }

        private async UniTask Opening(CancellationToken cancellation_token = default)
        {
            await UniTask.Delay(5000);
            tutorialText.gameObject.transform.parent.gameObject.SetActive(true);
            tutorialText.text = "ここではこの世界で戦うちゅーとりあるを行う";
            await UniTask.Delay(3000);
            tutorialText.text = "サムライは残された日本の希望だ";
            await UniTask.Delay(3000);
            Vector3 destination = new Vector3(10f, 3f, 30f);
            var enemyPrefab = Instantiate(enemy, destination, Quaternion.identity);
            exclamationMark.transform.position = destination + new Vector3(0f, 3f, 0f);
            exclamationMark.SetActive(true);
            enemyNumber.text = "1";
            //カメラを生成された選手に向ける
            samuraiCamera.Priority = 9;
            spotCamera.Follow = enemyPrefab.transform;
            spotCamera.LookAt = enemyPrefab.transform;
            spotCamera.Priority = 11;
            await UniTask.Delay(3000);
            tutorialText.text = "まずはここまで行こう";
            await UniTask.Delay(3000);
            exclamationMark.SetActive(false);
            //カメラをもとに戻す
            spotCamera.Priority = 9;
            samuraiCamera.Priority = 11;
            InGameEvent.PlayOnNext();
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            //敵に一定距離近づくまで待機
            while ((samurai.transform.position - destination).sqrMagnitude > 100)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            InGameEvent.PauseOnNext(true);

            //ファースト斬る
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "サムライが行うのはただ斬ることのみ";
            await UniTask.Delay(3000);
            tutorialText.text = "試しに目の前の人をひとおもいに斬れ";
            await UniTask.Delay(3000);
            InGameEvent.PauseOnNext(false);
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            //敵を切り倒して行って距離移動するまで待機
            while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400 || enemyPrefab.transform.position.y > -5)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            enemyNumber.text = "0";
            InGameEvent.PauseOnNext(true);
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "よし、それでいい";
            await UniTask.Delay(3000);
        }

        private async UniTask ThreeOnThree(CancellationToken cancellation_token = default)
        {
            ball.SetActive(true);
            InGameEvent.ResetOnNext();
            //カメラを全景にする
            samuraiCamera.Priority = 9;
            wholeviewCamera.Priority = 11;
            tutorialText.text = "今度は実戦形式だ";
            await UniTask.Delay(3000);
            tutorialText.text = "敵を全て斬りたおせ";
            await UniTask.Delay(3000);
            tutorialText.text = "ただし、れふぇりーには気をつけよ";
            await UniTask.Delay(3000);
            tutorialText.text = "視界内で刀を見せると反則だ";
            await UniTask.Delay(3000);
            tutorialText.text = "2回反則で退場になる";
            await UniTask.Delay(3000);
            tutorialText.text = "それでは試合開始だ";
            await UniTask.Delay(3000);
            //カメラをもとに戻す
            wholeviewCamera.Priority = 9;
            samuraiCamera.Priority = 11;
            InGameEvent.PlayOnNext();
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            //試合が終わるまで待機
            while (!isThreeOnThreeFinished)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
        }

        private bool CheckThreeOnThreeCleared()
        {
            InMemoryDataTransitClient<GameResult> resultTransitCliant = new InMemoryDataTransitClient<GameResult>();
            GameResult result;
            if(!resultTransitCliant.TryGet(StorageKey.KEY_WINORLOSE, out result)) return false;
            if(result == GameResult.Win) return true;
            else return false;
        }

        private async UniTask RetryThreeOnThree(CancellationToken cancellation_token = default)
        {
            InGameEvent.PauseOnNext(true);
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "しまった、れふぇりーに見つかってしまった";
            await UniTask.Delay(3000);
            tutorialText.text = "もう一度だ";
            await UniTask.Delay(1000);
            blackoutPanel.Blackout(5f);
            await UniTask.Delay(4000);
            // 敵と見方のオブジェクトを空にする
            easyCPUManager.team = new List<GameObject>();
            easyCPUManager.opp = new List<GameObject>();
            foreach (Transform child in teamGroup.transform) Destroy(child.gameObject);
            foreach (Transform child in enemyGroup.transform) Destroy(child.gameObject);
            // 再度生成を行う
            InGameEvent.ResetOnNext();
            await UniTask.Delay(1000);
            tutorialText.text = "次こそ成功させよ";
            await UniTask.Delay(3000);
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            InGameEvent.PlayOnNext();
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            isThreeOnThreeFinished = false;
            //試合が終わるまで待機
            while (!isThreeOnThreeFinished)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
        }

        private async UniTask UIDescription(CancellationToken cancellation_token = default)
        {
            InGameEvent.PauseOnNext(true);
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            //試合情報の見方説明
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "流石我らが希望、手際が良い";
            await UniTask.Delay(3000);
            tutorialText.text = "最後に試合で必要な情報を確認する";
            await UniTask.Delay(3000);
            arrowAnimator.gameObject.SetActive(true);
            tutorialText.text = "これは残りの敵の数だ";
            await UniTask.Delay(3000);
            tutorialText.text = "0になれば日本の勝利だ";
            await UniTask.Delay(3000);
            arrowAnimator.SetTrigger("MoveArrow");
            await UniTask.Delay(1000);
            tutorialText.text = "これは残り時間だ";
            await UniTask.Delay(2000);
            tutorialText.text = "無くなるまでにクリアせよ";
            await UniTask.Delay(3000);
            tutorialText.text = "それではちゅーとりあるを終わる";
            await UniTask.Delay(3000);
            arrowAnimator.gameObject.SetActive(false);
            tutorialText.text = "必ず日本に勝利を持ち帰れ";
        }  
    }
}
