using UnityEngine.Tilemaps;
using UnityEngine;

public enum Tetromino {
    S,          //S - Square 4 celle
    I,          //I -  4 celle
    L,          //L - destra 4 celle
    T,          //T - trattino 2 celle
    P,          //P - punto 1 cella
    U           //U - 10 celle   
}

[System.Serializable]
public struct TetrominoData {
    public Tetromino  tetromino;
    public Tile tile;

    public Vector2Int[] cells { get; private set; }

    public void Initialize() {
        this.cells = Data.Cells[this.tetromino];
    }
}