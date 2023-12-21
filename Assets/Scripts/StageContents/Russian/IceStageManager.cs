using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamuraiSoccer.Event;
using SamuraiSoccer.SoccerGame;

namespace SamuraiSoccer.StageContents.Russian
{
    /// <summary>
    /// ロシアステージのステージ設定を行う．
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class IceStageManager : MonoBehaviour
    {
        public StagePrefabManager stageManager;
        public Texture greenTexture;
        public Texture iceTexture;
        public Material greenMaterial;
        public Material iceMaterial;
        public GameObject eclanoplan;
        public GameObject mountObject;
        public GameObject iceObject;
        static public int no = 2;

        private void Start()
        {
            var dataTransitClient = new InMemoryDataTransitClient<int>();
            var stage = dataTransitClient.Get(StorageKey.KEY_FIELDNUMBER);
            dataTransitClient.Set(StorageKey.KEY_FIELDNUMBER, stage);
            switch (stage)
            {
                case 0:
                    stageManager.groundTexture = greenTexture;
                    stageManager.groundMaterial = greenMaterial;
                    eclanoplan.SetActive(false);
                    iceObject.SetActive(false);
                    mountObject.SetActive(true);
                    break;
                case 1:
                    stageManager.groundTexture = iceTexture;
                    stageManager.groundMaterial = iceMaterial;
                    eclanoplan.SetActive(false);
                    iceObject.SetActive(true);
                    mountObject.SetActive(false);
                    break;
                case 2:
                    stageManager.groundTexture = iceTexture;
                    stageManager.groundMaterial = iceMaterial;
                    eclanoplan.SetActive(true);
                    iceObject.SetActive(true);
                    mountObject.SetActive(false);
                    break;
            }
        }
    }
}
