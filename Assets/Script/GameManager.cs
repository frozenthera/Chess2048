using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //FIXME_Later move to Singleton<>
    public static GameManager Inst;

    [SerializeField] private List<GameObject> piecePrefabs = new();
    private Dictionary<PieceEnum, GameObject> pieceDict = new();

    public PlayerEnum player = PlayerEnum.WHITE;
    public Checker[,] boardState;

    public int WHITE_Idx = 0;
    public int BLACK_Idx = 0;

    public bool PlayerActed = false;

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
    private bool turnPhase = false;
    public bool TurnPhase
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
        if(player == PlayerEnum.WHITE) 
            player = PlayerEnum.BLACK;
        else player = PlayerEnum.WHITE;

        PlayerActed = false;
        UIManager.Inst.SetTurnEndButton(false);
        
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                if(boardState[i,j].curCheckerPlayer == PlayerEnum.EMPTY) continue;
                else if(boardState[i,j].curCheckerPlayer == player) boardState[i,j].curPiece.GetComponent<BoxCollider2D>().enabled = true;
                else boardState[i,j].curPiece.GetComponent<BoxCollider2D>().enabled = false;
            }
        }
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer != compare && boardState[coord.X, coord.Y].curCheckerPlayer != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer != PlayerEnum.EMPTY;
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
        WHITE_Idx = 0;
        BLACK_Idx = 0;
        PlayerActed = false;
        player = PlayerEnum.WHITE;
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                boardState[i,j].curCheckerPlayer = PlayerEnum.EMPTY;
                boardState[i,j].curPiece = null;
            }
        }
        foreach(var item in Pieces.GetComponentsInChildren<Transform>())
        {
            if(item == Pieces) continue;
            Destroy(item.gameObject);
        }
        UIManager.Inst.ResetUI();
        isGameOver = false;
    }
}
