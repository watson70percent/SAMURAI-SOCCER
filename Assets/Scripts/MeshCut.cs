using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SamuraiSoccer
{
    public class MeshCut : MonoBehaviour
    {
        static Mesh _targetMesh;
        static Vector3[] _targetVertices;
        static Vector3[] _targetNormals;
        static Vector2[] _targetUVs;   //この3つはめっちゃ大事でこれ書かないと10倍くらい重くなる(for文中で使うから参照渡しだとやばい)
        static MeshData _frontMeshData = new MeshData(); //切断面の法線に対して表側
        static MeshData _backMeshData = new MeshData(); //裏側
        static Plane _slashPlane;//切断平面
        static bool[] _isFront;//頂点が切断面に対して表にあるか裏にあるか
                               /// <summary>
                               /// gameObjectを切断して2つのMeshにして返します.
                               /// 1つ目のMeshが切断面の法線に対して表側, 2つ目が裏側です.
                               /// 何度も切るようなオブジェクトでも頂点数が増えないように処理をしてあります
                               /// </summary>
                               /// <param name="target">切断対象のgameObject</param>
                               /// <param name="planeAnchorPoint">切断面上の1点</param>
                               /// <param name="planeNormalDirection">切断面の法線</param>
                               /// <returns></returns>
        public static Mesh[] CutMesh(GameObject target, Vector3 planeAnchorPoint, Vector3 planeNormalDirection)
        {
            _targetMesh = target.GetComponent<MeshFilter>().mesh; //Mesh情報取得
            _targetVertices = _targetMesh.vertices;
            _targetNormals = _targetMesh.normals;
            _targetUVs = _targetMesh.uv; //for文で_targetMeshから参照するのは非常に重くなるのでここで配列に格納してfor文ではここから渡す
            _frontMeshData.ClearAll(); //staticなクラスで宣言してあるのでここで初期化(staticで宣言する意味は高速化のため？？？)
            _backMeshData.ClearAll();


            Vector3 scale = target.transform.localScale;//localscaleに合わせてPlaneに入れるnormalに補正をかける

            _slashPlane = new Plane(Vector3.Scale(scale, target.transform.InverseTransformDirection(planeNormalDirection)), target.transform.InverseTransformPoint(planeAnchorPoint));

            int vNum = _targetVertices.Length;
            _isFront = new bool[vNum];
            for (int i = 0; i < vNum; i++)
            {
                _isFront[i] = _slashPlane.GetSide(_targetVertices[i]);//planeの表側にあるか裏側にあるかを判定.(たぶん表だったらtrue)
            }



            bool[] sides = new bool[3]; //先に宣言しておいてfor文中で使いまわしたほうが早い???
            int[] indices;
            int p1, p2, p3;

            for (int sub = 0; sub < _targetMesh.subMeshCount; sub++)
            {
                indices = _targetMesh.GetIndices(sub);

                _frontMeshData.subMeshIndices.Add(new List<int>());//subMeshが増えたことを追加してる
                _frontMeshData.triangleFragments.Add(new List<TriangleFragment>()); //切断したMeshの破片を入れるListもsubMeshごとに追加
                _frontMeshData.rectangleFragments.Add(new List<RectangleFragment>());

                _backMeshData.subMeshIndices.Add(new List<int>());
                _backMeshData.triangleFragments.Add(new List<TriangleFragment>());
                _backMeshData.rectangleFragments.Add(new List<RectangleFragment>());

                int length = indices.Length; //先にintに入れておいたほうが処理が早いかも???
                for (int i = 0; i < length; i += 3)
                {
                    p1 = indices[i];
                    p2 = indices[i + 1];
                    p3 = indices[i + 2];


                    sides[0] = _isFront[p1];
                    sides[1] = _isFront[p2];
                    sides[2] = _isFront[p3];


                    if (sides[0] == sides[1] && sides[0] == sides[2])
                    {
                        if (sides[0])
                        {
                            _frontMeshData.AddTriangle(p1, p2, p3, sub);
                        }
                        else
                        {
                            _backMeshData.AddTriangle(p1, p2, p3, sub);
                        }
                    }
                    else
                    {
                        //三角ポリゴンを形成する各点で面に対する表裏が異なる場合, つまり切断面と重なっている平面は分割する.
                        Sepalate(sides, new int[3] { p1, p2, p3 }, sub);
                    }

                }
            }


            _frontMeshData.ConnectFragments(); //切断されたMeshの破片は最後にくっつけられるところはくっつけておく
            _backMeshData.ConnectFragments();


            Mesh frontMesh = new Mesh();
            frontMesh.name = "Split Mesh front";
            frontMesh.vertices = _frontMeshData.vertices.ToArray();
            frontMesh.triangles = _frontMeshData.triangles.ToArray();
            frontMesh.normals = _frontMeshData.normals.ToArray();
            frontMesh.uv = _frontMeshData.uvs.ToArray();

            frontMesh.subMeshCount = _frontMeshData.subMeshIndices.Count;
            for (int i = 0; i < _frontMeshData.subMeshIndices.Count; i++)
            {
                frontMesh.SetIndices(_frontMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            Mesh backMesh = new Mesh();
            backMesh.name = "Split Mesh back";
            backMesh.vertices = _backMeshData.vertices.ToArray();
            backMesh.triangles = _backMeshData.triangles.ToArray();
            backMesh.normals = _backMeshData.normals.ToArray();
            backMesh.uv = _backMeshData.uvs.ToArray();

            backMesh.subMeshCount = _backMeshData.subMeshIndices.Count;
            for (int i = 0; i < _backMeshData.subMeshIndices.Count; i++)
            {
                backMesh.SetIndices(_backMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            return new Mesh[2] { frontMesh, backMesh };
        }

        /// <summary>
        /// gameObjectを切断して2つのMeshにして返します.
        /// 1つ目のMeshが切断面の法線に対して表側, 2つ目が裏側です.
        /// 切断の処理が雑なので何度も斬ると頂点数が膨れ上がります. そのかわり一回斬るだけならこっちが早い
        /// </summary>
        /// <param name="target">切断対象のgameObject</param>
        /// <param name="planeAnchorPoint">切断面上の1点</param>
        /// <param name="planeNormalDirection">切断面の法線</param>
        /// <returns></returns>
        public static Mesh[] CutMeshOnce(GameObject target, Vector3 planeAnchorPoint, Vector3 planeNormalDirection)
        {
            _targetMesh = target.GetComponent<MeshFilter>().mesh; //Mesh情報取得
            _targetVertices = _targetMesh.vertices;
            _targetNormals = _targetMesh.normals;
            _targetUVs = _targetMesh.uv; //for文で_targetMeshから参照するのは非常に重くなるのでここで配列に格納してfor文ではここから渡す
            _frontMeshData.ClearAll(); //staticなクラスで宣言してあるのでここで初期化(staticで宣言する意味は高速化のため？？？)
            _backMeshData.ClearAll();

            Vector3 scale = target.transform.localScale;//localscaleに合わせてPlaneに入れるnormalに補正をかける

            _slashPlane = new Plane(Vector3.Scale(scale, target.transform.InverseTransformDirection(planeNormalDirection)), target.transform.InverseTransformPoint(planeAnchorPoint));


            int vNum = _targetVertices.Length;
            _isFront = new bool[vNum];
            for (int i = 0; i < vNum; i++)
            {
                _isFront[i] = _slashPlane.GetSide(_targetVertices[i]);//planeの表側にあるか裏側にあるかを判定.(たぶん表だったらtrue)
            }

            bool[] sides = new bool[3]; //先に宣言しておいてfor文中で使いまわしたほうが早い???
            int[] indices;
            int p1, p2, p3;

            for (int sub = 0; sub < _targetMesh.subMeshCount; sub++)
            {
                indices = _targetMesh.GetIndices(sub);

                _frontMeshData.subMeshIndices.Add(new List<int>());//subMeshが増えたことを追加してる

                _backMeshData.subMeshIndices.Add(new List<int>());


                int length = indices.Length; //先にintに入れておいたほうが処理が早いかも???
                for (int i = 0; i < length; i += 3)
                {
                    p1 = indices[i];
                    p2 = indices[i + 1];
                    p3 = indices[i + 2];



                    sides[0] = _isFront[p1];
                    sides[1] = _isFront[p2];
                    sides[2] = _isFront[p3];


                    if (sides[0] == sides[1] && sides[0] == sides[2])
                    {
                        if (sides[0])
                        {
                            _frontMeshData.AddTriangle(p1, p2, p3, sub);
                        }
                        else
                        {
                            _backMeshData.AddTriangle(p1, p2, p3, sub);
                        }
                    }
                    else
                    {
                        //三角ポリゴンを形成する各点で面に対する表裏が異なる場合, つまり切断面と重なっている平面は分割する.
                        SepalateOnce(sides, new int[3] { p1, p2, p3 }, sub);
                    }

                }
            }


            Mesh frontMesh = new Mesh();
            frontMesh.name = "Split Mesh front";
            frontMesh.vertices = _frontMeshData.vertices.ToArray();
            frontMesh.triangles = _frontMeshData.triangles.ToArray();
            frontMesh.normals = _frontMeshData.normals.ToArray();
            frontMesh.uv = _frontMeshData.uvs.ToArray();

            frontMesh.subMeshCount = _frontMeshData.subMeshIndices.Count;
            for (int i = 0; i < _frontMeshData.subMeshIndices.Count; i++)
            {
                frontMesh.SetIndices(_frontMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            Mesh backMesh = new Mesh();
            backMesh.name = "Split Mesh back";
            backMesh.vertices = _backMeshData.vertices.ToArray();
            backMesh.triangles = _backMeshData.triangles.ToArray();
            backMesh.normals = _backMeshData.normals.ToArray();
            backMesh.uv = _backMeshData.uvs.ToArray();

            backMesh.subMeshCount = _backMeshData.subMeshIndices.Count;
            for (int i = 0; i < _backMeshData.subMeshIndices.Count; i++)
            {
                backMesh.SetIndices(_backMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            return new Mesh[2] { frontMesh, backMesh };
        }



        /// <summary>
        /// gameObjectを雑に切断して2つのMeshにして返します. 切断面がザラザラなのでハイポリにしか使えませんが処理はちょっと早いです. 1つ目のMeshが切断面の法線に対して表側, 2つ目が裏側です.
        /// </summary>
        /// <param name="target">切断対象のgameObject</param>
        /// <param name="planeAnchorPoint">切断面上の1点</param>
        /// <param name="planeNormalDirection">切断面の法線</param>
        /// <returns></returns>
        public static Mesh[] CutMeshRough(GameObject target, Vector3 planeAnchorPoint, Vector3 planeNormalDirection)
        {
            _targetMesh = target.GetComponent<MeshFilter>().mesh;
            _targetVertices = _targetMesh.vertices;
            _targetNormals = _targetMesh.normals;
            _targetUVs = _targetMesh.uv;
            _frontMeshData.ClearAll();
            _backMeshData.ClearAll();

            Vector3 scale = target.transform.localScale;
            //localscaleに合わせてPlaneに入れるnormalに補正をかける
            Vector3 scaleCorrection = new Vector3(1 / scale.z, 1 / scale.y, 1 / scale.z);

            _slashPlane = new Plane(Vector3.Scale(scale, target.transform.InverseTransformDirection(planeNormalDirection)), target.transform.InverseTransformPoint(planeAnchorPoint));


            int vNum = _targetVertices.Length;
            _isFront = new bool[vNum];
            for (int i = 0; i < vNum; i++)
            {
                _isFront[i] = _slashPlane.GetSide(_targetVertices[i]);//planeの表側にあるか裏側にあるかを判定.(たぶん表だったらtrue)
            }
            int[] indices;
            int p1, p2, p3;

            //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

            for (int sub = 0; sub < _targetMesh.subMeshCount; sub++)
            {
                indices = _targetMesh.GetIndices(sub);

                _frontMeshData.subMeshIndices.Add(new List<int>());

                _backMeshData.subMeshIndices.Add(new List<int>());

                int length = indices.Length;
                for (int i = 0; i < length; i += 3)
                {
                    p1 = indices[i];
                    p2 = indices[i + 1];
                    p3 = indices[i + 2];

                    //sw.Start();
                    if (_isFront[p1]) //1番目の頂点のある側に残りの2つのポリゴンを持っていく(切断面はジグザグ)
                    {
                        _frontMeshData.AddTriangle(p1, p2, p3, sub);
                    }
                    else
                    {
                        _backMeshData.AddTriangle(p1, p2, p3, sub);
                    }
                    //sw.Stop();
                }
            }


            //Debug.Log(sw.ElapsedMilliseconds + "ms");


            Mesh frontMesh = new Mesh();
            frontMesh.name = "Split Mesh front";
            frontMesh.vertices = _frontMeshData.vertices.ToArray();
            frontMesh.triangles = _frontMeshData.triangles.ToArray();
            frontMesh.normals = _frontMeshData.normals.ToArray();
            frontMesh.uv = _frontMeshData.uvs.ToArray();

            frontMesh.subMeshCount = _frontMeshData.subMeshIndices.Count;
            for (int i = 0; i < _frontMeshData.subMeshIndices.Count; i++)
            {
                frontMesh.SetIndices(_frontMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            Mesh backMesh = new Mesh();
            backMesh.name = "Split Mesh back";
            backMesh.vertices = _backMeshData.vertices.ToArray();
            backMesh.triangles = _backMeshData.triangles.ToArray();
            backMesh.normals = _backMeshData.normals.ToArray();
            backMesh.uv = _backMeshData.uvs.ToArray();

            backMesh.subMeshCount = _backMeshData.subMeshIndices.Count;
            for (int i = 0; i < _backMeshData.subMeshIndices.Count; i++)
            {
                backMesh.SetIndices(_backMeshData.subMeshIndices[i].ToArray(), MeshTopology.Triangles, i);
            }

            return new Mesh[2] { frontMesh, backMesh };
        }




        private static void Sepalate(bool[] sides, int[] vertexIndices, int submesh)
        {
            Vector3[] frontPoints = new Vector3[2];
            Vector3[] frontNormals = new Vector3[2]; //ポリゴンの頂点が3つなのにfront-backで配列が2つずつなのは対象的な処理を行うため
            Vector2[] frontUVs = new Vector2[2];
            Vector3[] backPoints = new Vector3[2];
            Vector3[] backNormals = new Vector3[2];
            Vector2[] backUVs = new Vector2[2];

            bool didset_front = false; //表側に1つ頂点を置いたかどうか
            bool didset_back = false;
            bool twoPointsInFront = false;//表側に点が2つあるか(これがfalseのときは裏側に点が2つある)

            int p = 0;
            int f0 = 0, f1 = 0, b0 = 0, b1 = 0; //頂点のindex番号を格納するのに使用

            int faceType = 0;//面が外側を向くように整列させるために使用

            for (int side = 0; side < 3; side++)
            {
                p = vertexIndices[side];

                if (sides[side])
                {
                    faceType += (side); //これの合計値によって面の割り方の種類を6つに分類できる
                    if (!didset_front)
                    {
                        didset_front = true;
                        f0 = p;
                        //点が1つしかない側(front or back)ではelseの処理が行われないので2つの配列要素に代入(後で使う)
                        frontPoints[0] = frontPoints[1] = _targetVertices[p];
                        frontUVs[0] = frontUVs[1] = _targetUVs[p];
                        frontNormals[0] = frontNormals[1] = _targetNormals[p];
                    }
                    else
                    {
                        f1 = p;
                        twoPointsInFront = true;
                        frontPoints[1] = _targetVertices[p];
                        frontUVs[1] = _targetUVs[p];
                        frontNormals[1] = _targetNormals[p];
                    }
                }
                else
                {
                    faceType -= (side);
                    if (!didset_back)
                    {
                        didset_back = true;
                        b0 = p;
                        backPoints[0] = backPoints[1] = _targetVertices[p];
                        backUVs[0] = backUVs[1] = _targetUVs[p];
                        backNormals[0] = backNormals[1] = _targetNormals[p];
                    }
                    else
                    {
                        b1 = p;
                        backPoints[1] = _targetVertices[p];
                        backUVs[1] = _targetUVs[p];
                        backNormals[1] = _targetNormals[p];
                    }
                }
            }

            float normalizedDistance = 0f;
            float distance = 0f;

            //frontPoints[0]からbackPoints[0]に伸びるrayが切断面とぶつかるときの長さを取得
            _slashPlane.Raycast(new Ray(frontPoints[0], (backPoints[0] - frontPoints[0]).normalized), out distance);
            //0～1に変換(0だったらfrontPoints[0], 1だったらbackPoints[0], 0.5だったら中点)
            normalizedDistance = distance / (backPoints[0] - frontPoints[0]).magnitude;
            //Lerpで切断によってうまれる新しい頂点の情報を生成
            Vector3 newVertex1 = Vector3.Lerp(frontPoints[0], backPoints[0], normalizedDistance);
            Vector2 newUV1 = Vector2.Lerp(frontUVs[0], backUVs[0], normalizedDistance);
            Vector3 newNormal1 = Vector3.Lerp(frontNormals[0], backNormals[0], normalizedDistance);


            _slashPlane.Raycast(new Ray(frontPoints[1], (backPoints[1] - frontPoints[1]).normalized), out distance);
            normalizedDistance = distance / (backPoints[1] - frontPoints[1]).magnitude;
            Vector3 newVertex2 = Vector3.Lerp(frontPoints[1], backPoints[1], normalizedDistance);
            Vector2 newUV2 = Vector2.Lerp(frontUVs[1], backUVs[1], normalizedDistance);
            Vector3 newNormal2 = Vector3.Lerp(frontNormals[1], backNormals[1], normalizedDistance);


            if (twoPointsInFront) //切断面の表側に点が2つあるか裏側に2つあるかで分ける
            {
                if (faceType == 1)//入力されてきた頂点の回り方で面を作ると面の向きが外側になるので, 回り方を変えないように打ち込んでいく
                {
                    _frontMeshData.rectangleFragments[submesh].Add(
                    new RectangleFragment(f1, f0, newVertex1, newVertex2, new Vector2[2] { newUV1, newUV2 }, new Vector3[2] { newNormal1, newNormal2 })
                    );
                    _backMeshData.triangleFragments[submesh].Add(
                        new TriangleFragment(b0, newVertex2, newVertex1, new Vector2[2] { newUV2, newUV1 }, new Vector3[2] { newNormal2, newNormal1 })
                        );
                }
                else
                {
                    _frontMeshData.rectangleFragments[submesh].Add(
                    new RectangleFragment(f0, f1, newVertex2, newVertex1, new Vector2[2] { newUV2, newUV1 }, new Vector3[2] { newNormal2, newNormal1 })
                    );
                    _backMeshData.triangleFragments[submesh].Add(
                        new TriangleFragment(b0, newVertex1, newVertex2, new Vector2[2] { newUV1, newUV2 }, new Vector3[2] { newNormal1, newNormal2 })
                        );
                }

            }
            else
            {
                if (faceType == -1)
                {
                    _backMeshData.rectangleFragments[submesh].Add(
                  new RectangleFragment(b1, b0, newVertex1, newVertex2, new Vector2[2] { newUV1, newUV2 }, new Vector3[2] { newNormal1, newNormal2 })
                  );
                    _frontMeshData.triangleFragments[submesh].Add(
                        new TriangleFragment(f0, newVertex2, newVertex1, new Vector2[2] { newUV2, newUV1 }, new Vector3[2] { newNormal2, newNormal1 })
                        );
                }
                else
                {
                    _backMeshData.rectangleFragments[submesh].Add(
                  new RectangleFragment(b0, b1, newVertex2, newVertex1, new Vector2[2] { newUV2, newUV1 }, new Vector3[2] { newNormal2, newNormal1 })
                  );
                    _frontMeshData.triangleFragments[submesh].Add(
                        new TriangleFragment(f0, newVertex1, newVertex2, new Vector2[2] { newUV1, newUV2 }, new Vector3[2] { newNormal1, newNormal2 })
                        );
                }
            }
        }


        private static void SepalateOnce(bool[] sides, int[] vertexIndices, int submesh)
        {
            Vector3[] frontPoints = new Vector3[2];
            Vector3[] frontNormals = new Vector3[2]; //ポリゴンの頂点が3つなのにfront-backで配列が2つずつなのは対象的な処理を行うため
            Vector2[] frontUVs = new Vector2[2];
            Vector3[] backPoints = new Vector3[2];
            Vector3[] backNormals = new Vector3[2];
            Vector2[] backUVs = new Vector2[2];

            bool didset_front = false; //表側に1つ頂点を置いたかどうか
            bool didset_back = false;
            bool twoPointsInFront = false;//表側に点が2つあるか(これがfalseのときは裏側に点が2つある)

            int p = 0;
            int f0 = 0, f1 = 0, b0 = 0, b1 = 0; //頂点のindex番号を格納するのに使用

            int faceType = 0;//面が外側を向くように整列させるために使用

            for (int side = 0; side < 3; side++)
            {
                p = vertexIndices[side];

                if (sides[side])
                {
                    faceType += (side); //これの合計値によって面の割り方の種類を6つに分類できる
                    if (!didset_front)
                    {
                        didset_front = true;
                        f0 = p;
                        //点が1つしかない側(front or back)ではelseの処理が行われないので2つの配列要素に代入(後で使う)
                        frontPoints[0] = frontPoints[1] = _targetVertices[p];
                        frontUVs[0] = frontUVs[1] = _targetUVs[p];
                        frontNormals[0] = frontNormals[1] = _targetNormals[p];
                    }
                    else
                    {
                        f1 = p;
                        twoPointsInFront = true;
                        frontPoints[1] = _targetVertices[p];
                        frontUVs[1] = _targetUVs[p];
                        frontNormals[1] = _targetNormals[p];
                    }
                }
                else
                {
                    faceType -= (side);
                    if (!didset_back)
                    {
                        didset_back = true;
                        b0 = p;
                        backPoints[0] = backPoints[1] = _targetVertices[p];
                        backUVs[0] = backUVs[1] = _targetUVs[p];
                        backNormals[0] = backNormals[1] = _targetNormals[p];
                    }
                    else
                    {
                        b1 = p;
                        backPoints[1] = _targetVertices[p];
                        backUVs[1] = _targetUVs[p];
                        backNormals[1] = _targetNormals[p];
                    }
                }
            }

            float normalizedDistance = 0f;
            float distance = 0f;

            //frontPoints[0]からbackPoints[0]に伸びるrayが切断面とぶつかるときの長さを取得
            _slashPlane.Raycast(new Ray(frontPoints[0], (backPoints[0] - frontPoints[0]).normalized), out distance);
            //0～1に変換(0だったらfrontPoints[0], 1だったらbackPoints[0], 0.5だったら中点)
            normalizedDistance = distance / (backPoints[0] - frontPoints[0]).magnitude;
            //Lerpで切断によってうまれる新しい頂点の情報を生成
            Vector3 newVertex1 = Vector3.Lerp(frontPoints[0], backPoints[0], normalizedDistance);
            Vector2 newUV1 = Vector2.Lerp(frontUVs[0], backUVs[0], normalizedDistance);
            Vector3 newNormal1 = Vector3.Lerp(frontNormals[0], backNormals[0], normalizedDistance);


            _slashPlane.Raycast(new Ray(frontPoints[1], (backPoints[1] - frontPoints[1]).normalized), out distance);
            normalizedDistance = distance / (backPoints[1] - frontPoints[1]).magnitude;
            Vector3 newVertex2 = Vector3.Lerp(frontPoints[1], backPoints[1], normalizedDistance);
            Vector2 newUV2 = Vector2.Lerp(frontUVs[1], backUVs[1], normalizedDistance);
            Vector3 newNormal2 = Vector3.Lerp(frontNormals[1], backNormals[1], normalizedDistance);

            if (twoPointsInFront) //切断面の表側に点が2つあるか裏側に2つあるかで分ける
            {
                if (faceType == 1)//入力されてきた頂点の回り方で面を作ると面の向きが外側になるので, 回り方を変えないように打ち込んでいく
                {
                    _frontMeshData.AddTriangle(f1, f0, newVertex1, newNormal1, newUV1, submesh);
                    _frontMeshData.AddTriangle(f1, newVertex1, newVertex2, new Vector3[2] { newNormal1, newNormal2 }, new Vector2[2] { newUV1, newUV2 }, submesh);
                    _backMeshData.AddTriangle(b0, newVertex2, newVertex1, new Vector3[2] { newNormal2, newNormal1 }, new Vector2[2] { newUV2, newUV1 }, submesh);
                }
                else
                {
                    _frontMeshData.AddTriangle(f0, f1, newVertex1, newNormal1, newUV1, submesh);
                    _frontMeshData.AddTriangle(f1, newVertex2, newVertex1, new Vector3[2] { newNormal2, newNormal1 }, new Vector2[2] { newUV2, newUV1 }, submesh);
                    _backMeshData.AddTriangle(b0, newVertex1, newVertex2, new Vector3[2] { newNormal1, newNormal2 }, new Vector2[2] { newUV1, newUV2 }, submesh);
                }

            }
            else
            {
                if (faceType == -1)
                {
                    _backMeshData.AddTriangle(b1, b0, newVertex1, newNormal1, newUV1, submesh);
                    _backMeshData.AddTriangle(b1, newVertex1, newVertex2, new Vector3[2] { newNormal1, newNormal2 }, new Vector2[2] { newUV1, newUV2 }, submesh);
                    _frontMeshData.AddTriangle(f0, newVertex2, newVertex1, new Vector3[2] { newNormal2, newNormal1 }, new Vector2[2] { newUV2, newUV1 }, submesh);
                }
                else
                {
                    _backMeshData.AddTriangle(b0, b1, newVertex1, newNormal1, newUV1, submesh);
                    _backMeshData.AddTriangle(b1, newVertex2, newVertex1, new Vector3[2] { newNormal2, newNormal1 }, new Vector2[2] { newUV2, newUV1 }, submesh);
                    _frontMeshData.AddTriangle(f0, newVertex1, newVertex2, new Vector3[2] { newNormal1, newNormal2 }, new Vector2[2] { newUV1, newUV2 }, submesh);
                }
            }

        }


        public class MeshData
        {
            public List<Vector3> vertices = new List<Vector3>();
            public Vector3[] verticesArray;
            public List<Vector3> normals = new List<Vector3>();
            public List<Vector2> uvs = new List<Vector2>();
            public List<int> triangles = new List<int>();
            public List<List<int>> subMeshIndices = new List<List<int>>();

            public List<List<TriangleFragment>> triangleFragments = new List<List<TriangleFragment>>();
            public List<List<RectangleFragment>> rectangleFragments = new List<List<RectangleFragment>>();


            int[] trackedArray; //_targetVerticesとverticesの対応をとっている
            int trackedVertexNum = 0; //登録された頂点の数


            //すでに登録された頂点かどうかを判断(同じ頂点が複数の三角面を構成しているときはこれがないと三角面の数だけ頂点が増えてしまう)
            bool CheckIndex(int target)
            {
                if ((trackNum = trackedArray[target]) != 0)
                {
                    return true;
                }
                else
                {
                    trackedArray[target] = trackedVertexNum;

                    return false;
                }
            }

            private int trackNum;

            public void AddTriangle(int p1, int p2, int p3, int submeshNum)
            {

                if (CheckIndex(p1))
                {
                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);

                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[p1]);
                    normals.Add(_targetNormals[p1]);
                    uvs.Add(_targetUVs[p1]);
                    trackedVertexNum++;
                }


                if (CheckIndex(p2))
                {
                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);
                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[p2]);
                    normals.Add(_targetNormals[p2]);
                    uvs.Add(_targetUVs[p2]);
                    trackedVertexNum++;
                }

                if (CheckIndex(p3))
                {
                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);
                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[p3]);
                    normals.Add(_targetNormals[p3]);
                    uvs.Add(_targetUVs[p3]);
                    trackedVertexNum++;
                }
            }

            const float threshold = 0.0001f;
            public void ConnectFragments()
            {
                //三角形と四角形で同一平面にあるやつをくっつける.(ここではくっつけるだけで出力するのは最後)
                for (int sub = 0; sub < subMeshIndices.Count; sub++)
                {

                    for (int i = 0; i < rectangleFragments[sub].Count; i++)
                    {
                        var rectangle = rectangleFragments[sub][i];

                        for (int j = 0; j < triangleFragments[sub].Count; j++)
                        {
                            var triangle = triangleFragments[sub][j];

                            if (Mathf.Abs(Vector3.Dot(triangle.cutLine, rectangle.cutLine)) > 1 - threshold)
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    for (int m = 0; m < 2; m++)
                                    {
                                        if (triangle.cutPoints[n] == rectangle.cutPoints[m])
                                        {

                                            if (m == 0) //面を貼る向きを決める
                                            {
                                                rectangle.cutPoints = new Vector3[2] { triangle.cutPoints[0], rectangle.cutPoints[1] };
                                                rectangle.normals = new Vector3[2] { triangle.normals[0], rectangle.normals[1] };
                                                rectangle.uvs = new Vector2[2] { triangle.uvs[0], rectangle.uvs[1] };
                                            }
                                            else
                                            {
                                                rectangle.cutPoints = new Vector3[2] { rectangle.cutPoints[0], triangle.cutPoints[1] };
                                                rectangle.normals = new Vector3[2] { rectangle.normals[0], triangle.normals[1] };
                                                rectangle.uvs = new Vector2[2] { rectangle.uvs[0], triangle.uvs[1] };
                                            }
                                            triangleFragments[sub].RemoveAt(j);
                                            j -= 1;//リストの配列でループしてるので配列を消した場合は辻褄を合わせる
                                            goto CONNECTE;
                                        }
                                    }
                                }
                            }
                        CONNECTE:;
                        }
                    }


                    //三角形同士で同一平面にあるやつをくっつける.(ここではくっつけるだけで出力するのは最後)
                    for (int first = 0; first < triangleFragments[sub].Count; first++)
                    {
                        var fTriangle = triangleFragments[sub][first];
                        for (int second = first + 1; second < triangleFragments[sub].Count; second++)
                        {
                            var sTriangle = triangleFragments[sub][second];
                            if (Mathf.Abs(Vector3.Dot(fTriangle.cutLine, sTriangle.cutLine)) > 1 - threshold)
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    for (int m = 0; m < 2; m++)
                                    {
                                        if (fTriangle.cutPoints[n] == sTriangle.cutPoints[m])
                                        {
                                            if (n == 0)
                                            {
                                                fTriangle.cutPoints[0] = sTriangle.cutPoints[0];
                                                fTriangle.normals[0] = sTriangle.normals[0];
                                                fTriangle.uvs[0] = sTriangle.uvs[0];
                                            }
                                            else
                                            {
                                                fTriangle.cutPoints[1] = sTriangle.cutPoints[1];
                                                fTriangle.normals[1] = sTriangle.normals[1];
                                                fTriangle.uvs[1] = sTriangle.uvs[1];
                                            }

                                            triangleFragments[sub].RemoveAt(second);
                                            second -= 1;

                                            goto CONNECT2;
                                        }
                                    }
                                }
                            }
                        CONNECT2:;
                        }
                    }


                    //四角形同士で同一平面にあるやつをくっつける.(四角形をくっつけると五角形になってしまうのでこれはその場で出力する)
                    for (int first = 0; first < rectangleFragments[sub].Count; first++)
                    {
                        RectangleFragment fRectangle = rectangleFragments[sub][first];
                        for (int second = first + 1; second < rectangleFragments[sub].Count; second++)
                        {
                            RectangleFragment sRectangle = rectangleFragments[sub][second];
                            if (Mathf.Abs(Vector3.Dot(fRectangle.cutLine, sRectangle.cutLine)) > 1 - threshold)
                            {
                                for (int n = 0; n < 2; n++)
                                {
                                    for (int m = 0; m < 2; m++)
                                    {
                                        if (fRectangle.cutPoints[n] == sRectangle.cutPoints[m])
                                        {
                                            if (n == 0)
                                            {
                                                AddTriangle(
                                                    fRectangle.notCutPointIndecies[0],
                                                    fRectangle.notCutPointIndecies[1],
                                                    fRectangle.cutPoints[1],
                                                    fRectangle.normals[1],
                                                    fRectangle.uvs[1],
                                                    sub
                                                    );
                                                AddTriangle(
                                                    sRectangle.notCutPointIndecies[0],
                                                    sRectangle.notCutPointIndecies[1],
                                                    sRectangle.cutPoints[0],
                                                    sRectangle.normals[0],
                                                    sRectangle.uvs[0],
                                                    sub
                                                    );
                                                AddTriangle(
                                                    fRectangle.notCutPointIndecies[1],
                                                    sRectangle.cutPoints[0],
                                                    fRectangle.cutPoints[1],
                                                    new Vector3[2] { sRectangle.normals[0], fRectangle.normals[1] },
                                                    new Vector2[2] { sRectangle.uvs[0], fRectangle.uvs[1] },
                                                    sub
                                                    );
                                            }
                                            else
                                            {
                                                AddTriangle(
                                                    sRectangle.notCutPointIndecies[0],
                                                    sRectangle.notCutPointIndecies[1],
                                                    sRectangle.cutPoints[1],
                                                    sRectangle.normals[1],
                                                    sRectangle.uvs[1],
                                                    sub
                                                    );
                                                AddTriangle(
                                                    fRectangle.notCutPointIndecies[0],
                                                    fRectangle.notCutPointIndecies[1],
                                                    fRectangle.cutPoints[0],
                                                    fRectangle.normals[0],
                                                    fRectangle.uvs[0],
                                                    sub
                                                    );
                                                AddTriangle(
                                                    sRectangle.notCutPointIndecies[1],
                                                    fRectangle.cutPoints[0],
                                                    sRectangle.cutPoints[1],
                                                    new Vector3[2] { fRectangle.normals[0], sRectangle.normals[1] },
                                                    new Vector2[2] { fRectangle.uvs[0], sRectangle.uvs[1] },
                                                    sub
                                                    );
                                            }

                                            rectangleFragments[sub].RemoveAt(second);
                                            rectangleFragments[sub].RemoveAt(first);
                                            first -= 1;

                                            goto FINDFRAGMENT3;
                                        }
                                    }
                                }
                            }
                        }
                    FINDFRAGMENT3:;
                    }

                    //最後にペアを作れなかったやつらを追加して終了
                    foreach (TriangleFragment triangle in triangleFragments[sub])
                    {
                        AddTriangle(
                            triangle.notCutPointIndecies,
                            triangle.cutPoints[0],
                            triangle.cutPoints[1],
                            triangle.normals,
                            triangle.uvs,
                            sub
                            );
                    }

                    foreach (RectangleFragment rectangle in rectangleFragments[sub])
                    {
                        AddTriangle(
                            rectangle.notCutPointIndecies[0],
                            rectangle.notCutPointIndecies[1],
                            rectangle.cutPoints[0],
                            rectangle.normals[0],
                            rectangle.uvs[0],
                            sub
                            );
                        AddTriangle(
                            rectangle.notCutPointIndecies[0],
                            rectangle.cutPoints[0],
                            rectangle.cutPoints[1],
                            rectangle.normals,
                            rectangle.uvs,
                            sub
                            );
                    }

                }
            }

            public void AddTriangle(int notCutIndex0, int notCutIndex1, Vector3 cutPoint, Vector3 normal, Vector2 uv, int submeshNum)
            {

                if (CheckIndex(notCutIndex0))
                {

                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);

                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[notCutIndex0]);
                    normals.Add(_targetNormals[notCutIndex0]);
                    uvs.Add(_targetUVs[notCutIndex0]);
                    trackedVertexNum++;

                }

                if (CheckIndex(notCutIndex1))
                {
                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);

                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[notCutIndex1]);
                    normals.Add(_targetNormals[notCutIndex1]);
                    uvs.Add(_targetUVs[notCutIndex1]);
                    trackedVertexNum++;

                }

                subMeshIndices[submeshNum].Add(trackedVertexNum);
                triangles.Add(trackedVertexNum);
                vertices.Add(cutPoint);
                normals.Add(normal);
                uvs.Add(uv);
                trackedVertexNum++;
            }

            public void AddTriangle(int notCutIndex, Vector3 cutPoint1, Vector3 cutPoint2, Vector3[] normals3, Vector2[] uvs3, int submeshNum)
            {

                if (CheckIndex(notCutIndex))
                {
                    subMeshIndices[submeshNum].Add(trackNum);
                    triangles.Add(trackNum);
                }
                else
                {
                    subMeshIndices[submeshNum].Add(trackedVertexNum);
                    triangles.Add(trackedVertexNum);
                    vertices.Add(_targetVertices[notCutIndex]);
                    normals.Add(_targetNormals[notCutIndex]);
                    trackedVertexNum++;
                    uvs.Add(_targetUVs[notCutIndex]);
                }

                subMeshIndices[submeshNum].Add(trackedVertexNum);
                triangles.Add(trackedVertexNum);
                vertices.Add(cutPoint1);
                normals.Add(normals3[0]); //normal配列には新しい頂点の2つ分しか入ってない
                uvs.Add(uvs3[0]);
                trackedVertexNum++;

                subMeshIndices[submeshNum].Add(trackedVertexNum);
                triangles.Add(trackedVertexNum);
                vertices.Add(cutPoint2);
                normals.Add(normals3[1]);
                uvs.Add(uvs3[1]);
                trackedVertexNum++;

            }

            public void ClearAll()
            {
                vertices.Clear();
                normals.Clear();
                uvs.Clear();
                triangles.Clear();
                subMeshIndices.Clear();

                //trackedIndex.Clear();
                triangleFragments.Clear();
                rectangleFragments.Clear();

                trackedArray = new int[_targetVertices.Length];
                trackedVertexNum = 0;

            }

        }

        //三角ポリゴンを分割するとき, 三角になったほうをTriangleFragmentに, 四角になったほうをRectangleFragmentにいれている
        //これらは分割中はListにためておくだけにして, 最後にくっつけられるもの(同一平面にあるやつ)はくっつけて出力

        public class TriangleFragment
        {
            public Vector3 cutLine; //ポリゴンを分けたときの線
            public Vector3[] cutPoints;
            public int notCutPointIndecies;
            public Vector3[] normals;
            public Vector2[] uvs;
            public TriangleFragment(int notCutPoint1Index, Vector3 cutPoint1, Vector3 cutPoint2, Vector2[] uvs, Vector3[] normals)
            {
                cutLine = (cutPoint1 - cutPoint2).normalized;
                cutPoints = new Vector3[2] { cutPoint1, cutPoint2 };
                this.uvs = uvs;
                this.normals = normals;
                notCutPointIndecies = notCutPoint1Index;
            }
        }

        public class RectangleFragment
        {
            public Vector3 cutLine; //ポリゴンを分けたときの線
            public Vector3[] cutPoints;
            public int[] notCutPointIndecies;
            public Vector3[] normals;
            public Vector2[] uvs;
            public RectangleFragment(int notCutPoint1Index, int notCutPoint2Index, Vector3 cutPoint1, Vector3 cutPoint2, Vector2[] uvs, Vector3[] normals)
            {
                cutLine = (cutPoint1 - cutPoint2).normalized;
                cutPoints = new Vector3[2] { cutPoint1, cutPoint2 };
                this.uvs = uvs;
                this.normals = normals;
                notCutPointIndecies = new int[2] { notCutPoint1Index, notCutPoint2Index };
            }
        }

    }




}