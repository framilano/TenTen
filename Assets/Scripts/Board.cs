
using System;
using System.Collections.Generic; 
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement; 
using TMPro;

/*
*   Tilemap -> mappa utilizzata per il disegno dei nuovi pezzi disponibili e il settaggio nella griglia di gioco
*   Ghostmap -> mappa utilizzata per lo spostamento dei pezzi durante il dragging
*/

public class Board : MonoBehaviour {
    public const int numberOfPieces = 3;
    public const int boardDimension = 10;
    public Tilemap tilemap { get; private set; }
    public Tilemap ghostmap { get; private set; }
    private List<Piece> activePieces = new List<Piece>();
    public TetrominoData[] tetrominos;
    public Vector3Int[] spawnPositions;
    private Piece selectedPiece;

    public TextMeshProUGUI reset;

    public Vector2Int boardSize = new Vector2Int(boardDimension, boardDimension);

    public RectInt Bounds {
        get {
            Vector2Int position = new Vector2Int(-this.boardSize.x / 2, -this.boardSize.y / 2 );
            return new RectInt(position, this.boardSize);
        }
    }

    private void Awake() {
        //Tilemap principale per i pezzi finalmente fermi (allo spawn e dopo il piazzamento)
        this.tilemap = GetComponentsInChildren<Tilemap>()[0];

        //Tilemap per gli spostamenti, mentre sposto un pezzo
        this.ghostmap = GetComponentsInChildren<Tilemap>()[1];

        for (int i = 0; i < this.tetrominos.Length; i++) {
            this.tetrominos[i].Initialize();
        }
    }

    private void Start() {     
        for (int i = 0; i < numberOfPieces; i++) {
            SpawnPiece(i);
        }
    }

    public void SpawnPiece(int positionIndex) {
        int random = UnityEngine.Random.Range(0, this.tetrominos.Length);
        TetrominoData data = this.tetrominos[random];
        //Aggiungo dei nuovi pezzi all'elenco dei pezzi attivi
        //Uso gameObject (l'oggetto a cui è collegato lo script) con AddComponent
        //NON POSSO FARE LA NEW DI UN MONOBEHAVIOUR
        Piece newPiece = gameObject.AddComponent<Piece>();
        newPiece.Initialize(spawnPositions[positionIndex], data, this, positionIndex);
        this.activePieces.Add(newPiece);

        Set(newPiece, false);
    }

