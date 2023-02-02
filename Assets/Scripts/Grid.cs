using UnityEngine;
using System.Collections.Generic;
using Pathfinding;
using Unity.VisualScripting;

public class Grid : MonoBehaviour
{
    public GameObject[] treePrefabs;
    public GameObject[] stonePrefabs;
    public Material terrainMaterial;
    public Material edgeMaterial;
    public List<Color> grassColors;
    public float waterLevel = .4f;
    public float[] blockLevels = new float[] { 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, .9f, 1};
    public float heightMultiplier;
    public float scale = .1f;
    public float treeNoiseScale = .05f;
    public float treeDensity = .25f;
    public float stoneDensity = .25f;
    public float riverNoiseScale = .06f;
    public int rivers = 5;
    public int size = 100;

    float xTerrainOffset;
    float yTerrainOffset;

    Cell[,] grid;

    void Start()
    {
        xTerrainOffset = transform.position.x;
        yTerrainOffset = transform.position.z;

        float[,] noiseMap = new float[size, size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise((x +xTerrainOffset) * scale, (y+yTerrainOffset) * scale);
                noiseMap[x, y] = noiseValue;
            }
        }
        /*
        float[,] falloffMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float xv = x / (float)size * 2 - 1;
                float yv = y / (float)size * 2 - 1;
                float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                falloffMap[x, y] = Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
            }
        }
        */
        grid = new Cell[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = noiseMap[x, y];
                //noiseValue -= falloffMap[x, y];
                bool[] isblockLevels = new bool[blockLevels.Length];
                for(int i=0; i<blockLevels.Length; i++)
                {
                    isblockLevels[i] = noiseValue < blockLevels[i];
                }
                bool isWater = noiseValue < waterLevel;
                Cell cell = new Cell(isWater, isblockLevels);
                grid[x, y] = cell;
            }
        }

        GenerateRivers(grid);
        DrawTerrainMesh(grid);
        DrawEdgeMesh(grid);
        DrawTexture(grid);
        GenerateTrees(grid);


        foreach (Transform t in transform)
        {
            t.gameObject.layer = 6;
            t.gameObject.tag = "Ground";
        }

        this.AddComponent<MeshCollider>();


    }

    float getProperHeight(Cell cell)
    {
        float height = 0;

        for (int i = blockLevels.Length-1; i >= 0; i--)
        {
            if (cell.isBlockLevels[i])
            {
                height = i * heightMultiplier;
            }
        }

        return height;
    }

    void GenerateRivers(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * riverNoiseScale + xOffset, y * riverNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        GridGraph gg = AstarData.active.graphs[0] as GridGraph;
        gg.center = new Vector3(size / 2f - .5f, 0, size / 2f - .5f);
        gg.SetDimensions(size, size, 1);
        AstarData.active.Scan(gg);
        AstarData.active.AddWorkItem(new AstarWorkItem(ctx => {
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    GraphNode node = gg.GetNode(x, y);
                    node.Walkable = noiseMap[x, y] > .4f;
                }
            }
        }));
        AstarData.active.FlushGraphUpdates();

        int k = 0;
        for (int i = 0; i < rivers; i++)
        {
            GraphNode start = gg.nodes[Random.Range(16, size - 16)];
            GraphNode end = gg.nodes[Random.Range(size * (size - 1) + 16, size * size - 16)];
            ABPath path = ABPath.Construct((Vector3)start.position, (Vector3)end.position, (Path result) => {
                for (int j = 0; j < result.path.Count; j++)
                {
                    GraphNode node = result.path[j];
                    int x = Mathf.RoundToInt(((Vector3)node.position).x);
                    int y = Mathf.RoundToInt(((Vector3)node.position).z);
                    grid[x, y].isWater = true;
                }
                k++;
            });
            AstarPath.StartPath(path);
            AstarPath.BlockUntilCalculated(path);
        }
    }

    void DrawTerrainMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];
                
                float height = cell.isWater ? -heightMultiplier : getProperHeight(cell);

                Vector3 a = new Vector3(x - .5f, height, y + .5f);
                Vector3 b = new Vector3(x + .5f, height, y + .5f);
                Vector3 c = new Vector3(x - .5f, height, y - .5f);
                Vector3 d = new Vector3(x + .5f, height, y - .5f);
                Vector2 uvA = new Vector2(x / (float)size, y / (float)size);
                Vector2 uvB = new Vector2((x + 1) / (float)size, y / (float)size);
                Vector2 uvC = new Vector2(x / (float)size, (y + 1) / (float)size);
                Vector2 uvD = new Vector2((x + 1) / (float)size, (y + 1) / (float)size);
                Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                Vector2[] uv = new Vector2[] { uvA, uvB, uvC, uvB, uvD, uvC };
                for (int k = 0; k < 6; k++)
                {
                    vertices.Add(v[k]);
                    triangles.Add(triangles.Count);
                    uvs.Add(uv[k]);
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
    }

    void DrawEdgeMesh(Cell[,] grid)
    {
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                float height = getProperHeight(cell);

                if (!cell.isWater)
                {
                    
                    Vector3 a = new Vector3(xTerrainOffset + x - .5f, height,  yTerrainOffset + y + .5f);
                    Vector3 b = new Vector3(xTerrainOffset + x - .5f, height,  yTerrainOffset + y - .5f);
                    Vector3 c = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset +  y + .5f);
                    Vector3 d = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset +  y - .5f);
                    Vector3[] v = new Vector3[] { a, b, c, b, d, c };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v[k]);
                        triangles.Add(triangles.Count);
                    }
                    
                    Vector3 a1 = new Vector3(xTerrainOffset + x + .5f, height, yTerrainOffset + y - .5f);
                    Vector3 b1 = new Vector3(xTerrainOffset + x + .5f, height, yTerrainOffset + y + .5f);
                    Vector3 c1 = new Vector3(xTerrainOffset + x + .5f, height - 1,yTerrainOffset + y - .5f);
                    Vector3 d1 = new Vector3(xTerrainOffset + x + .5f, height - 1, yTerrainOffset + y + .5f);
                    Vector3[] v1 = new Vector3[] { a1, b1, c1, b1, d1, c1 };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v1[k]);
                        triangles.Add(triangles.Count);
                    }
                    
                    Vector3 a2 = new Vector3(xTerrainOffset + x - .5f, height, yTerrainOffset + y - .5f);
                    Vector3 b2 = new Vector3(xTerrainOffset + x + .5f, height, yTerrainOffset + y - .5f);
                    Vector3 c2 = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset + y - .5f);
                    Vector3 d2 = new Vector3(xTerrainOffset + x + .5f, height - 1, yTerrainOffset + y - .5f);
                    Vector3[] v2 = new Vector3[] { a2, b2, c2, b2, d2, c2 };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v2[k]);
                        triangles.Add(triangles.Count);
                    }
                    
                    Vector3 a3 = new Vector3(xTerrainOffset + x + .5f, height, yTerrainOffset + y + .5f);
                    Vector3 b3 = new Vector3(xTerrainOffset + x - .5f, height, yTerrainOffset + y + .5f);
                    Vector3 c3 = new Vector3(xTerrainOffset + x + .5f, height - 1, yTerrainOffset + y + .5f);
                    Vector3 d3 = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset + y + .5f);
                    Vector3[] v3 = new Vector3[] { a3, b3, c3, b3, d3, c3 };
                    
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v3[k]);
                        triangles.Add(triangles.Count);
                    }

                    //bottom
                    Vector3 a4 = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset + y - .5f);
                    Vector3 b4 = new Vector3(xTerrainOffset + x + .5f, height - 1, yTerrainOffset + y - .5f);
                    Vector3 c4 = new Vector3(xTerrainOffset + x - .5f, height - 1, yTerrainOffset + y + .5f);
                    Vector3 d4 = new Vector3(xTerrainOffset + x + .5f, height - 1, yTerrainOffset + y + .5f);
                    Vector3[] v4 = new Vector3[] { a4, b4, c4, b4, d4, c4 };
                    for (int k = 0; k < 6; k++)
                    {
                        vertices.Add(v4[k]);
                        triangles.Add(triangles.Count);
                    }
                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        GameObject edgeObj = new GameObject("Edge");
        edgeObj.transform.SetParent(transform);

        MeshFilter meshFilter = edgeObj.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        MeshRenderer meshRenderer = edgeObj.AddComponent<MeshRenderer>();
        meshRenderer.material = edgeMaterial;
        meshRenderer.AddComponent<MeshCollider>();
    }

    
    Color ColorBasedOnHeight(Cell cell)
    {
        float averagePerColor = (float) blockLevels.Length / grassColors.Count;

        for(int i= 0; i < blockLevels.Length; i++)
        {
            if (cell.isBlockLevels[i])
            {
                for(int k = 0; k < grassColors.Count; k++)
                {
                    if(i < (k+1) * averagePerColor)
                    {
                        return grassColors[k];
                    }
                }
            }
        }
        return grassColors[grassColors.Count-1];
    }

    void DrawTexture(Cell[,] grid)
    {
        Texture2D texture = new Texture2D(size, size);
        Color[] colorMap = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                colorMap[y * size + x] = ColorBasedOnHeight(cell);

                if (cell.isWater)
                    colorMap[y * size + x] = new Color(202/255f, 150/255f, 90/255f);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.SetPixels(colorMap);
        texture.Apply();

        MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = terrainMaterial;
        meshRenderer.material.mainTexture = texture;
    }

    void GenerateTrees(Cell[,] grid)
    {
        float[,] noiseMap = new float[size, size];
        (float xOffset, float yOffset) = (Random.Range(-10000f, 10000f), Random.Range(-10000f, 10000f));
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = Mathf.PerlinNoise(x * treeNoiseScale + xOffset, y * treeNoiseScale + yOffset);
                noiseMap[x, y] = noiseValue;
            }
        }

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Cell cell = grid[x, y];

                float height = getProperHeight(cell);
                if (!cell.isWater)
                {
                    float vTree = Random.Range(0f, 1f);
                    float vStone = Random.Range(0f, 1f);
                    if (vTree < treeDensity)
                    {
                        GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Length)];
                        GameObject tree = Instantiate(prefab, transform);
                        tree.transform.position = new Vector3(x + xTerrainOffset, height, y + yTerrainOffset);
                        tree.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        tree.transform.localScale = Vector3.one * Random.Range(.1f, .2f);
                    }else if(vStone < stoneDensity)
                    {
                        GameObject prefab = stonePrefabs[Random.Range(0, stonePrefabs.Length)];
                        GameObject stone = Instantiate(prefab, transform);
                        stone.transform.position = new Vector3(x + xTerrainOffset, height, y + yTerrainOffset);
                        stone.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
                        stone.transform.localScale = Vector3.one * Random.Range(.6f, .8f);
                    }
                }
            }
        }
    }
}