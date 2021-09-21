using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public int resolution = 16;

    [HideInInspector] public List<Vector3> vertices = new List<Vector3>();
    [HideInInspector] public List<int> tris = new List<int>();
    [HideInInspector] public List<Vector3> normals = new List<Vector3>();
    [HideInInspector] public List<Vector2> uvs = new List<Vector2>();

    private Mesh mesh;
    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;

    public void Start()
    {
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));

        meshFilter = gameObject.AddComponent<MeshFilter>();

        mesh = new Mesh();

        GeneratePlane();
    }

    public void GeneratePlane()
    {
        int index = 0;
        for (int z = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++)
            {

                // Generate vertices
                vertices.Add(new Vector3(x, 0, z));
                vertices.Add(new Vector3(x + 1, 0, z));
                vertices.Add(new Vector3(x, 0, z + 1));
                vertices.Add(new Vector3(x + 1, 0, z + 1));

                // Generate triangles
                tris.Add(index);
                tris.Add(index + 2);
                tris.Add(index + 1);

                tris.Add(index + 1);
                tris.Add(index + 2);
                tris.Add(index + 3);

                // Generate normals
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);
                normals.Add(-Vector3.forward);

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

}
