using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundHolder
{
    public Bounds subBound;
    private bool isActive;

    public BoundHolder()
    {

    }

    public BoundHolder(Vector3 center, Vector3 size)
    {
        subBound = new Bounds(center, size);
        isActive = false;
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
}