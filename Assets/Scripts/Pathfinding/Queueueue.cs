using NUnit.Framework;
using UnityEditor.Search;
using UnityEngine;

public class Queueueue<T> where T : System.IComparable<T>
{
    private List<T> nodes = new List<T>();
    public int Count => nodes.Count;
    public void Enqueueueue (T node)
    {
        nodes.Add(node);
        int childIdx = nodes.Count - 1;

        while (childIdx >= 0)
        {
            int parentIdx = (childIdx - 1) / 2;
            if (nodes[childIdx].CompareTo(nodes[parentIdx]) >= 0)
            {
                break;
            }

            Swap(childIdx, parentIdx);
            childIdx = parentIdx;
        }
    }

    public 

    public T Dequeueueue()
    {
        T first = nodes[0];
        int lastIdx = nodes.Count - 1;
        nodes[0] = nodes[lastIdx];
        nodes.RemoveAt(lastIdx);
        lastIdx--;

        int parentIdx = 0;
        while (true)
        {
            int leftChild = parentIdx * 2 + 1;
            int rightChild = parentIdx * 2 + 2;
            int swapIdx = parentIdx;
            
            if(leftChild<= lastIdx && nodes[leftChild].CompareTo(nodes[swapIdx]) < 0)
            {
                swapIdx = leftChild;
            }

            if(rightChild <= lastIdx && nodes[rightChild].CompareTo(nodes[swapIdx]) < 0)
            {
                swapIdx = rightChild;
            }

            if(swapIdx == parentIdx)
            {
                break;
            }

            Swap(parentIdx, swapIdx);
            parentIdx = swapIdx;
        }
        return first;
    }

    private void Swap(int idxA, int idxB)
    {
        (nodes[idxA], nodes[idxB]) = (nodes[idxB], nodes[idxA]);
    }
}
