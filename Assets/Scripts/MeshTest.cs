using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

namespace SamuraiSoccer
{
    public class MeshTest : MonoBehaviour
    {
        DateTime start;
        bool isCut = false;
        // Start is called before the first frame update
        void Start()
        {
            start = DateTime.Now;
        }

        private void Update()
        {
            if (!isCut && (DateTime.Now - start).TotalSeconds > 5)
            {
                _ = Cut();
                isCut = true;
            }
        }

        async UniTask Cut()
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            var meshes = await BurstMeshCut.CutMesh(gameObject, Vector3.zero, Vector3.left);
            sw.Stop();
            Debug.Log("t = "  + sw.Elapsed);
            GetComponent<MeshFilter>().mesh = meshes[1];

            GameObject fragment = new GameObject("fragment", typeof(MeshFilter), typeof(MeshRenderer));
            fragment.transform.SetPositionAndRotation(transform.position, transform.rotation);
            fragment.transform.localScale = transform.localScale;
            fragment.GetComponent<MeshFilter>().mesh = meshes[0];
            fragment.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
        }
    }
}
