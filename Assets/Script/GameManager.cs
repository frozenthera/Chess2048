using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    //FIXME_Later move to Singleton<>
    public static GameManager Inst;

    private Dictionary<PieceEnum, GameObject> pieceDict = new();

    public NetworkVariable<PlayerEnum> curPlayer = new NetworkVariable<PlayerEnum>(PlayerEnum.BLACK);

    public Checker[,] boardState = new Checker[4,4];
    public PlayerEnum[,] boardPlayerState;
    public PieceEnum[,] boardPieceState;

    // public NetworkVariable<List<Piece>> boardPieceList;
    public NetworkVariable<int> WHITE_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<int> BLACK_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<bool> PlayerActed = new NetworkVariable<bool>(false);

    public Coordinate curSelected = Coordinate.none;
    public Vector2 temp;
    public List<Coordinate> curMovable;
    public List<PieceEnum> spawnList;

    [SerializeField] private Transform pieces;
    public Transform Pieces => pieces;

    public bool isGameOver = false;
    /// <summary>
    /// False when curPlayer is at Move Phase(Slide or Piece move)<br/>
    /// True when curPlayer is at Spawn Phase
    /// </summary>
    private int turnPhase = 0;
    public int TurnPhase
    {
        get => turnPhase;
        set
        {
            turnPhase = value;
            UIManager.Inst.SetTurnPhaseIndicator();
        }
    }

    //FIXME
    private void Awake()
    {
        Inst = this;
    }

    private void Update()
    {
        temp = new Vector2(curSelected.X, curSelected.Y);
    }

    public void SwapTurn()
    {
        if(curPlayer.Value == PlayerEnum.WHITE) 
            curPlayer.Value = PlayerEnum.BLACK;
        else curPlayer.Value = PlayerEnum.WHITE;

        PlayerActed.Value = false;
        // UIManager.Inst.SetTurnEndButton(false);

        turnPhase = 0;
        UIManager.Inst.SetTurnPhaseIndicator();
        
        curSelected = Coordinate.none;
        Board.Inst.ResetPaintedClientRpc();
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardPlayerState[coord.X, coord.Y] != compare && boardPlayerState[coord.X, coord.Y] != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardPlayerState[coord.X, coord.Y] != PlayerEnum.EMPTY;
    }

    public void GameOver(PlayerEnum winner)
    {
        isGameOver = true;
        Debug.Log($"{winner.ToString()} WINS!!");
        UIManager.Inst.SetResultPanel(winner);
    }

    public void Initialize()
    {
        boardState = new Checker[4,4];
        boardPlayerState = new PlayerEnum[4,4];
        boardPieceState = new PieceEnum[4,4];

        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardPlayerState[i, j] = PlayerEnum.EMPTY;
                boardPieceState[i, j] = PieceEnum.NONE;
            }
        }
    }

    public void ResetGame()
    {
        curSelected = Coordinate.none;
        curMovable = null;
        WHITE_Idx.Value = 0;
        BLACK_Idx.Value = 0;
        PlayerActed.Value = false;
        curPlayer.Value = PlayerEnum.BLACK;

        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardPlayerState[i, j] = PlayerEnum.EMPTY;
                boardPieceState[i, j] = PieceEnum.NONE;
            }
        }
        // should reset at client level

        UIManager.Inst.ResetUI();
        isGameOver = false;
    }

    public PlayerEnum GetPlayerState(Coordinate coord)
    {
        return boardPlayerState[coord.X, coord.Y];
    }

    public PieceEnum GetPieceState(Coordinate coord)
    {
        // Debug.Log(coord.ToString());
        return boardPieceState[coord.X, coord.Y];
    }

    public bool isSelectedAvailable()
    {
        return curSelected.X > -1 && curSelected.X < 4 && curSelected.Y > -1 && curSelected.Y < 4;
    }

    //Should only call from Server-Side
    public void RemovePiece(Vector2Int cor)
    {
        boardState[cor.x, cor.y].player.Value = PlayerEnum.EMPTY;
        boardState[cor.x, cor.y].piece.Value = PieceEnum.NONE;  
        boardPieceState[cor.x, cor.y] = PieceEnum.NONE;
        boardPlayerState[cor.x, cor.y] = PlayerEnum.EMPTY;
        // Board.Inst.UpdateSinglePieceClientRpc(new Vector2Int(cor.X, cor.Y));
    }

    public void SetPiece(Vector2Int cor, PlayerEnum player, PieceEnum piece)
    {
        Debug.Log(cor + "is setted to " + piece.ToString());
        boardState[cor.x, cor.y].player.Value = player;
        boardState[cor.x, cor.y].piece.Value = piece; 
        boardPlayerState[cor.x, cor.y] = player;
        boardPieceState[cor.x, cor.y] = piece;
        // Board.Inst.UpdateSinglePieceClientRpc(new Vector2Int(cor.X, cor.Y));
    }

    // [ClientRpc]
    // public void MovePieceClientRpc(int x, int y, int w, int z) => MovePieceClientRpc(new Coordinate(x,y), new Coordinate(w,z));
    // [ClientRpc]
    // public void MovePieceClientRpc(Coordinate src, Coordinate dest)
    // {
    //     if(src == dest) return;
    //     // Debug.Log(src.ToString() + " => " + dest.ToString());
    //     SetPieceClientRpc(dest, boardPlayerState[src.X, src.Y], boardPieceState[src.X, src.Y]);
    //     RemovePieceClientRpc(src);
    // }
}