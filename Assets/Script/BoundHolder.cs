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
    private bool isActive;

    public Dictionary<int, int> dictionary;

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        partialVertices = new List<Vector3>();
        partialNormals = new List<Vector3>();
        partialColoar = new List<Color>();
        partialUVs = new List<Vector2>();
        partialTriangles = new List<int>();
        isActive = false;
        dictionary = new Dictionary<int, int>();
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

    public void AddVertexAndPopulateDic(Vector3 vertex, Vector3[] originalVertices)
    {
        partialVertices.Add(vertex);
        int originalVertexIndex = System.Array.IndexOf(originalVertices, vertex);
        while (dictionary.ContainsKey(originalVertexIndex))
            originalVertexIndex++;
        dictionary.Add(originalVertexIndex, partialVertices.Count - 1);
    }

    public void AddNormals(Vector3 normal)
    {
        partialNormals.Add(normal);
    }

    public void AddColor(Color color)
    {
        partialColoar.Add(color);
    }

    public void AddUV(Vector2 uv)
    {
        partialUVs.Add(uv);
    }

    public void AddTriangle(int i, int j, int k)
    {
        partialTriangles.Add(i);
        partialTriangles.Add(j);
        partialTriangles.Add(k);
    }

    public List<Vector3> GetVertices()
    {
        return partialVertices;
    }

    public List<int> GetTriangles()
    {
        return partialTriangles;
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

    public void RegenerateTriangle()
    {
        List<int> newTriangle = new List<int>();
        for (int index = 0; index < partialTriangles.Count; index++)
        {
            //if (!dictionary.ContainsKey(partialTriangles[index])) break;
            int newIndex = dictionary[partialTriangles[index]];
            newTriangle.Add(newIndex);
        }
        partialTriangles.Clear();
        partialTriangles = newTriangle;
        //for (int i = 0; i < partialTriangles.Count; i++)
        //{
        //    Debug.Log("partialTriangles[" + i + "]: " + partialTriangles[i] /*+ " newTriangles[" + i + "]: " + newTriangle[i]*/);
        //}
    }
}