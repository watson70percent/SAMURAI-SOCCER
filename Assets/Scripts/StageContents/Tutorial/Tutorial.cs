﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading;
using SamuraiSoccer;
using SamuraiSoccer.Event;
using SamuraiSoccer.SoccerGame;
using SamuraiSoccer.SoccerGame.AI;

namespace Tutorial
{
    public class Tutorial : MonoBehaviour
    {
        public GameObject samurai;
        public GameObject ball;
        public GameObject firstYellowCard;
        public GameObject enemy; //召喚する敵
        public Text tutorialText; //チュートリアルで流れるテキスト
        public Text enemyNumber; //残り敵数(手動で変更)
        public Animator textAnimator; //テキストを動かして画面外までスライドするアニメーター
        public Animator arrowAnimator; //チュートリアル中に表示される矢印用のアニメーター
        public CinemachineVirtualCamera spotCamera; //何か焦点を当てるためのカメラ
        public CinemachineVirtualCamera samuraiCamera; //侍を追尾するためのカメラ
        public GameObject exclamationMark; //敵の位置を指示してくれる！マーク

        private void Awake()
        {
            // TODO : チュートリアル用の敵選手を呼び出すテストコード
            var cliant = new InMemoryDataTransitClient<string>();
            cliant.Set(StorageKey.KEY_OPPONENT_TYPE, "opponent_Tutorial");
        }

        // Start is called before the first frame update
        void Start()
        {
            var token = this.GetCancellationTokenOnDestroy();
            Runner(token).Forget();
        }

        async UniTask Runner(CancellationToken cancellation_token = default)
        {
            // 勝手に動くボールを一時停止
            ball.SetActive(false);
            //テキスト表示1
            await UniTask.Delay(5000);
            tutorialText.gameObject.transform.parent.gameObject.SetActive(true);
            tutorialText.text = "ここではこの世界で戦うちゅーとりあるを行う";
            await UniTask.Delay(3000);
            tutorialText.text = "サムライは残された日本の希望だ";
            await UniTask.Delay(3000);
            Vector3 destination = new Vector3(10f, 3f, 30f);
            var enemyPrefab = Instantiate(enemy, destination, Quaternion.identity);
            enemyPrefab.GetComponent<EasyCPU>().enabled = false; // この選手は動かない(EasyCPUが必要ない)
            exclamationMark.transform.position = destination + new Vector3(0f, 3f, 0f);
            exclamationMark.SetActive(true);
            //カメラを生成された選手に向ける
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
            textAnimator.SetTrigger("SlideText");
            await UniTask.Delay(2000);
            //テキストを非表示に
            tutorialText.text = "";
            //敵に一定距離近づくまで待機
            while ((samurai.transform.position - destination).sqrMagnitude > 100)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            InGameEvent.PauseOnNext(true);
            //テキスト表示2
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "サムライが行うのはただ斬ることのみ";
            await UniTask.Delay(3000);
            tutorialText.text = "試しに目の前の人をひとおもいに斬れ";
            await UniTask.Delay(3000);
            InGameEvent.PauseOnNext(false);
            textAnimator.SetTrigger("SlideText");
            await UniTask.Delay(2000);
            //テキストを非表示に
            tutorialText.text = "";
            //敵を切り倒して行って距離移動するまで待機
            while ((enemyPrefab.transform.position - destination).sqrMagnitude < 400 || enemyPrefab.transform.position.y > -5)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            InGameEvent.PauseOnNext(true);
            enemyNumber.text = 1.ToString();
            //テキスト表示2
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "よし、それでいい";
            await UniTask.Delay(3000);
            ball.SetActive(true);
            InGameEvent.ResetOnNext();
            tutorialText.text = "今度は実戦形式だ";
            await UniTask.Delay(3000);
            tutorialText.text = "3対3をしている敵選手を斬れ";
            await UniTask.Delay(3000);
            tutorialText.text = "ただし、れふぇりーには気をつけろ";
            await UniTask.Delay(3000);
            InGameEvent.PauseOnNext(false);
            textAnimator.SetTrigger("SlideText");
            //テキストを非表示に
            await UniTask.Delay(2000);
            tutorialText.text = "";
            //イエローカードが出るまで待機
            while (!firstYellowCard.activeSelf)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, cancellation_token);
            }
            InGameEvent.PauseOnNext(true);
            //テキスト表示3
            textAnimator.SetTrigger("ReturnText");
            tutorialText.text = "しまった！れふぇりーに見られてしまった";
            await UniTask.Delay(3000);
            tutorialText.text = "2回見つかると退場だ、気をつけろ";
            await UniTask.Delay(3000);
            tutorialText.text = "最後に確認する";
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
            PlayerPrefs.SetInt("DoneTutorial", 1);
            await UniTask.Delay(5000);
            SceneManager.LoadScene("StageSelect");
        }
    }

}
