using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer;
using SamuraiSoccer.StageContents;
using SamuraiSoccer.Event;
using SamuraiSoccer.SoccerGame.AI;
using SamuraiSoccer.UI;
using System.Collections.Generic;

namespace Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        public EasyCPUManager easyCPUManager;
        public GameObject samurai;
        public GameObject referee;
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
        public int remainChargeAttackNum; //残りのため斬り回数
        public GameObject chargeAttackText; // ため斬りの回数表示テキスト
        public Text remainChargeAttackText; // 残り敵数の表示
        public GameObject uiMask;
        public Animator leftControllerFocusAnimator;
        public Animator rightControllerFocusAnimator;
        public TouchProvider m_provider;

        private bool isThreeOnThreeFinished; // 3対3のミニゲームが終了したかどうか

        private bool chargeAttackTutorialFinished; // ため斬りのチュートリアルが終了したかどうか

        private Vector3 initBallPos;

        private bool m_isTouched = false; //画面に触れたかどうか

        private void Awake()
        {
            InGameEvent.ResetResetSubject();
            // TODO : 後で消す(チュートリアル用の敵選手を呼び出すテストコード)
            var cliant = new InMemoryDataTransitClient<string>();
            cliant.Set(StorageKey.KEY_OPPONENT_TYPE, "opponent_Tutorial");
        }

        // Start is called before the first frame update
        private void Start()
        {
            // 3対3のミニゲーム終了時にフラグをtrueにする
            InGameEvent.Finish.Subscribe(_ => { isThreeOnThreeFinished = true; });
            var token = this.GetCancellationTokenOnDestroy();
            //勝手に動くボールを一時停止
            ball.SetActive(false);
            initBallPos = ball.transform.position;
            // ため斬りを使用不可にする
            PlayerEvent.SetLockChargeAttack(true);
            // フォーカス演出を非表示(最初から非表示にすると画面サイズに合わせたUIの縮小が機能しない)
            leftControllerFocusAnimator.SetTrigger("None");
            rightControllerFocusAnimator.SetTrigger("None");
            // タップしたときに処理が進むReactivePropertyを登録
            m_provider.IsTouchingReactiveProperty.Where(b => b).Subscribe(_ =>
            {
                m_isTouched = true;
            }).AddTo(this);
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

            //ため斬り
            await ChargeAttack(cancellation_token);
            while (remainChargeAttackNum > 0)
            {
                await UniTask.Yield();
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
            await WaitUntilIsTouched();
            tutorialText.text = "サムライは残された日本の希望だ";
            await WaitUntilIsTouched();
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
            await UniTask.Delay(3000); // これは演出だから変えない
            tutorialText.text = "まずはここまで行こう";
            await WaitUntilIsTouched();
            exclamationMark.SetActive(false);
            leftControllerFocusAnimator.SetTrigger("Focus");
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
            leftControllerFocusAnimator.SetTrigger("None");
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "サムライが行うのはただ斬ることのみ";
            await WaitUntilIsTouched();
            tutorialText.text = "試しに目の前の人をひとおもいに斬れ";
            await WaitUntilIsTouched();
            rightControllerFocusAnimator.SetTrigger("Focus");
            InGameEvent.PlayOnNext();
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            //敵が一定距離吹っ飛ぶまで待機
            while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400 || Mathf.Abs(enemyPrefab.transform.position.y) < 5)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            rightControllerFocusAnimator.SetTrigger("None");
            enemyNumber.text = "0";
            InGameEvent.PauseOnNext(true);
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "よし、それでいい";
            await WaitUntilIsTouched();
        }

        private async UniTask ThreeOnThree(CancellationToken cancellation_token = default)
        {
            ball.SetActive(true);
            InGameEvent.ResetOnNext();
            //カメラを全景にする
            samuraiCamera.Priority = 9;
            wholeviewCamera.Priority = 11;
            tutorialText.text = "今度は実戦形式だ";
            await WaitUntilIsTouched();
            tutorialText.text = "敵を全て斬りたおせ";
            await WaitUntilIsTouched();
            tutorialText.text = "ただし、れふぇりーには気をつけよ";
            await WaitUntilIsTouched();
            tutorialText.text = "視界内で刀を見せると反則だ";
            await WaitUntilIsTouched();
            tutorialText.text = "2回反則で退場になる";
            await WaitUntilIsTouched();
            tutorialText.text = "それでは試合開始だ";
            await WaitUntilIsTouched();
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
            if (!resultTransitCliant.TryGet(StorageKey.KEY_WINORLOSE, out result)) return false;
            if (result == GameResult.Win) return true;
            else return false;
        }

        private async UniTask RetryThreeOnThree(CancellationToken cancellation_token = default)
        {
            InGameEvent.PauseOnNext(true);
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "しまった、れふぇりーに見つかってしまった";
            await WaitUntilIsTouched();
            tutorialText.text = "もう一度だ";
            await UniTask.Delay(1000);
            UIEffectEvent.BlackOutOnNext(5f);
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
            await WaitUntilIsTouched();
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            InGameEvent.PlayOnNext();
            // キーがセットされていたら全て倒し終わった時にバグるので吐き出させておく
            var client = new InMemoryDataTransitClient<GameResult>();
            client.TryGet(StorageKey.KEY_WINORLOSE, out var outvalue);
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

        private async UniTask ChargeAttack(CancellationToken cancellation_token = default)
        {
            InGameEvent.PauseOnNext(true);
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            // 敵と見方のオブジェクトを空にする
            easyCPUManager.team = new List<GameObject>();
            foreach (Transform child in teamGroup.transform) Destroy(child.gameObject);
            Destroy(referee);
            ball.SetActive(false);
            ball.transform.position = initBallPos;
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "流石我らが希望、手際が良い";
            await WaitUntilIsTouched();
            tutorialText.text = "次はため斬りの練習だ";
            await WaitUntilIsTouched();
            uiMask.SetActive(true);
            tutorialText.text = "斬る力をためることで前方に移動しながら斬れるぞ";
            await WaitUntilIsTouched();
            tutorialText.text = "試しに3回ため斬りしよう";
            rightControllerFocusAnimator.SetTrigger("Focus");
            chargeAttackText.SetActive(true);
            remainChargeAttackText.text = remainChargeAttackNum.ToString();
            // ため斬りを使用可能にする
            PlayerEvent.SetLockChargeAttack(false);
            // チャージアタック時の処理を登録
            PlayerEvent.IsInChargeAttack.Subscribe(
                x =>
                {
                    if (x)
                    {
                        MinusChargeAttackNum();
                        CheckChargeAttackTutorialFinished();
                        rightControllerFocusAnimator.SetTrigger("None");
                    }
                }
            ).AddTo(this);
            await WaitUntilIsTouched();
            InGameEvent.PlayOnNext();
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
        }

        public void MinusChargeAttackNum()
        {
            remainChargeAttackNum--;
            remainChargeAttackText.text = remainChargeAttackNum.ToString();
        }

        public void CheckChargeAttackTutorialFinished()
        {
            if (!chargeAttackTutorialFinished)
            {
                chargeAttackTutorialFinished = true;
                uiMask.SetActive(false);
            }
        }

        private async UniTask UIDescription(CancellationToken cancellation_token = default)
        {
            chargeAttackText.SetActive(false);
            _ = SoundMaster.Instance.PlaySE(whistleSENumber);
            //試合情報の見方説明
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "よくぞ使いこなして見せた";
            await WaitUntilIsTouched();
            tutorialText.text = "本番でも使いこなすのだ";
            await WaitUntilIsTouched();
            tutorialText.text = "最後に試合で必要な情報を確認する";
            await WaitUntilIsTouched();
            arrowAnimator.gameObject.SetActive(true);
            tutorialText.text = "これは残りの敵の数だ";
            await WaitUntilIsTouched();
            tutorialText.text = "0になれば日本の勝利だ";
            await WaitUntilIsTouched();
            arrowAnimator.SetTrigger("MoveArrow");
            await UniTask.Delay(1000);
            tutorialText.text = "これは残り時間だ";
            await WaitUntilIsTouched();
            tutorialText.text = "無くなるまでにクリアせよ";
            await WaitUntilIsTouched();
            tutorialText.text = "それではちゅーとりあるを終わる";
            await WaitUntilIsTouched();
            arrowAnimator.gameObject.SetActive(false);
            tutorialText.text = "必ず日本に勝利を持ち帰れ";
        }

        private async UniTask WaitUntilIsTouched()
        {
            m_isTouched = false;
            while (!m_isTouched)
            {
                await UniTask.Yield();
            }
        }
    }
}
