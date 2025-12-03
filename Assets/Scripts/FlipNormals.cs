using UnityEngine;

public class FlipNormals : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;

        // 1. Flip Normals
        for (int i = 0; i < normals.Length; i++)
            normals[i] = -normals[i];

        // 2. Reverse Triangle Winding (so the renderer sees the inside)
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i];
            triangles[i] = triangles[i + 1];
            triangles[i + 1] = temp;
        }

        mesh.normals = normals;
        mesh.triangles = triangles;
        
        // 3. Update Collider
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
}