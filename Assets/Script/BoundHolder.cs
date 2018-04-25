using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundHolder
{
    public Bounds subBound;

    private List<Vector3> partialVertices;
    private List<Vector3> partialNormals;
    private List<Color> partialColoar;
    private List<Vector2> partialUVs;
    private List<int> partialTriangles;
    private List<int> newTriangles;
    //private int[] newTriangles;
    private bool isActive;

    private VertexLookUp map;

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        partialVertices = new List<Vector3>();
        partialNormals = new List<Vector3>();
        partialColoar = new List<Color>();
        partialUVs = new List<Vector2>();
        partialTriangles = new List<int>();
        newTriangles = new List<int>();
        isActive = false;
        map = new VertexLookUp();
    }

    public Bounds GetBounds()
    {
        return subBound;
    }

    public bool GetStatus()
    {
        return isActive;
    }

    public void SetStatus(bool status)
    {
        isActive = status;
    }

    public bool CheckIntersects(Vector3 point)
    {
        if (subBound.Contains(point))
        {
            return true;
        }
        return false;
    }

    public void ConstructMesh(Mesh m)
    {
        List<int> checkRedundant = new List<int>();
        for (int triangleIndex = 0; triangleIndex < partialTriangles.Count; triangleIndex++)
        {
            int vertexIndex = partialTriangles[triangleIndex];
            if (checkRedundant.Contains(vertexIndex))
                continue;
            checkRedundant.Add(vertexIndex);
            partialVertices.Add(m.vertices[vertexIndex]);
            partialNormals.Add(m.normals[vertexIndex]);
            if (m.colors.Length != 0)
                partialColoar.Add(m.colors[vertexIndex]);

            if (m.uv.Length != 0)
                partialUVs.Add(m.uv[vertexIndex]);
        }
    }

    public void AddTriangle(int[] triangle)
    {
        for (int index = 0; index < triangle.Length; index++)
        {
            partialTriangles.Add(triangle[index]);
            if (!map.IsOldIndexInLookUp(triangle[index]))
                map.AddOldIndex(triangle[index]);
            int newIndex = map.FindValue(triangle[index]);
            newTriangles.Add(newIndex);
        }
    }

    public List<Vector3> GetVertices()
    {
        return partialVertices;
    }

    public List<int> GetTriangles()
    {
        return newTriangles;
    }

    public List<Vector3> GetNormals()
    {
        return partialNormals;
    }

    public List<Color> GetColors()
    {
        return partialColoar;
    }

    public List<Vector2> GetUVs()
    {
        return partialUVs;
    }
}