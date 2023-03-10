using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckerStateEnum
{
    BLACK,
    WHITE,
    EMPTY
}

public enum PieceEnum
{
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING,
    EMPTY
}

public class Checker : MonoBehaviour
{
    public CheckerStateEnum curCheckerPlayer { get; set; }
    public Coordinate coord { get; set; }
    public Piece curPiece {get; set; }

    public void OnMouseDown()
    {
        Spawn(PieceEnum.PAWN);
    }

    private void Spawn(PieceEnum pieceEnum)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(pieceEnum);
        curPiece = Instantiate(go, transform.position, Quaternion.identity).GetComponent<Piece>();

    }
}