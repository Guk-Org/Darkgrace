using Mirror;
using UnityEngine;

public class DestructiblePlane : NetworkBehaviour
{
    private MeshFilter filter;

    private void Start()
    {
        filter = GetComponent<MeshFilter>();

        Bounds bounds = GetComponent<Renderer>().bounds;
        float width = bounds.size.x;
        float height = bounds.size.z; // plane lies on XZ

        float halfW = width / 2f;
        float halfH = height / 2f;

        Vector3[] newVertices =
        {
            new Vector3(-halfW, 0, -halfH),
            new Vector3( halfW, 0, -halfH),
            new Vector3( halfW, 0,  halfH),
            new Vector3(-halfW, 0,  halfH)
        };

        Vector2[] newUv =
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1)
        };

        int[] newTriangles = { 0, 2, 1, 0, 3, 2 };

        Mesh mesh = new Mesh();
        mesh.vertices = newVertices;
        mesh.uv = newUv;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        filter.mesh = mesh;
    }

    public void Destruct(Vector2 pos)
    {

    }


}
