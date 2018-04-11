using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHolder {
    private List<Vector3> totalVertices;
    private List<Vector3> totalNormals;
    private List<Color> totalColors;
    private List<int> totalTriangle;
    private List<Vector2> totalUVs;

    private Transform trans;

    public MeshHolder()
    {
        totalVertices = new List<Vector3>();
        totalNormals = new List<Vector3>();
        totalColors = new List<Color>();
        totalTriangle = new List<int>();
        totalUVs = new List<Vector2>();
    }

    public int CombineMeshElement(Vector3[] vertices, Vector3[] normals, Color[] colors, int[] triangles, int arrayCount, Vector2[] uvs)
    {
        totalVertices.AddRange(vertices);
        totalNormals.AddRange(normals);
        totalColors.AddRange(colors);

        int[] tempIndex = triangles;
        int[] tempIndex2 = new int[tempIndex.Length];
        for (int i = 0; i < tempIndex.Length; i++)
        {
            tempIndex2[i] = tempIndex[i] + arrayCount;
        }
        totalTriangle.AddRange(tempIndex2);
        arrayCount = arrayCount + vertices.Length;

        totalUVs.AddRange(uvs);

        return arrayCount;
    }

    public Vector3[] GetVertices()
    {
        return totalVertices.ToArray();
    }

    public Vector3[] GetNormals()
    {
        return totalNormals.ToArray();
    }

    public Color[] GetColors()
    {
        return totalColors.ToArray();
    }

    public int[] GetTriangles()
    {
        return totalTriangle.ToArray();
    }

    public Vector2[] GetUVs()
    {
        return totalUVs.ToArray();
    }
}
