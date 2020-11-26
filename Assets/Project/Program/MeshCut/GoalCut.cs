using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalCut : MonoBehaviour
{

    bool cutted;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.transform.root.name);
        if (!cutted)
        {
            if (other.transform.root.tag == "Slash")
            {
                Cut();
                cutted = true;
            }
        }
    }

    void Cut()
    {
        Mesh[] meshes;
        Vector3 vec = Vector3.up * 3;
        meshes = MeshCut.CutMesh(this.gameObject, transform.position+ vec, Vector3.up);
        GetComponent<MeshFilter>().mesh = meshes[1];

        GameObject fragment = new GameObject("fragment",typeof(MeshFilter),typeof(MeshRenderer));
        fragment.transform.position = this.transform.position;
        fragment.transform.rotation = this.transform.rotation;
        fragment.transform.localScale = this.transform.localScale;
        fragment.GetComponent<MeshFilter>().mesh = meshes[0];
        fragment.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
        Destroy(fragment.GetComponent<BoxCollider>());

        fragment.AddComponent<Rigidbody>().AddForce(new Vector3(0,0.6f,0) * 20, ForceMode.Impulse);
        

    }
}
