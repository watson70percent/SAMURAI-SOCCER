using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Linq;
using System.IO;

public class PrimitivesBuilder : EditorWindow
{
    [MenuItem("Help/Sugi's Temp/PrimitivesBuilder")]
    static void Open()
    {
        GetWindow<PrimitivesBuilder>();
    }

    private void OnGUI()
    {
        var button = GUILayout.Button("プリミティブを自動生成");
        if (button)
        {
            CreateCircle()
        }
    }


    void CreateCircle()
    {
        var mesh = new Mesh();

        var xCount = 5;
        var yCount = 5;
        mesh.vertices = (
            from x in Enumerable.Range(0, xCount)
            from y in Enumerable.Range(0, yCount)
            from direction in new[] { Vector3.up, Vector3.right, Vector3.forward }
            from normal in new[] { direction, -direction }
            from binormal in new[] { new Vector3(normal.z, normal.x, normal.y) }
            from tangent in new[] { Vector3.Cross(normal, binormal) }

            from binormal2 in new[] { binormal, -binormal }
            from tangent2 in new[] { tangent, -tangent }

            from vec in new[] { normal + binormal2 + tangent2 }
            select vec
            ).ToArray();

        mesh.RecalculateNormals();
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100);

        if (!Directory.Exists("Assets/Resources/Primitives")) Directory.CreateDirectory("Assets/Resources/Primitives/");
        AssetDatabase.CreateAsset(mesh, "Assets/Resources/Primitives/Boxes.asset");
    }
}
