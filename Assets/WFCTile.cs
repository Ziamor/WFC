using System.Collections.Generic;
using UnityEngine;

public class WFCTile : MonoBehaviour {
    public List<WFCTileType> superPosition;
    public int x;
    public int y;

    SpriteRenderer spriteRenderer;
    WFCMapGenerator mapGenerator;

    float entropy;

    // Start is called before the first frame update
    void Start() {
        mapGenerator = FindObjectOfType<WFCMapGenerator>();
    }

    // Update is called once per frame
    void Update() {

    }

    public void SetSuperPosition(WFCTileType[] tiles) {
        superPosition = new List<WFCTileType>();
        for (int i = 0; i < tiles.Length; i++) {
            superPosition.Add(tiles[i]);
        }
        Refresh();
    }

    public void Refresh() {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        float r = 0, g = 0, b = 0;
        for (int i = 0; i < superPosition.Count; i++) {
            Color color = superPosition[i].color;
            r += color.r;
            g += color.g;
            b += color.b;
        }
        float l = superPosition.Count;
        spriteRenderer.color = new Color(r / l, g / l, b / l);
    }

    public float CalculateEntropy() {
        entropy = 0;

        superPosition.Sort((x, y) => (int)(y.testP - x.testP));

        float total = 0;
        for (int i = 0; i < superPosition.Count; i++) {
            total += superPosition[i].testP;
        }

        for (int i = 0; i < superPosition.Count; i++) {
            float v = superPosition[i].testP / total;
            float logV = Mathf.Log(v);
            entropy += -v * Mathf.Log(v);
        }
        return entropy + Random.value * 0.0001f;
        //return superPosition.Count;
    }

    public bool CanCollapse() {
        return superPosition.Count > 1;
    }

    public void Collapse() {
        float total = 0;
        for (int i = 0; i < superPosition.Count; i++) {
            total += superPosition[i].testP;
        }
        superPosition.Shuffle();
        float random = Random.value * total;
        for (int i = 0; i < superPosition.Count; i++) {
            float v = superPosition[i].testP;
            if (random < v) {
                WFCTileType toKeep = superPosition[i];
                superPosition.Clear();
                superPosition.Add(toKeep);
                Refresh();
                return;
            }
            random -= v;
        }

        print("Somthing went horribly wrong");
    }

    public void SetIndex(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public bool Propagete(WFCTile neighbourTile) {
        bool changed = false;
        for (int i = superPosition.Count - 1; i >= 0; i--) {
            if (!CanCollapse()) break;
            WFCTileType tileType = superPosition[i];
            bool compatible = false;
            for (int j = 0; j < neighbourTile.superPosition.Count; j++) {
                WFCTileType neighbourTileType = neighbourTile.superPosition[j];
                if (tileType.IsCompatible(neighbourTileType.tileType)) {
                    compatible = true;
                    break;
                }
            }
            if (!compatible) {
                superPosition.RemoveAt(i);
                changed = true;
            }

        }
        Refresh();
        return changed;
    }
}
