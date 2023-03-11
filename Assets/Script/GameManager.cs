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
    
    public bool isHighlighted = false;
    public Piece curSelected = null;

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
        return pieceDict[pieceEnum];
    }

    public void SwapTurn()
    {
        if(player == PlayerEnum.WHITE) 
            player = PlayerEnum.BLACK;
        else player = PlayerEnum.WHITE;
    }

    public bool isTherePieceWithOppo(Coordinate coord, PlayerEnum compare)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer != compare && boardState[coord.X, coord.Y].curCheckerPlayer != PlayerEnum.EMPTY; 
    }

    public bool isTherePiece(Coordinate coord)
    {
        return boardState[coord.X, coord.Y].curCheckerPlayer != PlayerEnum.EMPTY;
    }
}
