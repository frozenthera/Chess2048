using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : MonoBehaviour
{
    private SpriteRenderer sp;
    public Coordinate coord;
    [SerializeField] private SpriteRenderer pieceRenderer;
    [SerializeField] private List<Sprite> whitePieceSprite;
    [SerializeField] private List<Sprite> blackPieceSprite;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    public void OnMouseDown()
    {
        ClickMovablePiece();
        ClickVoidChecker();
    }

    public void ClickVoidChecker()
    {
        if(GameManager.Inst.PlayerActed.Value || GameManager.Inst.isGameOver) return;

        if(GameManager.Inst.curSelected != Coordinate.none && GameManager.Inst.TurnPhase < 2)
        {
            Coordinate temp = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == this.coord.X && item.Y == this.coord.Y)
                {
                    if(GameManager.Inst.boardPlayerState.Value[item.X, item.Y] != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.boardState[item.X, item.Y].RemovePiece();
                    }
                    GameManager.Inst.boardState[temp.X, temp.Y].MovePiece(this);
                    Board.Inst.ResetPainted();
                    GameManager.Inst.curSelected = new Coordinate(-1, -1);
                    GameManager.Inst.curMovable = null;

                    GameManager.Inst.TurnPhase = 2;
                }
            }
            return;
        }

        if(GameManager.Inst.GetPlayerState(coord) == PlayerEnum.EMPTY && GameManager.Inst.TurnPhase < 3)
        {
            if(GameManager.Inst.curPlayer.Value == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                SetPiece(GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++], PlayerEnum.WHITE);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                SetPiece(GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++], PlayerEnum.BLACK);
            }

            GameManager.Inst.PlayerActed.Value = true;
            // UIManager.Inst.SetTurnEndButton(true);
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPiece();
        }
    }

    public void ClickMovablePiece()
    {
        if(GameManager.Inst.curPlayer.Value != GameManager.Inst.GetPlayerState(coord)) return;
        if(GameManager.Inst.PlayerActed.Value) return;
        if(GameManager.Inst.TurnPhase > 2) return;

        if(GameManager.Inst.isSelectedAvailable())
        {
            GameManager.Inst.curMovable = null;
            Board.Inst.ResetPainted();
        } 

        if(GameManager.Inst.GetPieceState(GameManager.Inst.curSelected) == GameManager.Inst.GetPieceState(coord)) 
        {
            GameManager.Inst.curSelected = Coordinate.none;
            return;
        }

        GameManager.Inst.curSelected = coord;
        // GameManager.Inst.curMovable = ReachableCoordinate();
        if(GameManager.Inst.curMovable.Count == 0)
        {
            GameManager.Inst.curSelected = Coordinate.none;
            return;
        }
        // Board.Inst.PaintReachable(ReachableCoordinate());
    }

    public void SetPiece(PieceEnum pieceEnum, PlayerEnum playerEnum)
    {
        GameManager.Inst.SetPieceState(coord, pieceEnum);
        GameManager.Inst.SetPlayerState(coord, playerEnum);

        PaintPiece();
    }

    public void RemovePiece()
    {
        GameManager.Inst.SetPieceState(coord, PieceEnum.NULL);
        GameManager.Inst.SetPlayerState(coord, PlayerEnum.EMPTY);

        PaintPiece();
    }

    public void MovePiece(Checker dest)
    {
        if(dest == this) return;

        dest.SetPiece(GameManager.Inst.GetPieceState(coord), GameManager.Inst.GetPlayerState(coord));
        RemovePiece();

        dest.PaintPiece();
        PaintPiece();
    }

    public void PaintBackground(Color color)
    {
        sp.color = color;
    }

    public void PaintPiece()
    {
        if(GameManager.Inst.GetPlayerState(coord) == PlayerEnum.EMPTY) pieceRenderer.sprite = null;
        
        if(GameManager.Inst.GetPlayerState(coord) == PlayerEnum.WHITE)
            pieceRenderer.sprite = whitePieceSprite[(int)GameManager.Inst.GetPieceState(coord)];

        else pieceRenderer.sprite = blackPieceSprite[(int)GameManager.Inst.GetPieceState(coord)]; 
    }
}