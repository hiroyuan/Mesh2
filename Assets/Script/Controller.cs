using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Controller : MonoBehaviour {
    public Button btn;
    public Button btn2;
    public Transform trans;
    public Bounds bounds;
    public Mesh mesh;
    public BoundHolder[] subBounds;
    public List<GameObject> list = new List<GameObject>();
    public GameObject indicator;

    public int xAxisSplitter;
    public int yAxisSplitter;
    public int zAxisSplitter;

    // key = old vertex index, value = new vertex index
    public Dictionary<int, int> oldVertexIndices2newVertexIndices;

    public HashSet<Triangle> triangleHash;

    private int xDirCount;
    private int yDirCount;
    private int zDirCount;
    private List<Vector3> partialVertices = new List<Vector3>();
    private List<Vector3> partialNormals = new List<Vector3>();
    private List<Color> partialColors = new List<Color>();
    private List<int> partialTriangles = new List<int>();
    private List<Vector2> partialUVs = new List<Vector2>();

    // Use this for initialization
    void Start () {
        btn.onClick.AddListener(TaskOnClick);
        btn2.onClick.AddListener(TaskOnClick2);
        list.Add(GameObject.Find("Sphere"));
        bounds = DrawBoundingBox(list);
        xDirCount = xAxisSplitter;
        yDirCount = yAxisSplitter;
        zDirCount = zAxisSplitter;

        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
        mesh = GameObject.Find("Sphere").GetComponent<MeshFilter>().mesh;

        oldVertexIndices2newVertexIndices = new Dictionary<int, int>();
        triangleHash = new HashSet<Triangle>();
    }
	
	// Update is called once per frame
	void Update () {
        xDirCount = xAxisSplitter;
        yDirCount = yAxisSplitter;
        zDirCount = zAxisSplitter;
        subBounds = new BoundHolder[xDirCount * yDirCount * zDirCount];
        SplitBounds();
        ActivateSubBoundsByIndicator();
    }

    void TaskOnClick()
    {
        Debug.Log("You have clicked the button!");
        SplitMesh();
    }

    void TaskOnClick2()
    {
        Load();
    }

    private void OnDrawGizmos()
    {
        if (subBounds != null)
        {
            for (int i = 0; i < subBounds.Length; i++)
            {
                Gizmos.DrawWireCube(subBounds[i].GetBounds().center, subBounds[i].GetBounds().size);
            }
        }
    }

    /// <summary>
    /// This function finds the bounding box for all game objects in the List
    /// passed into the parameter.
    /// Referenced from Unity community and API.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="l"></param>
    /// <returns>bounds</returns>
    public static Bounds DrawBoundingBox(List<GameObject> l)
    {
        if (l.Count == 0)
        {
            return new Bounds(Vector3.zero, Vector3.one);
        }

        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;
        float minZ = Mathf.Infinity;
        float maxZ = -Mathf.Infinity;

        Vector3[] points = new Vector3[8];

        foreach (GameObject go in l)
        {
            getBoundsPointsNoAlloc(go, points);
            foreach (Vector3 v in points)
            {
                if (v.x < minX) minX = v.x;
                if (v.x > maxX) maxX = v.x;
                if (v.y < minY) minY = v.y;
                if (v.y > maxY) maxY = v.y;
                if (v.z < minZ) minZ = v.z;
                if (v.z > maxZ) maxZ = v.z;
            }
        }

        float sizeX = maxX - minX;
        float sizeY = maxY - minY;
        float sizeZ = maxZ - minZ;

        Vector3 center = new Vector3(minX + sizeX / 2.0f, minY + sizeY / 2.0f, minZ + sizeZ / 2.0f);

        return new Bounds(center, new Vector3(sizeX, sizeY, sizeZ));
    }

    public void SplitBounds()
    {
        float sizeX = (bounds.max.x - bounds.min.x) / xDirCount;
        float sizeY = (bounds.max.y - bounds.min.y) / yDirCount;
        float sizeZ = (bounds.max.z - bounds.min.z) / zDirCount;
        Vector3 subSize = new Vector3(sizeX, sizeY, sizeZ);

        Vector3 subCenter;
        int index = 0;
        float xStartPoint = bounds.min.x;
        float yStartPoint = bounds.min.y;
        float zStartPoint = bounds.min.z;
        for (int i = 0; i < xDirCount; i++)
        {
            for (int j = 0; j < yDirCount; j++)
            {
                for (int k = 0; k < zDirCount; k++)
                {
                    subCenter = new Vector3(xStartPoint + sizeX / 2, yStartPoint + sizeY / 2, zStartPoint + sizeZ / 2);
                    subBounds[index++] = new BoundHolder(subCenter, subSize);
                    zStartPoint += sizeZ;
                }
                zStartPoint = bounds.min.z;
                yStartPoint += sizeY;
            }
            zStartPoint = bounds.min.z;
            yStartPoint = bounds.min.y;
            xStartPoint += sizeX;
        }
    }

    public void ActivateSubBoundsByIndicator()
    {
        for (int i = 0; i < subBounds.Length; i++)
        {
            if (subBounds[i].CheckIntersects(indicator.transform.position))
            {
                subBounds[i].SetStatus(true);
            }
            else
            {
                subBounds[i].SetStatus(false);
            }
        }
    }

    public void SplitMesh()
    {
        for (int i = 0; i < subBounds.Length; i++)
        {
            foreach(GameObject g in list) trans = g.transform;
                
            Vector3[] worldPointVertices = transformToWorldPoint(mesh.vertices);
            for (int index = 0; index < worldPointVertices.Length; index++)
            {
                if (subBounds[i].CheckIntersects(worldPointVertices[index]))
                {
                    if (index < mesh.colors.Length) partialColors.Add(mesh.colors[index]);
                        
                    partialVertices.Add(mesh.vertices[index]);
                    partialNormals.Add(mesh.normals[index]);
                    partialUVs.Add(mesh.uv[index]);
                }
            }

            Vector3[] worldPointPartialVertices = transformToWorldPoint(partialVertices.ToArray());
            List<int> triangles = trianglesToUse(mesh.triangles, partialVertices);
            for (int index = 0; index < worldPointVertices.Length; index++)
            {
                for (int newIndex = 0; newIndex < worldPointPartialVertices.Length; newIndex++)
                {
                    if (worldPointVertices[index] == worldPointPartialVertices[newIndex] && !oldVertexIndices2newVertexIndices.ContainsKey(index))
                        oldVertexIndices2newVertexIndices.Add(index, newIndex);
                }
            }
            partialTriangles = transTriangle2newVertexIndices(triangles);

            drawNewMesh(i);
            clearArraysAndDic();
        }
    }

    public void Load()
    {
        for(int i = 0; i < subBounds.Length; i++)
        {
            GameObject g = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/File/NewMesh" + i + ".mat", typeof(GameObject));
            Instantiate(g);
        }
        
    }

    /// <summary>
    /// This function is support function of drawBoundingBox.
    /// Referenced from Unity community.
    /// https://answers.unity.com/questions/777855/bounds-finding-box.html
    /// https://docs.unity3d.com/ScriptReference/Bounds.html
    /// </summary>
    /// <param name="go"></param>
    /// <param name="points"></param>
    private static void getBoundsPointsNoAlloc(GameObject go, Vector3[] points)
    {
        if (points == null || points.Length < 8)
        {
            Debug.Log("Bad Array");
            return;
        }
        MeshFilter mf = go.GetComponent<MeshFilter>();
        if (mf == null)
        {
            Debug.Log("No MeshFilter on object");
            for (int i = 0; i < points.Length; i++)
                points[i] = go.transform.position;
            return;
        }

        Transform tr = go.transform;

        Vector3 v3Center = mf.mesh.bounds.center;
        Vector3 v3ext = mf.mesh.bounds.extents;

        points[0] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top left corner
        points[1] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top right corner
        points[2] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom left corner
        points[3] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom right corner
        points[4] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top left corner
        points[5] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top right corner
        points[6] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom left corner
        points[7] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom right corner
    }

    private Vector3[] transformToWorldPoint(Vector3[] array)
    {
        Vector3[] worldPosArray = new Vector3[array.Length];
        for (int index = 0; index < array.Length; index++)
        {
            worldPosArray[index] = trans.TransformPoint(array[index]);
        }

        return worldPosArray;
    }

    private void drawNewMesh(int index)
    {
        GameObject newGameObject = new GameObject();
        MeshFilter mf = newGameObject.AddComponent<MeshFilter>();
        mf.name = "NewMesh" + index;
        MeshRenderer mr = newGameObject.AddComponent<MeshRenderer>();
        Mesh m = new Mesh();

        m.vertices = partialVertices.ToArray();
        m.normals = partialNormals.ToArray();
        m.colors = partialColors.ToArray();
        m.uv = partialUVs.ToArray();
        //m.triangles = partialTriangles.ToArray();
        writeNewTriArray(partialTriangles, m);

        mf.mesh = m;
        mr.material = new Material(Shader.Find("Transparent/Diffuse"));

        AssetDatabase.CreateAsset(mr.material, "Assets/File/" + mf.name + ".mat");
    }

    /// <summary>
    /// This function figures out which triangle to use for split mesh.
    /// First, this function iterates through whole triangle array, and check each vertex shown in the triangle
    /// list to see if its in bounds or not. Then, it adds triangle to a new list if it is inside of the active
    /// bound.
    /// </summary>
    /// <param name="originalTriArray"></param>
    /// <param name="vertices"></param>
    /// <returns></returns>
    private List<int> trianglesToUse(int[] originalTriArray, List<Vector3> vertices)
    {
        //List<int> retVal = new List<int>();
        List<int> trianglesToKeep = new List<int>();
        List<int> triangle = new List<int>();
        for (int i = 0; i < originalTriArray.Length; i += 3)
        {
            triangle.Add(originalTriArray[i + 0]);
            triangle.Add(originalTriArray[i + 1]);
            triangle.Add(originalTriArray[i + 2]);

            int[] onlyOneTriangle = new int[3];
            onlyOneTriangle[0] = originalTriArray[i + 0];
            onlyOneTriangle[1] = originalTriArray[i + 1];
            onlyOneTriangle[2] = originalTriArray[i + 2];
            Triangle t = new Triangle(onlyOneTriangle);

            for (int triangleIndex = 0; triangleIndex < 3; triangleIndex++)
            {
                int vertexIndex = triangle[originalTriArray[triangleIndex]];
                Vector3 vertex = vertices[vertexIndex];
                bool keep = mesh.bounds.Contains(vertex);
                if (keep)
                {
                    //triangleHash.Add(t);
                    trianglesToKeep.Add(triangle[i + 0]);
                    trianglesToKeep.Add(triangle[i + 1]);
                    trianglesToKeep.Add(triangle[i + 2]);
                    //break;
                }
            }
        }
        //List<Triangle> trianglesToKeep = new List<Triangle>(triangleHash);
        //for (int i = 0; i < trianglesToKeep.Count; i++)
        //{
        //    retVal.AddRange(trianglesToKeep[i].GetTriangle());
        //}

        //return retVal;
        return trianglesToKeep;
    }

    /// <summary>
    /// This function figures out what indices they correespond to in the new vertex array which is counstructed by other function.
    /// 
    /// </summary>
    /// <param name="originalTriangleList"></param>
    /// <returns></returns>
    private List<int> transTriangle2newVertexIndices(List<int> originalTriangleList)
    {
        List<int> newTriangleList = new List<int>();

        List<int> triangle = new List<int>();
        for (int i = 0; i < originalTriangleList.Count; i += 3)
        {
            triangle.Insert(0, originalTriangleList[i + 2]);
            triangle.Insert(0, originalTriangleList[i + 1]);
            triangle.Insert(0, originalTriangleList[i + 0]);

            List<int> newTriangle = new List<int>();
            for (int tIndex = 0; tIndex < 3; tIndex++)
            {
                int oldVertexIndex = triangle[tIndex];
                if (!oldVertexIndices2newVertexIndices.ContainsKey(oldVertexIndex))
                    break;
                int newVertexIndex = oldVertexIndices2newVertexIndices[oldVertexIndex];
                newTriangle.Add(newVertexIndex);
            }
            newTriangleList.AddRange(newTriangle);
        }
        return newTriangleList;
    }

    private void writeNewTriArray(List<int> newTriangleList, Mesh m)
    {
        Debug.Log("number of index for triangle array: " + newTriangleList.Count);
        m.SetTriangles(newTriangleList.ToArray(), 0);
    }

    private void clearArraysAndDic()
    {
        partialVertices.Clear();
        partialNormals.Clear();
        partialColors.Clear();
        partialUVs.Clear();
        partialTriangles.Clear();
        oldVertexIndices2newVertexIndices.Clear();
        triangleHash.Clear();
    }
}
