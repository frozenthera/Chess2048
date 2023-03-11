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
        if(GameManager.Inst.PlayerActed || GameManager.Inst.isGameOver) return;

        if(GameManager.Inst.curSelected != null)
        {
            Piece temp = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == this.coord.X && item.Y == this.coord.Y)
                {
                    if(GameManager.Inst.boardState[item.X, item.Y].curCheckerPlayer != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.boardState[item.X, item.Y].RemovePiece();
                    }
                    GameManager.Inst.boardState[temp.curCoord.X, temp.curCoord.Y].MovePiece(this);
                    Board.Inst.ResetPainted();
                    GameManager.Inst.curSelected = null;
                    GameManager.Inst.curMovable = null;

                    GameManager.Inst.PlayerActed = true;
                    UIManager.Inst.SetTurnEndButton(true);
                }
            }
            return;
        }

        if(curCheckerPlayer == PlayerEnum.EMPTY)
        {
            if(GameManager.Inst.player == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx++]);

                GameManager.Inst.PlayerActed = true;
                UIManager.Inst.SetTurnEndButton(true);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx++]);
                
                GameManager.Inst.PlayerActed = true;
                UIManager.Inst.SetTurnEndButton(true);
            }
            UIManager.Inst.UpdateNextPiece();
        }
    }

    private void Spawn(PieceEnum pieceEnum)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(pieceEnum);
        curPiece = Instantiate(go, transform.position, Quaternion.identity, GameManager.Inst.Pieces).GetComponent<Piece>();

        curCheckerPlayer = GameManager.Inst.player;
        curPiece.curCoord = coord;
        curPiece.Initialize(GameManager.Inst.player);
    }

    public void Paint(Color color)
    {
        sp.color = color;
    }
}