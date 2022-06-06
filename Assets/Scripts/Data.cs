using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Data {
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = new float[] { cos, sin, -sin, cos };

    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        //I pezzi spawnano dal loro riferimento in basso a sinistra
        //QUADRATO
        { Tetromino.S, new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(0, 0), new Vector2Int(-1, 0), new Vector2Int(0, -1) } },
        //RETTANGOLO
        { Tetromino.I, new Vector2Int[] { new Vector2Int(0, -2), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) } },
        //L
        { Tetromino.L, new Vector2Int[] { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(0, -1) } },
        //T
        { Tetromino.T, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(0, 0) } },
        //Punto
        { Tetromino.P, new Vector2Int[] { new Vector2Int(0, 0) } },
        //U
        { Tetromino.U, new Vector2Int[] { 
            new Vector2Int(-2, -2), new Vector2Int(-2, -1), new Vector2Int(-2, 0), new Vector2Int(-1, 0), 
            new Vector2Int(0, 0),new Vector2Int(1, 0),new Vector2Int(1, -1),
            new Vector2Int(1, -2),} 
        },
        
    };
}
