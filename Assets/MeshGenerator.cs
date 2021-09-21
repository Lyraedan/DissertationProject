using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public int resolution = 10;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> tris = new List<int>();
    public List<Vector3> normals = new List<Vector3>();
    public List<Vector2> uvs = new List<Vector2>();

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        meshFilter = gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();

        GeneratePlane(0, 0);
    }

    private void OnValidate()
    {
        if(mesh != null)
            Refresh();
    }

    public void GeneratePlane(float xOff, float zOff)
    {
        int index = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {
                // Generate vertices
                Vector3 topLeft = new Vector3(xOff + x, 0, zOff + z);
                Vector3 topRight = new Vector3((xOff + x) + 1, 0, (zOff + z));
                Vector3 bottomLeft = new Vector3((xOff + x), 0, (zOff + z) + 1);
                Vector3 bottomRight = new Vector3((xOff + x) + 1, 0, (zOff + z) + 1);

                vertices.Add(topLeft);
                vertices.Add(topRight);
                vertices.Add(bottomLeft);
                vertices.Add(bottomRight);

                // Generate triangles
                tris.Add(index);
                tris.Add(index + 2);
                tris.Add(index + 1);

                tris.Add(index + 1);
                tris.Add(index + 2);
                tris.Add(index + 3);

                // Generate normals
                Vector3 normalA = CalculateNormal(topLeft, bottomLeft, bottomRight);
                Vector3 normalB = CalculateNormal(topLeft, bottomRight, topRight);

                normals.Add(normalA);
                normals.Add(normalA);
                normals.Add(normalA);
                normals.Add(normalB);

                // Generate UVs
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                index += 4;
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        meshFilter.mesh = mesh;
    }

    public void Refresh()
    {
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uvs.ToArray();

        meshFilter.mesh = mesh;
    }

    public Vector3 CalculateNormal(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = new Vector3(p2.x - p1.x, p2.y - p1.y, p2.z - p1.z);
        Vector3 b = new Vector3(p3.x - p1.x, p3.y - p1.y, p3.z - p1.z);

        Vector3 normal = new Vector3(a.y * b.z - a.z * b.y,
                                     a.z * b.x - a.x * b.z,
                                     a.x * b.y - a.y * b.x);
        return normal.normalized;
    }

    public void UpdateVerticeY(int x, int z, float value)
    {
        int index = x + z * resolution;
        float vertX = vertices[index].x;
        float vertZ = vertices[index].z;
        vertices[index].Set(vertX, value, vertZ);
    }
}