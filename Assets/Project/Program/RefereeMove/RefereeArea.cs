using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefereeArea : MonoBehaviour
{
        public float maxang = 60;
        public int size = 10; 
    // Start is called before the first frame update
    void Start()
    {
        //MeshMaker();
    }

    // Update is called once per frame
    void Update()
    {
        DynamicMeshMaker();
    }

    void MeshMaker()
    {

        var mesh = new Mesh();
        var verticles = new List<Vector3> {Vector3.zero };

        float deltaang = maxang / 30;
        float ang;
        int count=0;

        for (ang = -maxang; ang <= maxang; ang += deltaang) {

            count++;
            var vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI))*size;
            verticles.Add(vec);
        }
        mesh.SetVertices(verticles);

        int[] triangles = new int[(count - 1) * 3];
        for(int i = 0; i < count-1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i+2;
        }
            mesh.SetTriangles(triangles, 0);

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    void DynamicMeshMaker()
    {
        var mesh = new Mesh();
        var verticles = new List<Vector3> { Vector3.zero };

        float deltaang = maxang / 80;
        float ang;
        int count = 0;

        for (ang = -maxang; ang <= maxang; ang += deltaang)
        {

            count++;
            var vec =new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI));
            Vector3 vec2;
            Ray ray = new Ray(transform.position,transform.TransformDirection(vec));
           // Debug.DrawRay(transform.position, transform.TransformDirection(vec));
            RaycastHit hit;
            vec2=(Physics.Raycast(ray,out hit,size)) ? vec*hit.distance:vec*size;
            
            verticles.Add(vec2);
        }
        mesh.SetVertices(verticles);

        int[] triangles = new int[(count - 1) * 3];
        for (int i = 0; i < count - 1; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }
        mesh.SetTriangles(triangles, 0);

        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
