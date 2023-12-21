using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer.StageContents.StageSelect
{
    [CreateAssetMenu]
    public class StagePreviewDatas : ScriptableObject
    {
        public List<StagePreviewData> stageSelectList = new List<StagePreviewData>();
    }

    [System.Serializable]
    public class StagePreviewData
    {
        public int stageNumber; //ステージ番号(全ステージで通し番号)
        public string name;
        public string previewName; //表示名
        [TextArea(1, 5)]
        public string summary; //ステージの要約文
        public Sprite stageImage; //ステージを象徴する画像
        public SceneObject gameScene; //ステージのScene

        public int fieldNumber; //ステージ番号(ロシア用)
        public int groundNumber; //フィールド番号(ロシア用)
        public string opponentType; //敵名(Prefab＋敵性能ファイル読み込み用)
    }
}

