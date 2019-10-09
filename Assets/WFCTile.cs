using System.Collections.Generic;
using UnityEngine;

public class WFCTile : MonoBehaviour {
    public List<WFCTileType> superPosition;
    public int x;
    public int y;

    SpriteRenderer spriteRenderer;

    float entropy;

    // Start is called before the first frame update
    void Start() {

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
        for (int i = 0; i < superPosition.Count; i++) {
            entropy += superPosition[i].testP * Mathf.Log(superPosition[i].testP);
        }
        return entropy;
        //return superPosition.Count;
    }

    public bool CanCollapse() {
        return superPosition.Count > 1;
    }

    public void Collapse() {
        superPosition.RemoveAt(Random.Range(0, superPosition.Count));
        Refresh();
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
