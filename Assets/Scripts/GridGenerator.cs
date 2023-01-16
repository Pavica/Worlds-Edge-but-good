using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GridGenerator : MonoBehaviour
{
    public GameObject grid;
    public int xAmount = 3;
    public int yAmount = 3;
    public int spaceBetween = 70;

    public GameObject player;
    public GameObject enemy;

    List<GameObject> grids = new List<GameObject>();
    int size;

    void Start()
    {
        size = grid.GetComponent<Grid>().size;

        int x = Random.Range(-10000, 10000);
        int y = Random.Range(-10000, 10000);
        player.transform.position = new Vector3(x + 50, 1, y + 50);
        enemy.GetComponent<NavMeshAgent>().Warp(new Vector3(x + 50, 1, y + +55));

        //Spawn first 
        GameObject grid1 = Instantiate(grid, new Vector3(x, 0, y), Quaternion.identity, transform);
        GameObject grid2 = Instantiate(grid, new Vector3(x + spaceBetween, 0, y + spaceBetween), Quaternion.identity, transform);
        GameObject grid3 = Instantiate(grid, new Vector3(x - spaceBetween, 0, y - spaceBetween), Quaternion.identity, transform);
        GameObject grid4 = Instantiate(grid, new Vector3(x + spaceBetween, 0, y - spaceBetween), Quaternion.identity, transform);
        GameObject grid5 = Instantiate(grid, new Vector3(x - spaceBetween, 0, y + spaceBetween), Quaternion.identity, transform);
        GameObject grid6 = Instantiate(grid, new Vector3(x, 0, y + spaceBetween), Quaternion.identity, transform);
        GameObject grid7 = Instantiate(grid, new Vector3(x + spaceBetween, 0, y), Quaternion.identity, transform);
        GameObject grid8 = Instantiate(grid, new Vector3(x - spaceBetween, 0, y), Quaternion.identity, transform);
        GameObject grid9 = Instantiate(grid, new Vector3(x, 0, y - spaceBetween), Quaternion.identity, transform);

        grids.Add(grid1);
        grids.Add(grid2);
        grids.Add(grid3);
        grids.Add(grid4);
        grids.Add(grid5);
        grids.Add(grid6);
        grids.Add(grid7);
        grids.Add(grid8);
        grids.Add(grid9);

        Debug.Log(grids.Count);
    }

    private void Update()
    {
        for (int i=0; i<grids.Count; i++)
        {
            float distance = Vector3.Distance(player.transform.position, grids[i].transform.position);

            if(distance > size*2)
            {
                Destroy(grids[i]);
                grids.Remove(grids[i]);
                GameObject newGrid  = Instantiate(grid, new Vector3(player.transform.position.x -size/2, 0 , player.transform.position.z-size/2), Quaternion.identity, transform);
                grids.Add(newGrid);
            }
        }
    }
}

