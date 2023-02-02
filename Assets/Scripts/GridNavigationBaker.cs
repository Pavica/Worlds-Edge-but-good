using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridNavigationBaker : MonoBehaviour
{
    // Start is called before the first frame update
    public NavMeshSurface surface;
    public GameObject gridGenerator;

    private int childCount;
    private void Start()
    {
        gridGenerator = GameObject.Find("GridGenerator");
        childCount = gridGenerator.transform.childCount;
    }

    private void Update()
    {
        
        if (childCount != childCountActive())
        {
            bake();
            childCount = childCountActive(); 
        }
    }

    private int childCountActive()
    {
        int activeChildren = 0;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (gridGenerator.transform.GetChild(i).gameObject.activeSelf)
            {
                activeChildren++;
            }
        }
        return activeChildren;
    }
    private void bake()
    {
        surface.BuildNavMesh();
    }
}

