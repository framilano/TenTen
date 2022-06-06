using UnityEngine;

public class Piece: MonoBehaviour
{

    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int position { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public int spawnIndex { get; private set; }

    public void Initialize(Vector3Int position, TetrominoData data, Board board, int spawnIndex)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.spawnIndex = spawnIndex;

        if (this.cells is null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    public void SetPosition(Vector3Int position) {
        this.position = position;
    }
}