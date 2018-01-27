using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSortingLayer : MonoBehaviour
{
    public string SortingLayerName = "Default";
    public int SortingOrder = 0;

    void Awake()
        {
            GetComponent<MeshRenderer>().sortingLayerName = SortingLayerName;
            GetComponent<MeshRenderer>().sortingOrder = SortingOrder;
        }
}
