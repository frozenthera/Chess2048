using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private void Update()
    {
        if(GameManager.Inst.isGameOver) return;
        if (!IsLocalPlayer) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
           GameManager.Inst.SwapTurn();
           UIManager.Inst.UpdateTurnEndButton();
           return;
        }

        if(GameManager.Inst.PlayerActed.Value) return;

        if(GameManager.Inst.TurnPhase < 1)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Slide Down!");
                Board.Inst.SlideServerRpc(Direction.DOWN);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Slide Right!");
                Board.Inst.SlideServerRpc(Direction.RIGHT);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Slide Up!");
                Board.Inst.SlideServerRpc(Direction.UP);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Slide Left!");
                Board.Inst.SlideServerRpc(Direction.LEFT);
                return;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);
            if(hit.transform == null) return;
            if(hit.transform.CompareTag("Checker"))
            {
                if(GameManager.Inst.TurnPhase < 3)
                {
                    ClickToSpawnPieceServerRpc(hit.transform.GetComponent<Checker>().coord.Value);
                }
                if(GameManager.Inst.TurnPhase < 2)
                {
                    ClickToMovePieceServerRpc(hit.transform.GetComponent<Checker>().coord.Value);
                    ClickMovablePieceServerRpc(hit.transform.GetComponent<Checker>().coord.Value);
                }
            }
        }
    }

    [ServerRpc]
    public void ClickToMovePieceServerRpc(Coordinate cor)
    {
        Checker dest = GameManager.Inst.boardState[cor.X, cor.Y];
        //Piece move procedure
        if(GameManager.Inst.curMovable == null) return;
        if(GameManager.Inst.curMovable.Count == 0) return;

        if(GameManager.Inst.curSelected != Coordinate.none)
        {
            Coordinate temp = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == dest.coord.Value.X && item.Y == dest.coord.Value.Y)
                {
                    if(GameManager.Inst.boardPlayerState[item.X, item.Y] != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.RemovePiece(new Vector2Int(item.X, item.Y));
                    }
                    // GameManager.Inst.MovePieceClientRpc(temp, cor);

                    if(temp != cor)
                    {
                        GameManager.Inst.SetPiece(new Vector2Int(temp.X, temp.Y), GameManager.Inst.GetPlayerState(cor), GameManager.Inst.GetPieceState(cor));
                        GameManager.Inst.RemovePiece(new Vector2Int(cor.X, cor.Y));
                    }

                    Board.Inst.ResetPaintedClientRpc();
                    GameManager.Inst.curSelected = new Coordinate(-1, -1);
                    GameManager.Inst.curMovable = null;

                    GameManager.Inst.TurnPhase = 2;
                }
            }
        }
    }

    [ServerRpc]
    public void ClickMovablePieceServerRpc(Coordinate cor)
    {
        GameManager.Inst.curSelected = cor;

        if(GameManager.Inst.curPlayer.Value != GameManager.Inst.GetPlayerState(cor)) return;
        if(GameManager.Inst.isSelectedAvailable())
        {
            GameManager.Inst.curMovable = null;
            Board.Inst.ResetPaintedClientRpc();
        } 

        if(GameManager.Inst.curSelected == cor) 
        {
            GameManager.Inst.curSelected = Coordinate.none;
            return;
        }

        GameManager.Inst.curMovable = Piece.ReachableCoordinate(cor, GameManager.Inst.boardPlayerState[cor.X, cor.Y], GameManager.Inst.boardPieceState[cor.X, cor.Y]);
        if(GameManager.Inst.curMovable.Count == 0)
        {
            GameManager.Inst.curSelected = Coordinate.none;
            return;
        }
        Board.Inst.PaintReachableClientRpc(CoordinateToVector2Int(GameManager.Inst.curMovable.ToArray()));
    }

    public Vector2Int[] CoordinateToVector2Int(Coordinate[] cor)
    {
        Vector2Int[] vec = new Vector2Int[cor.Length];
        for(int i=0; i<cor.Length; i++)
        {
            vec[i] = new Vector2Int(cor[i].X, cor[i].Y);
        }
        return vec;
    }

    [ServerRpc]
    public void ClickToSpawnPieceServerRpc(Coordinate cor)
    {
        Checker dest = GameManager.Inst.boardState[cor.X, cor.Y];
        //Piece spawn procedure
        // if(GameManager.Inst.GetPlayerState(dest.coord.Value) == PlayerEnum.EMPTY)
        if(GameManager.Inst.boardPlayerState[cor.X, cor.Y] == PlayerEnum.EMPTY)
        {
            if(GameManager.Inst.curPlayer.Value == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                GameManager.Inst.SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.WHITE, GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++]);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                GameManager.Inst.SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.BLACK, GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++]);
            }

            GameManager.Inst.PlayerActed.Value = true;
            // UIManager.Inst.SetTurnEndButton(true);
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPieceClientRpc();
        }
    }
}