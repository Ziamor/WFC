using System.Collections;
using UnityEngine;

public class WFCMapGenerator : MonoBehaviour {
    public int mapSize = 10;
    public float runSpeed = 0.001f;
    public WFCTileType[] tileTypes;
    public WFCTile tilePrefab;

    WFCTile[,] tiles;

    bool running = true;

    // Start is called before the first frame update
    void Start() {
        tiles = new WFCTile[mapSize, mapSize];
        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                tiles[x, y] = Instantiate(tilePrefab).GetComponent<WFCTile>();
                tiles[x, y].SetSuperPosition(tileTypes);

                tiles[x, y].transform.position = new Vector3(x, y, 0);
            }
        }
        StartCoroutine(Execute());
    }

    // Update is called once per frame
    void Update() {
        if (running) {
            //Wave();
        }
    }

    public IEnumerator Execute() {
        while (running) {
            //print("Start Wave");
            yield return Wave();
        }
    }

    public IEnumerator Wave() {
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
        } else {
            running = false;
            print("Can't find tile to collapse");
        }
    }

    public bool IsBoundry(int x, int y) {
        return x < 0 || x >= mapSize || y < 0 || y >= mapSize;
    }

    public IEnumerator Propagate() {
        for (int x = 0; x < mapSize; x++) {
            for (int y = 0; y < mapSize; y++) {
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        if (i == 0 && j == 0) continue;
                        int x2 = x + i;
                        int y2 = y + j;
                        if (IsBoundry(x2, y2)) continue;
                        WFCTile tile = tiles[x, y];
                        WFCTile neighBour = tiles[x2, y2];
                        if (tile.CanCollapse() && neighBour.superPosition.Count == 1)
                            yield return tile.Propagete(neighBour);                        
                    }
                }
            }
        }
    }
}
