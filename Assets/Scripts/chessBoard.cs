using UnityEngine;

public class chessBoard : MonoBehaviour
{
    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.15f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    //LOGIC
    private ChessPiece[,] chessPieces;
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCamera;
    private Vector2Int currentHover;
    private Vector3 bounds;

    private void Awake()
    {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }
    private void Update() {
        if(!currentCamera)
        {
            currentCamera = Camera.main;
            return;
        }
        RaycastHit info;
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile", "Hover")))
        {
            // Get the indexes of the tile i have hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);
            
            if(currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if(currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        }
        else
        {
            if(currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    //generate the board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;

        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x,y] = GenerateSingleTile(tileSize, x, y);
    }
    private GameObject GenerateSingleTile(float tilezise, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tilezise, yOffset, y * tilezise) - bounds;
        vertices[1] = new Vector3(x * tilezise, yOffset, (y + 1) * tilezise) - bounds;
        vertices[2] = new Vector3((x + 1) * tilezise, yOffset, y * tilezise) - bounds;
        vertices[3] = new Vector3((x + 1) * tilezise, yOffset, (y + 1) * tilezise) - bounds;

        int[] tris = new int[] {0, 1, 2, 1, 3, 2};

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }
    
    //Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        //White team
        chessPieces[0,0] = SpawnSinglePiece(chessPieceType.Rook, whiteTeam);
        chessPieces[1,0] = SpawnSinglePiece(chessPieceType.Knight, whiteTeam);
        chessPieces[2,0] = SpawnSinglePiece(chessPieceType.Bishop, whiteTeam);
        chessPieces[3,0] = SpawnSinglePiece(chessPieceType.Queen, whiteTeam);
        chessPieces[4,0] = SpawnSinglePiece(chessPieceType.King, whiteTeam);
        chessPieces[5,0] = SpawnSinglePiece(chessPieceType.Bishop, whiteTeam);
        chessPieces[6,0] = SpawnSinglePiece(chessPieceType.Knight, whiteTeam);
        chessPieces[7,0] = SpawnSinglePiece(chessPieceType.Rook, whiteTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,1] = SpawnSinglePiece(chessPieceType.Pawn, whiteTeam);
        //Black team
        chessPieces[0,7] = SpawnSinglePiece(chessPieceType.Rook, blackTeam);
        chessPieces[1,7] = SpawnSinglePiece(chessPieceType.Knight, blackTeam);
        chessPieces[2,7] = SpawnSinglePiece(chessPieceType.Bishop, blackTeam);
        chessPieces[3,7] = SpawnSinglePiece(chessPieceType.Queen, blackTeam);
        chessPieces[4,7] = SpawnSinglePiece(chessPieceType.King, blackTeam);
        chessPieces[5,7] = SpawnSinglePiece(chessPieceType.Bishop, blackTeam);
        chessPieces[6,7] = SpawnSinglePiece(chessPieceType.Knight, blackTeam);
        chessPieces[7,7] = SpawnSinglePiece(chessPieceType.Rook, blackTeam);
        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i,6] = SpawnSinglePiece(chessPieceType.Pawn, blackTeam);
    }
    private ChessPiece SpawnSinglePiece(chessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int) type - 1], transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        return cp;
    }

    //positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }
    private void PositionSinglePiece(int x, int y, bool force = false)
    {
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        float currentYPosition = chessPieces[x,y].transform.position.y;
        chessPieces[x,y].transform.position = GetTileCenter(x, y, currentYPosition);
    }
    private Vector3 GetTileCenter(int x, int y, float currentYPosition)
    {
        return new Vector3(x * tileSize, currentYPosition,y * tileSize) - bounds  + new Vector3(tileSize/2, 0 , tileSize/2);
    }

    //Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x,y] == hitInfo)
                    return new Vector2Int(x, y);
        
        return -Vector2Int.one;
    }

}
