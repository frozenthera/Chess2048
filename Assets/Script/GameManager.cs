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

    public Checker[,] boardState;
    
    public NetworkVariable<PlayerEnum[,]> boardPlayerState = new();
    public NetworkVariable<PieceEnum[,]> boardPieceState = new();
    public NetworkVariable<int> WHITE_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<int> BLACK_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<bool> PlayerActed = new NetworkVariable<bool>(false);

    public Coordinate curSelected = Coordinate.none;
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
        boardState = new Checker[4,4];
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
        Board.Inst.ResetPainted();
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardPlayerState.Value[coord.X, coord.Y] != compare && boardPlayerState.Value[coord.X, coord.Y] != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardPlayerState.Value[coord.X, coord.Y] != PlayerEnum.EMPTY;
    }

    public void GameOver(PlayerEnum winner)
    {
        isGameOver = true;
        Debug.Log($"{winner.ToString()} WINS!!");
        UIManager.Inst.SetResultPanel(winner);
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
                boardPlayerState.Value[i,j] = PlayerEnum.EMPTY;
                boardPieceState.Value[i,j] = PieceEnum.NULL;
            }
        }
        UIManager.Inst.ResetUI();
        isGameOver = false;
    }

    public void SetPlayerState(Coordinate coord, PlayerEnum playerEnum)
    {
        boardPlayerState.Value[coord.X, coord.Y] = playerEnum;
    }

    public PlayerEnum GetPlayerState(Coordinate coord)
    {
        return boardPlayerState.Value[coord.X, coord.Y];
    }

    public void SetPieceState(Coordinate coord, PieceEnum pieceEnum)
    {
        boardPieceState.Value[coord.X, coord.Y] = pieceEnum;
    }
    public PieceEnum GetPieceState(Coordinate coord)
    {
        return boardPieceState.Value[coord.X, coord.Y];
    }

    public bool isSelectedAvailable()
    {
        return curSelected.X > -1 && curSelected.X < 4 && curSelected.Y > -1 && curSelected.Y < 4;
    }
}
