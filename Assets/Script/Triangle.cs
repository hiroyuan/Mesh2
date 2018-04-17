using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle {
    private int[] triangleArray;

    public Triangle ()
    {
        triangleArray = new int[3];
    }

    public Triangle (int[] array)
    {
        triangleArray = new int[3];
        for (int index = 0; index < triangleArray.Length; index++)
        {
            triangleArray[index] = array[index];
        }
    }

    public int[] GetTriangle()
    {
        return triangleArray;
    }

    public static bool operator ==(Triangle t, Triangle other)
    {
        if (t.triangleArray.Length != other.triangleArray.Length) return false;

        for (int index = 0; index < t.triangleArray.Length; index++)
        {
            if (t.triangleArray[index] != other.triangleArray[index])
                return false;
        }

        return true;
    }

    public static bool operator !=(Triangle t, Triangle other)
    {
        if (t.triangleArray.Length != other.triangleArray.Length) return true;

        for (int index = 0; index < t.triangleArray.Length; index++)
        {
            if (t.triangleArray[index] != other.triangleArray[index])
                return true;
        }

        return false;
    }
}
