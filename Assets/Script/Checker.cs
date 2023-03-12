using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : NetworkBehaviour
{
    public NetworkVariable<PlayerEnum> curCheckerPlayer = new NetworkVariable<PlayerEnum>(PlayerEnum.EMPTY);
    public NetworkVariable<Coordinate> coord { get; set; }
    public NetworkVariable<Piece> curPiece = new();
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

        curPiece.Value.transform.position = dest.transform.position;
        curPiece.Value.curCoordX = dest.coord.Value.X;
        curPiece.Value.curCoordY = dest.coord.Value.Y;

        dest.curCheckerPlayer = curCheckerPlayer;
        dest.curPiece = curPiece;

        curCheckerPlayer.Value = PlayerEnum.EMPTY;
        curPiece = null;
    }

    public void RemovePiece()
    {
        curPiece.Value.GetComponent<NetworkObject>().Despawn();
        // Destroy(curPiece.gameObject);

        curCheckerPlayer.Value = PlayerEnum.EMPTY;
        curPiece = null;
    }

    public void OnMouseDown()
    {
        if(GameManager.Inst.PlayerActed || GameManager.Inst.isGameOver) return;

        if(GameManager.Inst.curSelected != null && GameManager.Inst.TurnPhase < 2)
        {
            Piece temp = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == this.coord.Value.X && item.Y == this.coord.Value.Y)
                {
                    if(GameManager.Inst.boardState[item.X, item.Y].curCheckerPlayer.Value != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.boardState[item.X, item.Y].RemovePiece();
                    }
                    GameManager.Inst.boardState[temp.curCoordX, temp.curCoordY].MovePiece(this);
                    Board.Inst.ResetPainted();
                    GameManager.Inst.curSelected = null;
                    GameManager.Inst.curMovable = null;

                    GameManager.Inst.TurnPhase = 2;
                }
            }
            return;
        }

        if(curCheckerPlayer.Value == PlayerEnum.EMPTY && GameManager.Inst.TurnPhase < 3)
        {
            if(GameManager.Inst.curPlayer.Value == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++]);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                Spawn(GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++]);
            }

            GameManager.Inst.PlayerActed = true;
            // UIManager.Inst.SetTurnEndButton(true);
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPiece();
        }
    }

    private void Spawn(PieceEnum pieceEnum)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(pieceEnum);
        curPiece.Value = Instantiate(go, transform.position, Quaternion.identity, GameManager.Inst.Pieces).GetComponent<Piece>();
        curPiece.Value.GetComponent<NetworkObject>().Spawn();

        curCheckerPlayer.Value = GameManager.Inst.curPlayer.Value;
        curPiece.Value.curCoordX = coord.Value.X;
        curPiece.Value.curCoordY = coord.Value.Y;
        curPiece.Value.Initialize(GameManager.Inst.curPlayer.Value);
    }

    public void Paint(Color color)
    {
        sp.color = color;
    }
}