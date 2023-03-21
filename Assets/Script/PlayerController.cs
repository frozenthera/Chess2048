using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{

    private bool spawnDone = false;

    public override void OnNetworkSpawn()
    {
        if(!IsLocalPlayer) return;
        GameManager.Inst.localPlayer = this;
    }

    private void Update()
    {
        if(GameManager.Inst.isGameOver) return;
        if (!IsLocalPlayer) return;

        if(Input.GetKeyDown(KeyCode.E))
        {
           SwapTurnServerRpc();
           UIManager.Inst.UpdateTurnEndButton();
           return;
        }

        if(GameManager.Inst.PlayerActed.Value) return;

        if(GameManager.Inst.TurnPhase < 1)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Slide Down!");
                SlideServerRpc(Direction.DOWN);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Debug.Log("Slide Right!");
                SlideServerRpc(Direction.RIGHT);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("Slide Up!");
                SlideServerRpc(Direction.UP);
                return;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Debug.Log("Slide Left!");
                SlideServerRpc(Direction.LEFT);
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
                    ClickMovablePieceServerRpc(hit.transform.GetComponent<Checker>().coord.Value);
                    ClickToMovePieceServerRpc(hit.transform.GetComponent<Checker>().coord.Value);
                }
            }
        }
    }
    
    [ServerRpc]
    public void SlideServerRpc(Direction dir)
    {
        Board.Inst.SlideServerRpc(dir);
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
            Coordinate src = GameManager.Inst.curSelected;
            foreach(var item in GameManager.Inst.curMovable)
            {
                if(item.X == dest.coord.Value.X && item.Y == dest.coord.Value.Y)
                {
                    if(GameManager.Inst.boardPlayerState[item.X, item.Y] != PlayerEnum.EMPTY)
                    {
                        GameManager.Inst.RemovePiece(new Vector2Int(item.X, item.Y));
                    }
                    // GameManager.Inst.MovePieceClientRpc(temp, cor);

                    if(src != cor)
                    {
                        GameManager.Inst.SetPiece(new Vector2Int(cor.X, cor.Y), GameManager.Inst.GetPlayerState(src), GameManager.Inst.GetPieceState(src));
                        GameManager.Inst.RemovePiece(new Vector2Int(src.X, src.Y));
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
        //빈칸을 선택하는 경우
        if(GameManager.Inst.GetPlayerState(cor) == PlayerEnum.EMPTY) return;
        //상대의 기물을 선택하는 경우
        if(GameManager.Inst.curPlayer.Value != GameManager.Inst.GetPlayerState(cor)) return;

        //현재 선택된 칸을 선택하는 경우
        if(GameManager.Inst.curSelected == cor)
        {
            GameManager.Inst.curMovable.Clear();
            GameManager.Inst.curSelected = Coordinate.none;
            Board.Inst.ResetPaintedClientRpc();
            return;
        }

        //자신의 기물을 선택하였을 때 이동가능한 부분 표시
        GameManager.Inst.curSelected = cor;
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
        if(GameManager.Inst.curSelected != Coordinate.none) return;

        Checker dest = GameManager.Inst.boardState[cor.X, cor.Y];
        //Piece spawn procedure
        // if(GameManager.Inst.GetPlayerState(dest.coord.Value) == PlayerEnum.EMPTY)
        if(GameManager.Inst.boardPlayerState[cor.X, cor.Y] == PlayerEnum.EMPTY)
        {
            if(GameManager.Inst.curPlayer.Value == PlayerEnum.WHITE)
            {
                if(GameManager.Inst.WHITE_Idx.Value > 15) return;
                GameManager.Inst.SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.WHITE, GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value++]);
                spawnDone = true;
            }
            else
            {
                if(GameManager.Inst.BLACK_Idx.Value > 15) return;
                GameManager.Inst.SetPiece(new Vector2Int(cor.X, cor.Y), PlayerEnum.BLACK, GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value++]);
                spawnDone = true;
            }

            GameManager.Inst.PlayerActed.Value = true;
            GameManager.Inst.TurnPhase = 3;
            UIManager.Inst.UpdateNextPieceClientRpc();
        }
    }

    [ServerRpc]
    public void SwapTurnServerRpc()
    {
        GameManager.Inst.SwapTurn();
    }

    private void LateUpdate() 
    {
        if(!IsServer) return;
        if(spawnDone)
        {
            Board.Inst.ResetPaintedClientRpc();
            spawnDone = false;
        }    
    }
}