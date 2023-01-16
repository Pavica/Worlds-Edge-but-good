using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaneNavigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    private void Start()
    {
        bake();
    }

    private void Update()
    {
        bake();
    }

    private void bake()
    {
        surface.BuildNavMesh();
    }
}