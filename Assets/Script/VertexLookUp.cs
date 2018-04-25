using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexLookUp {
    private Dictionary<int, int> dictionary;
    private int newIndex;

	public VertexLookUp()
    {
        dictionary = new Dictionary<int, int>();
        newIndex = 0;
    }

    public bool IsOldIndexInLookUp(int oldVertex)
    {
        return dictionary.ContainsKey(oldVertex);
    }

    public void AddOldIndex(int oldIndex)
    {
        dictionary.Add(oldIndex, getNewIndex());
    }

    public int FindValue(int index)
    {
        return dictionary[index];
    }

    private int getNewIndex()
    {
        int retVal = newIndex;
        newIndex++;
        return retVal;
    }
}
