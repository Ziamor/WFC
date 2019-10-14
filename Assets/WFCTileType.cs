using UnityEngine;


public enum TileType {
    GRASS, SAND, WATER, STONE, DEEPWATER
}


[CreateAssetMenu(menuName = "WFC Tile")]
public class WFCTileType : ScriptableObject {
    public float testP = .5f;
    public TileType tileType;
    public Color color;

    public TileRule[] rules;

    public bool IsCompatible(TileType target) {
        for (int i = 0; i < rules.Length; i++) {
            if (rules[i].tileType == target)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public struct TileRule {
    public TileType tileType;
}