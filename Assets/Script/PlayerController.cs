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

        if(GameManager.Inst.TurnPhase > 0) return;

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Slide Down!");
            Board.Inst.Slide(Direction.DOWN);
            GameManager.Inst.TurnPhase = 1;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Slide Right!");
            Board.Inst.Slide(Direction.RIGHT);
            GameManager.Inst.TurnPhase = 1;
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Slide Up!");
            Board.Inst.Slide(Direction.UP);
            GameManager.Inst.TurnPhase = 1;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Slide Left!");
            Board.Inst.Slide(Direction.LEFT);
            GameManager.Inst.TurnPhase = 1;
        }

        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if(hit.transform.CompareTag("Checker"))
                {
                    ClickMovablePiece(hit.transform.GetComponent<Checker>());
                    ClickVoidChecker(hit.transform.GetComponent<Checker>());
                }
            }
        }
    }

    public void ClickVoidChecker(Checker dest)
    {
        if(GameManager.Inst.PlayerActed.Value || GameManager.Inst.isGameOver) return;

        if(GameManager.Inst.curSelected != Coordinate.none && GameManager.Inst.TurnPhase < 2)
        {
            Coordinate temp = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == dest.coord.X && item.Y == dest.coord.Y)
                {
                    if(GameManager.Inst.boardPlayerState.Value[item.X, item.Y] != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.boardState[item.X, item.Y].RemovePiece();
                    }
                    GameManager.Inst.boardState[temp.X, temp.Y].MovePiece(dest);
                    Board.Inst.ResetPainted();
                    GameManager.Inst.curSelected = new Coordinate(-1, -1);
                    GameManager.Inst.curMovable = null;

                    GameManager.Inst.TurnPhase = 2;
                }
            }
            return;
        }

        if(GameManager.Inst.GetPlayerState(dest.coord) == PlayerEnum.EMPTY && GameManager.Inst.TurnPhase < 3)
        {
            if(GameManager.Inst.curPlayer.Value == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                GameManager.Inst.SetPieceState(dest.coord, GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++]);
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                GameManager.Inst.SetPieceState(dest.coord, GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++]);
            }

            GameManager.Inst.PlayerActed.Value = true;
            // UIManager.Inst.SetTurnEndButton(true);
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPiece();
        }
    }

    public void ClickMovablePiece(Checker dest)
    {
        if(GameManager.Inst.curPlayer.Value != GameManager.Inst.GetPlayerState(dest.coord)) return;
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


}
 