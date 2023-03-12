using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    //FIXME_Later move to Singleton<>
    public static GameManager Inst;

    [SerializeField] private List<GameObject> piecePrefabs = new();
    private Dictionary<PieceEnum, GameObject> pieceDict = new();

    public NetworkVariable<PlayerEnum> player = new NetworkVariable<PlayerEnum>(PlayerEnum.BLACK);
    public Checker[,] boardState;

    public NetworkVariable<int> WHITE_Idx = new NetworkVariable<int>(0);
    public NetworkVariable<int> BLACK_Idx = new NetworkVariable<int>(0);

    public NetworkVariable<bool> PlayerActed = new NetworkVariable<bool>(false);

    public bool isHighlighted
    {
        get => curSelected != null;
    }

    public Piece curSelected = null;
    public List<Coordinate> curMovable;
    public List<PieceEnum> spawnList;

    [SerializeField] private Transform pieces;
    public Transform Pieces => pieces;

    public bool isGameOver = false;
    /// <summary>
    /// False when player is at Move Phase(Slide or Piece move)<br/>
    /// True when player is at Spawn Phase
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

    private void Start()
    {
        pieceDict = new();
        foreach(var item in piecePrefabs)
        {
            pieceDict.Add(item.GetComponent<Piece>().pieceClass, item);
        }
    }

    public GameObject GetObjectByPieceEnum(PieceEnum pieceEnum)
    {
        if(pieceEnum == PieceEnum.NULL) return null;
        return pieceDict[pieceEnum];
    }

    public void SwapTurn()
    {
        if(player.Value == PlayerEnum.WHITE) 
            player.Value = PlayerEnum.BLACK;
        else player.Value = PlayerEnum.WHITE;

        PlayerActed.Value = false;
        // UIManager.Inst.SetTurnEndButton(false);

        turnPhase = 0;
        UIManager.Inst.SetTurnPhaseIndicator();
        
        curSelected = null;
        Board.Inst.ResetPainted();

        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                if(boardState[i,j].curCheckerPlayer.Value == PlayerEnum.EMPTY) continue;
                else if(boardState[i,j].curCheckerPlayer.Value == player.Value) boardState[i,j].curPiece.GetComponent<BoxCollider2D>().enabled = true;
                else boardState[i,j].curPiece.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer.Value != compare && boardState[coord.X, coord.Y].curCheckerPlayer.Value != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer.Value != PlayerEnum.EMPTY;
    }

    public void GameOver(PlayerEnum winner)
    {
        isGameOver = true;
        Debug.Log($"{winner.ToString()} WINS!!");
        UIManager.Inst.SetResultPanel(winner);
    }

    public void ResetGame()
    {
        curSelected = null;
        curMovable = null;
        WHITE_Idx.Value = 0;
        BLACK_Idx.Value = 0;
        PlayerActed.Value = false;
        player.Value = PlayerEnum.BLACK;
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardState[i,j].curCheckerPlayer.Value = PlayerEnum.EMPTY;
                boardState[i,j].curPiece = null;
            }
        }
        foreach(var item in Pieces.GetComponentsInChildren<Transform>())
        {
            if(item == Pieces) continue;

            // Destroy(item.gameObject);
            item.GetComponent<NetworkObject>().Despawn();
        }
        UIManager.Inst.ResetUI();
        isGameOver = false;
    }
}
