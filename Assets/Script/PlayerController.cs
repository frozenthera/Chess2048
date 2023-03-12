using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    private void Update()
    {
        if(GameManager.Inst.isGameOver) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
           GameManager.Inst.SwapTurn();
           UIManager.Inst.UpdateTurnEndButton();
           return;
        }

        if(GameManager.Inst.PlayerActed) return;

        if(GameManager.Inst.TurnPhase > 0) return;

        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Slide Down!");
            Board.Inst.Slide(Direction.DOWN);
            GameManager.Inst.TurnPhase = 1;
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Slide Right!");
            Board.Inst.Slide(Direction.RIGHT);
            GameManager.Inst.TurnPhase = 1;
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Slide Up!");
            Board.Inst.Slide(Direction.UP);
            GameManager.Inst.TurnPhase = 1;
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Slide Left!");
            Board.Inst.Slide(Direction.LEFT);
            GameManager.Inst.TurnPhase = 1;
        }
    }


}
 