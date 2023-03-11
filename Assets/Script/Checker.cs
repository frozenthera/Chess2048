using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checker : MonoBehaviour
{
    public PlayerEnum curCheckerPlayer;
    public Coordinate coord { get; set; }
    public Piece curPiece;
    private SpriteRenderer sp;
    
    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// Use only when dest is empty
    /// </summary>
    /// <param name="dest"></param>
    public void MovePiece(Checker dest)
    {
        if(dest == this) return;

        curPiece.transform.position = dest.transform.position;
        curPiece.curCoord = dest.coord;

        dest.curCheckerPlayer = curCheckerPlayer;
        dest.curPiece = curPiece;

        curCheckerPlayer = PlayerEnum.EMPTY;
        curPiece = null;
    }

    public void RemovePiece()
    {
        Destroy(curPiece.gameObject);
        curCheckerPlayer = PlayerEnum.EMPTY;
        curPiece = null;
    }


    public void OnMouseDown()
    {
        if(curCheckerPlayer != PlayerEnum.EMPTY) return;

        Spawn(PieceEnum.PAWN);
    }

    private void Spawn(PieceEnum pieceEnum)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(pieceEnum);
        curPiece = Instantiate(go, transform.position, Quaternion.identity, transform.parent).GetComponent<Piece>();

        curCheckerPlayer = GameManager.Inst.player;
        curPiece.curCoord = coord;
        curPiece.Initialize(GameManager.Inst.player);
    }

    public void Paint(Color color)
    {
        sp.color = color;
    }
}