    public void Set(Piece piece, bool isGhost) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            if (isGhost) this.ghostmap.SetTile(tilePosition, piece.data.tile);
            else this.tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece, bool isGhost) {
        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            if (isGhost) this.ghostmap.SetTile(tilePosition, null);
            else this.tilemap.SetTile(tilePosition, null);
        }
    }

    /**
    *   Controllo la posizione raggiunta non sia utilizzabile dalle celle del piece
    **/
    public bool isValidPosition(Piece piece, Vector3Int position) {

        RectInt bounds = this.Bounds;

        for (int i = 0; i < piece.cells.Length; i++) {
            Vector3Int tilePosition =  piece.cells[i] + position ;

            if (!bounds.Contains((Vector2Int)tilePosition)) {
                Debug.Log("Fuori dai bound!");
                return false;
            }

            if (this.tilemap.HasTile(tilePosition)) {
                Debug.Log("Non puoi passare sopra un tile!");
                return false;
            }
        }
        return true;
    }
    
    /**
    * Recupero un Piece data la posizione passata
    * Se la posizione colpisce la cella di un Piece, restituisco l'intero Piece
    **/
    private Piece GetPiece(Vector3Int position) {
        //Tile tile = tilemap.GetTile<Tile>(position);  //Recupero Tile dalla posizione

        //Controllo che la posizione di click del mouse corrisponda a un pezzo attivo
        foreach (var Piece in activePieces) {
            foreach (var cell in Piece.cells) {
                if (position.Equals(cell + Piece.position)) {
                    Debug.Log("Hai selezionato un pezzo");
                    return Piece;
                }
            }
        }

        return null;
    }

    /**
    *   Metodo chiamato a ogni frame
    **/
    public void Update() {
        //BUTTON RELEASE EVENT (DURA 1 FRAME)
        if (Input.GetMouseButtonUp(0)) {

            if (this.selectedPiece is null) return;

            //Recupero posizione nella tilemap una volta rilasciato il mouse
            Vector3Int tilemapPos = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (!isValidPosition(this.selectedPiece, tilemapPos)) {
                this.Clear(selectedPiece, true);

                //Rimando il pezzo nella sua posizione iniziale nella Tilemap
                this.selectedPiece.SetPosition(this.spawnPositions[this.selectedPiece.spawnIndex]);
                this.Set(this.selectedPiece, true);
                this.selectedPiece = null;
                return;
            }

            //Setto l'elemento nella tilemap se la posizione valida
            this.Set(this.selectedPiece, false);
            //Elimino il pezzo dalla GhostMap
            this.Clear(this.selectedPiece, true);

            //Rimuovo il Pezzo appena posizionato dai pezzi attualmente attivi
            //Per rimuoverlo confronto l'indice di posizione di partenza del Pezzo
            foreach (var Piece in this.activePieces) {
                if (Piece.spawnIndex == this.selectedPiece.spawnIndex) {
                    this.activePieces.Remove(Piece);
                    Destroy(Piece);
                    break;
                }
            }

            this.selectedPiece = null;

            //Riempio nuovamente con nuovi pezzi
            if (this.activePieces.Count == 0) {
                this.Start();
            }

            //Controllo se l'utente ha fatto dei punti
            ScoreManager.instance.addPoints(this.CheckRowsAndColumns());

            return;
        }

        //BUTTON PRESS EVENT (DURA 1 FRAME)
        if (Input.GetMouseButtonDown(0)) {
            Vector3Int ghostmapPos = ghostmap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //Recupera Piece
            Piece p = GetPiece(ghostmapPos);
            if (p is null)  { 
                Debug.Log("Non hai selezionato un pezzo");
                return;
            } 

            this.selectedPiece = p;

            //Il pezzo sarà ora disponibili sulla ghostmap, quindi lo cancello dalla tilemap
            this.Clear(this.selectedPiece, false);
            return;
        }

        //BUTTON PRESSED EVENT (DURA TANTI FRAME QUANTO LA DURATA DI PRESSIONE DEL TASTO)
        if (Input.GetMouseButton(0)) {
            if (this.selectedPiece is null) return;
            Vector3Int ghostmapPos = ghostmap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            //Ogni volta che mi sposto e tengo premuto il mouse pulisco la GhostMap e la aggiorno
            //con la nuova posizione
            this.Clear(this.selectedPiece, true);

            //Aggiorna Piece
            this.selectedPiece.SetPosition(ghostmapPos);
            //Mentre premo il tasto sinistro del mouse attendo anche un input di rotazione
            if (Input.GetMouseButtonDown(1)) {
                Debug.Log("Hai premuto mouse destro!");

                //Applico matrice di rotazione
                for (int i = 0; i < this.selectedPiece.cells.Length; i++) {
                    Vector3 cell = this.selectedPiece.cells[i];

                    int x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0]) + (cell.y * Data.RotationMatrix[1]));
                    int y =  Mathf.RoundToInt((cell.x * Data.RotationMatrix[2]) + (cell.y * Data.RotationMatrix[3]));

                    this.selectedPiece.cells[i] = new Vector3Int(x, y, 0);
                }
            }

            //Setta Piece
            this.Set(this.selectedPiece, true);
            return;
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
    }

    public int CheckRowsAndColumns() {
        Vector3Int tilePosition;

        int clearedLines = 0;

        //Controllo che le righe siano piene
        //Checking Rows
        bool filledRow;
        for (int row = boardDimension / 2 - 1; row >= - boardDimension / 2; row--) {
            filledRow = true;
            for (int column = - boardDimension / 2; column <= boardDimension / 2 - 1; column++) {
                tilePosition = new Vector3Int(column, row, 0);
                if (!this.tilemap.HasTile(tilePosition)) {
                    filledRow = false;
                    break;
                }
            }
            if (filledRow) {
                //Se la riga è piena, la pulisco
                ClearRow(row);
                clearedLines += 1;
            }
        }

        //Controllo che le colonne siano piene
        //Checking Columns
        bool filledColumn;
        for (int column = - boardDimension / 2; column <= boardDimension / 2 - 1; column++) {
            filledColumn = true;
            for (int row = boardDimension / 2 - 1; row >= - boardDimension / 2; row--) {
                tilePosition = new Vector3Int(column, row, 0);
                if (!this.tilemap.HasTile(tilePosition)) {
                    filledColumn = false;
                    break;
                }
            }
            if (filledColumn) {
                ClearColumn(column);
                clearedLines += 1;
            }
        }
        
        return clearedLines;

    }

    public void ClearRow(int row) {
        Debug.Log("Hai fatto punto!");
        Vector3Int tilePosition;
        for (int column = - boardDimension / 2; column <= boardDimension / 2 - 1; column++) {
            tilePosition = new Vector3Int(column, row, 0);
            this.tilemap.SetTile(tilePosition, null);
        }
    }

    public void ClearColumn(int column) {
        Debug.Log("Hai fatto punto!");
        Vector3Int tilePosition;
        for (int row = boardDimension / 2 - 1; row >= - boardDimension / 2; row--) {
            tilePosition = new Vector3Int(column, row, 0);
            this.tilemap.SetTile(tilePosition, null);
        }
    }
}
