using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

namespace SamuraiSoccer.UI
{
    /// <summary>
    /// 巻物をなめらかに移動させる
    /// </summary>
    public class ScrollScript : MonoBehaviour
    {
        [SerializeField] RectTransform rectra;//動かす巻物のRectTransform
        [SerializeField] float slideTime;//移動にかける時間
        [SerializeField] GameObject ScrollObject;//巻物の3Dオブジェクト
        [SerializeField] float rotSpeed;//回転速度係数
        [SerializeField] Material[] FlagMaterials;//旗のマテリアル
        MeshRenderer ScrollMaterial;//巻物(3D)のMeshRenderer
        public enum CountryNameForScroll//国と巻物のマテリアルの番号を一致させるenum
        {
            Japan,
            UK,
            China,
            America,
            Russia
        }

        private Vector3 initRot;

        private void Start()
        {
            initRot = ScrollObject.transform.eulerAngles;
            ScrollMaterial = ScrollObject.GetComponent<MeshRenderer>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startX">巻物の初期位置のX座標</param>
        /// <param name="goalX">巻物の最終位置のX座標</param>
        /// <param name="Y">巻物のY座標</param>
        /// <param name="rollTime">移動、回転にかける時間</param>
        /// <returns></returns>
        public async UniTask ScrollSlide(float startX, float goalX, float Y, float rollTime)
        {
            float elapsedTime = 0;
            rectra.localPosition = new Vector3(startX, Y, rectra.localPosition.z);
            while (elapsedTime < rollTime)
            {
                elapsedTime += Time.deltaTime;
                float x = easeOutCubic(elapsedTime, goalX, startX, rollTime);
                rectra.anchoredPosition = new Vector2(x, Y);
                ScrollObject.transform.eulerAngles = new Vector3(0, -(x - startX) * rotSpeed, 0);
                await UniTask.Delay(1);
            }
        }

        /// <summary>
        /// イージングの計算
        /// </summary>
        /// <param name="t">現在の時刻</param>
        /// <param name="goal">最終地点のx座標</param>
        /// <param name="start">t=0のx座標</param>
        /// <param name="goalTime">最終地点の時刻</param>
        /// <returns>現在のx座標</returns>
        float easeOutCubic(float t, float goal, float start, float goalTime)
        {
            return (goal - start) * (1 - Mathf.Pow(goalTime - t, 3)) + start;
        }

        public void ResetObject()
        {
            rectra.localPosition = new Vector3(startX, rectra.localPosition.y, rectra.localPosition.z);
            ScrollObject.transform.eulerAngles = initRot;
        }

        /// <summary>
        /// 巻物のMaterialを変更する
        /// </summary>
        /// <param name="nowCountry">巻物に反映させる国(Japan,UK,China,America,Russia)</param>
        public void ChangeMaterial(CountryNameForScroll nowCountry)
        {
            ScrollMaterial.material = FlagMaterials[(int)nowCountry];
        }
    }
}
