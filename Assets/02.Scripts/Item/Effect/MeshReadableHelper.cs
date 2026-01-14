using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[DefaultExecutionOrder(-100)] // Outline보다 먼저 실행
public class MeshReadableHelper : MonoBehaviour
{
    private void Awake()
    {
        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            meshFilter.mesh = Instantiate(meshFilter.sharedMesh);
        }
    }
}
