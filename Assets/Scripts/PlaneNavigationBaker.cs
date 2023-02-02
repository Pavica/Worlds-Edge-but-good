using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlaneNavigationBaker : MonoBehaviour
{
    public NavMeshSurface surface;
    public GameObject player;
    

    private void Start()
    {
        player = GameObject.Find("RigidBodyFPSController");
        StartCoroutine(initialBake(1));
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) > (25/2))
        {
            transform.position = new Vector3(player.transform.position.x, 1 ,player.transform.position.z);
            bake();
        }
    }

    private void bake()
    {
        surface.BuildNavMesh();
    }

    IEnumerator initialBake(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        bake();
    }

}