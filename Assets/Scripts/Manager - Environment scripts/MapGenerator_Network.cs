using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MapGenerator_Network : NetworkBehaviour
{
    public GameObject tilePrefab;
    public GameObject reflectBulletPrefab;
    public GameObject blockBulletPrefab;
    [Range(0, 1)] public float obstaclePercent;
    [Range(0, 100)] public int blockBulletPercentage;
    public GameObject[] itemList;
    public int numItems;
    public int spawnItemInterval;
    public int width;
    public int height;

    private GameObject[] obstaclePrefab;

    private float tileSize = 1.11f;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Queue<Coord> shuffledOpenTileCoords;
    Transform[,] tileMap;
    bool[,] obstacleMap;
    bool[,] itemMap;
    Transform mapHolder;

    Coord[] playerSpawnPoints;
    Coord mapSize;
    int seed;

    void Start()
    {
        if (!isServer)
            return;

        GenerateObstacleArray();

        mapSize.x = width;
        mapSize.y = height;
        seed = (int)Random.Range(-1000000000f, 1000000000f);

        transform.rotation = Quaternion.identity;

        playerSpawnPoints = new Coord[8];
        playerSpawnPoints[0] = new Coord(9, 6);
        playerSpawnPoints[1] = new Coord(9, 11);
        playerSpawnPoints[2] = new Coord(22, 6);
        playerSpawnPoints[3] = new Coord(22, 11);
        playerSpawnPoints[4] = new Coord(1, 6);
        playerSpawnPoints[5] = new Coord(13, 6);
        playerSpawnPoints[6] = new Coord(2, 2);
        playerSpawnPoints[7] = new Coord(14, 2);

        GenerateMap();
        itemMap = new bool[(int)mapSize.x, (int)mapSize.y];
        GenerateItems(numItems);
        Invoke("IncreaseNumItems", spawnItemInterval);
    }

    void GenerateObstacleArray()
    {
        obstaclePrefab = new GameObject[100];
        for (int i = 0; i < 100; i++)
        {
            if (i < blockBulletPercentage)
                obstaclePrefab[i] = blockBulletPrefab;
            else
                obstaclePrefab[i] = reflectBulletPrefab;
        }
    }

    void GenerateMap()
    {
        tileMap = new Transform[mapSize.x, mapSize.y];
        System.Random prng = new System.Random(seed);
        System.Random prng2 = new System.Random(seed + 1);

        allTileCoords = new List<Coord>();

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        // Create map holder object
        string holderName = "Generated Map";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }
        mapHolder = new GameObject(holderName).transform;
        mapHolder.parent = transform;

        // Spawning tiles
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Vector3 tilePosition = CoordToPosition(x, y);
                GameObject newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90));
                newTile.transform.localScale = Vector3.one * tileSize;
                newTile.transform.parent = mapHolder;
                tileMap[x, y] = newTile.transform;

                NetworkServer.Spawn(newTile);
            }
        }

        obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        List<Coord> allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            //random a spawn obstacle
            if (obstaclePrefab.Length == 0) return;
            int obstaclePrefabIndex = prng2.Next(0, obstaclePrefab.Length);

            //random spawn coordinate,
            Coord randomCoord = GetRandomCoord();
            currentObstacleCount++;
            obstacleMap[randomCoord.x, randomCoord.y] = true;

            if (CheckPlayerSpawnPoints(randomCoord) && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                GameObject Obstacle = Instantiate(obstaclePrefab[obstaclePrefabIndex], obstaclePosition,
                                                  Quaternion.identity);
                Obstacle.transform.parent = mapHolder;

                NetworkServer.Spawn(Obstacle);
                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }
        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), seed));
    }

    //Check map connectivity
    bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();

        int accessibleTileCount = 0;
        queue.Enqueue(playerSpawnPoints[0]);
        accessibleTileCount++;
        mapFlags[playerSpawnPoints[0].x, playerSpawnPoints[0].y] = true;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            //L,U,D,R
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 ^ y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) 
                            && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public void GenerateItems(int num)
    {
        while (num-- > 0)
        {
            Coord randomCoord = GetRandomCoord();
            if (obstacleMap[randomCoord.x, randomCoord.y] 
                || itemMap[randomCoord.x, randomCoord.y] 
                || !CheckPlayerSpawnPoints(randomCoord))
            {
                num++;
                continue;
            }
            Vector3 itemPosition = CoordToPosition(randomCoord.x, randomCoord.y);
            int itemPrefabIndex = (int) Random.Range(0f, itemList.Length);
            GameObject item = Instantiate(itemList[itemPrefabIndex], itemPosition,
                                          Quaternion.identity, mapHolder);
            NetworkServer.Spawn(item);
            itemMap[randomCoord.x, randomCoord.y] = true;
        }
    }

    void IncreaseNumItems()
    {
        GenerateItems(1);
        Invoke("IncreaseNumItems", spawnItemInterval);
    }

    Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-mapSize.x / 2f + 0.5f + x, -mapSize.y / 2f + 0.5f + y, 0f) * tileSize;
    }

    Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    [System.Serializable]
    struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 == c2);
        }
    }

    bool CheckPlayerSpawnPoints(Coord coord)
    {
        for (int i = 0; i < playerSpawnPoints.Length; i++)
        {
            if (coord == playerSpawnPoints[i])
                return false;
        }
        return true;
    }
}