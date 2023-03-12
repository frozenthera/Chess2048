using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : NetworkBehaviour
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
        curPiece.GetComponent<NetworkObject>().Despawn();
        // Destroy(curPiece.gameObject);

        curCheckerPlayer = PlayerEnum.EMPTY;
        curPiece = null;
    }

    public void OnMouseDown()
    {
        if(GameManager.Inst.PlayerActed.Value || GameManager.Inst.isGameOver) return;

        if(GameManager.Inst.curSelected != null && GameManager.Inst.TurnPhase < 2)
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

                    GameManager.Inst.TurnPhase = 2;
                }
            }
            return;
        }

        if(curCheckerPlayer == PlayerEnum.EMPTY && GameManager.Inst.TurnPhase < 3)
        {
            if(GameManager.Inst.player == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++]);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++]);
            }

            GameManager.Inst.PlayerActed.Value = true;
            // UIManager.Inst.SetTurnEndButton(true);
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPiece();
        }
    }

    private void Spawn(PieceEnum pieceEnum)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(pieceEnum);
        curPiece = Instantiate(go, transform.position, Quaternion.identity, GameManager.Inst.Pieces).GetComponent<Piece>();
        curPiece.GetComponent<NetworkObject>().Spawn();

        curCheckerPlayer = GameManager.Inst.player;
        curPiece.curCoord = coord;
        curPiece.Initialize(GameManager.Inst.player);
    }

    public void Paint(Color color)
    {
        sp.color = color;
    }
}