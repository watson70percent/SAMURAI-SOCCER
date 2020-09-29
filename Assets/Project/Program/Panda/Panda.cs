using UnityEngine;

public class Panda : MonoBehaviour
{
    [SerializeField] private float m_expandAmount = 100.0f;
    [SerializeField] private float speed;

    private void Start()
    {
        var meshFilter = GetComponent<MeshFilter>();

        var bounds = meshFilter.mesh.bounds;
        bounds.size = Vector3.one * m_expandAmount;
        //bounds.Expand(m_expandAmount);
        meshFilter.mesh.bounds = bounds;
    }

    private void Update()
    {
        var pos = transform.position;
        pos.y -= speed * Time.deltaTime;
        transform.position = pos;

        if (pos.y < -50) { Destroy(gameObject); }
    }

    private void OnTriggerEnter(Collider other)
    {
        
    }

} 