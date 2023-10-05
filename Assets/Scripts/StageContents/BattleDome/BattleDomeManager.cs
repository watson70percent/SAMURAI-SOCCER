using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine.SceneManagement;
using SamuraiSoccer;

namespace SamuraiSoccer.StageContents.BattlerDome
{

    

    public class BattleDomeManager : MonoBehaviour
    {
        int clearPoint = 3;

        // Start is called before the first frame update
        void Start()
        {
            InGameEvent.TeammateScore.Subscribe(score => { if (score > clearPoint) { Clear(); } }).AddTo(this);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void Clear()
        {
            InGameEvent.FinishOnNext();
            var win = new InMemoryDataTransitClient<GameResult>();
            win.Set(StorageKey.KEY_WINORLOSE, GameResult.Win);
            SceneManager.LoadScene("Result");
        }
    }
}
