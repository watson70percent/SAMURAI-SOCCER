using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RefereeArea : MonoBehaviour
{
    public float maxang = 60;
    public int areaSize = 10;
    public AnimationController anicom;
    public ParticleSystem surprisedMark;
    public Button attackButton;
    // Start is called before the first frame update
    void Start()
    {
        //MeshMaker();
        anicom.AttackEvent += FoulCheck;
    }

    // Update is called once per frame
    void Update()
    {
        DynamicMeshMaker();
    }

    //これは今は使ってない
    void MeshMaker()
    {

        var mesh = new Mesh();
        var verticles = new List<Vector3> {Vector3.zero };

        float deltaang = maxang / 30;
        float ang;
        int count=0;

        for (ang = -maxang; ang <= maxang; ang += deltaang) {

            count++;
            var vec = new Vector3(Mathf.Sin(ang / 360 * 2 * Mathf.PI), 0, Mathf.Cos(ang / 360 * 2 * Mathf.PI))*areaSize;
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
            vec2=(Physics.Raycast(ray,out hit,areaSize)) ? vec*hit.distance:vec*areaSize;
            
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

    void FoulCheck(object sender, System.EventArgs e)
    {

        
        var vec = ((AnimationController)sender).transform.position - transform.position;
        if (vec.magnitude > areaSize || Vector3.Dot(vec.normalized, transform.forward) < Mathf.Cos(maxang / 360 * 2 * Mathf.PI))  return;
        Ray ray = new Ray(transform.position, vec);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit,areaSize) ? (hit.collider.tag=="Player") : false)
        {
            surprisedMark.Play();
            attackButton.enabled = false;
            Invoke("PenaltyRemoval", 1);
        }
       
    }
    void PenaltyRemoval()
    {
        attackButton.enabled = true;
    }
}
