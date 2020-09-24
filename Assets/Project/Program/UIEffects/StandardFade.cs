using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 他のタイプにも対応したいが、現状はTextのみ対応
/// </summary>
public class StandardFade : MonoBehaviour
{
    /// <summary>
    /// テキストをフェードイン
    /// </summary>
    /// <param name="text">
    /// フェードインする対象
    /// </param>
    /// <param name="intervalMilliSec">
    /// フェード全体の秒数
    /// </param>
    public static async Task FadeIn(Text text,int FadeLength)
    {
        while (text.color.a<1)
        {
            await Task.Delay(1000/60);
            var alpha = text.color.a + Mathf.Sin(Mathf.PI/180)/FadeLength ;
            SetColor(text, alpha);
        }
    }

    /// <summary>
    /// テキストをフェードアウト
    /// </summary>
    /// <param name="text">
    /// フェードアウトする対象
    /// </param>
    /// <param name="FadeLength">
    /// フェード全体の秒数
    /// </param>
    /// <returns></returns>
    public static async Task FadeOut(Text text, int FadeLength)
    {
        while (text.color.a > 0)
        {
            await Task.Delay(1000/60);
            var alpha = text.color.a - Mathf.Sin(Mathf.PI / 180)/FadeLength;
            SetColor(text, alpha);
        }
    }

    /// <summary>
    /// colorのα値をセットする
    /// </summary>
    /// <param name="text"></param>
    /// <param name="alpha"></param>
    static void SetColor(Text text,float alpha)
    {
        var txcolor = text.color;
        txcolor.a = alpha;
        text.color = txcolor;
    }
}
