using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WFCMapGenerator : MonoBehaviour {
    public int mapSize = 10;
    public float runSpeed = 0.001f;
    public WFCTileType[] tileTypes;
    public WFCTile tilePrefab;

    WFCTile[,] tiles;

    bool running = true;

    Queue<WFCTile> tilesToCheck;
    // Start is called before the first frame update
    void Start() {
        Init();
        StartCoroutine(Execute());
    }

    // Update is called once per frame
    void Update() {
        if (running) {
            //Wave();
        }
    }

    public void Init() {
        tilesToCheck = new Queue<WFCTile>();

        if (tiles != null) {
            for (int x = 0; x < mapSize; x++) {
                for (int y = 0; y < mapSize; y++) {
                    if (Application.isEditor)
                        DestroyImmediate(tiles[x, y].gameObject);
                    else
                        Destroy(tiles[x, y].gameObject);
                }
            }
        }
        tiles = new WFCTile[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                tiles[x, y] = Instantiate(tilePrefab, transform).GetComponent<WFCTile>();
                tiles[x, y].SetSuperPosition(tileTypes);
                tiles[x, y].SetIndex(x, y);
                tiles[x, y].transform.position = new Vector3(x, y, 0);
            }
        }
    }

    public IEnumerator Execute() {
        while (running) {
            //print("Start Wave");
            yield return Wave();
        }
    }

    public IEnumerator Wave() {
        print("Wave");
        Observe();
        yield return Propagate();
    }

    public void Observe() {
        float lowestEntropy = float.MaxValue;
        WFCTile tileToCollapse = null;

        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                WFCTile tile = tiles[x, y];
                float entropy = tile.CalculateEntropy();
                if (entropy < lowestEntropy && tile.CanCollapse()) {
                    lowestEntropy = entropy;
                    tileToCollapse = tile;
                }
            }
        }
        if (tileToCollapse != null) {
            tileToCollapse.Collapse();
            CheckNeighbours(tileToCollapse);
        } else {
            running = false;
            print("Can't find tile to collapse");
        }
    }

    public bool IsBoundry(int x, int y) {
        return x < 0 || x >= mapSize || y < 0 || y >= mapSize;
    }

    public IEnumerator Propagate() {
        print("Propogate");
        while (tilesToCheck.Count > 0) {
            WFCTile tile = tilesToCheck.Dequeue();
            if (!tile.CanCollapse()) continue;
            int x = tile.x;
            int y = tile.y;
            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++) {
                    if (!tile.CanCollapse())
                        break;
                    if (i == 0 && j == 0) continue;
                    int x2 = x + i;
                    int y2 = y + j;
                    if (IsBoundry(x2, y2)) continue;
                    WFCTile neighBour = tiles[x2, y2];
                    if (tile.Propagete(neighBour)) {
                        CheckNeighbours(tile);
                    }
                }
            }
        }
        yield return null;
    }

    void CheckNeighbours(WFCTile tile) {
        int x = tile.x;
        int y = tile.y;

        for (int i = -1; i <= 1; i++) {
            for (int j = -1; j <= 1; j++) {
                if (i == 0 && j == 0) continue;
                int x2 = x + i;
                int y2 = y + j;
                if (IsBoundry(x2, y2)) continue;

                WFCTile neighBour = tiles[x2, y2];
                if (neighBour.CanCollapse()) {
                    tilesToCheck.Enqueue(neighBour);
                }
            }
        }
    }
}

[CustomEditor(typeof(WFCMapGenerator))]
public class WFCMapGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        WFCMapGenerator generator = (WFCMapGenerator)target;
        if (GUILayout.Button("Setup")) {
            generator.Init();
        }

        if (GUILayout.Button("Run")) {
            generator.StartCoroutine(generator.Execute());
        }

        if (GUILayout.Button("Step")) {
            Debug.Log("Step");
            generator.Wave();
        }
    }
}
