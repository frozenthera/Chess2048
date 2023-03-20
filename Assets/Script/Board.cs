using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Board : NetworkBehaviour
{
    public static Board Inst;
    [SerializeField] private GameObject squarePrefab;

    private void Awake()
    {
        Inst = this;
    }

    public void Initialize()
    {
        for(int i=0; i<4; i++) 
        {
            for(int j=0; j<4; j++)
            {
                Vector2 pos = new Vector2(i, j)*2;
                Checker checker = GameManager.Inst.boardState[i,j] = Instantiate(squarePrefab, pos, Quaternion.identity, transform).GetComponent<Checker>();
                checker.name = i + ", " + j;
                checker.coord = new NetworkVariable<Coordinate>(new Coordinate(i,j));
                if((i+j) % 2 == 0)
                {
                    checker.GetComponent<SpriteRenderer>().color = new Color(60/255f,60/255f,60/255f);
                }
                checker.GetComponent<NetworkObject>().Spawn();
                checker.transform.parent = this.transform;
            }
        }
        transform.position = new Vector3(-2f, -3f, 0);
    }

    public override void OnNetworkSpawn()
    {
        if(IsServer) return;
        transform.localScale = new Vector3(1,1,1);
    }

    int[] dx = new int[]{0, 1, 0, -1};
    int[] dy = new int[]{-1, 0, 1, 0};

    [ServerRpc]
    public void SlideServerRpc(Direction dir)
    {
        GameManager.Inst.TurnPhase = 1;

        int _dx = dx[(int)dir];
        int _dy = dy[(int)dir];

        for(int i= _dx < 0 ? 0 : 3; _dx < 0 ? i<4 : i>-1 ; i -= _dx != 0 ? _dx : 1)
        {
            for(int j= _dy < 0 ? 0 : 3; _dy < 0 ? j<4 : j>-1 ; j -= _dy != 0 ? _dy : 1)
            {
                if(i+_dx < 0 || i+_dx > 3 || j+_dy < 0 || j+_dy > 3) continue;

                if(GameManager.Inst.boardPlayerState[i, j] == PlayerEnum.EMPTY) continue;
                
                int temp = 1;
                while(i+_dx*temp > -1 && i+_dx*temp < 4 && j+_dy*temp > -1 && j+_dy*temp < 4 && GameManager.Inst.boardPieceState[i+_dx*temp, j+_dy*temp] == PieceEnum.NONE)
                {
                    temp++;
                }
                // GameManager.Inst.MovePieceClientRpc(i, j, i+_dx*(temp-1), j+_dy*(temp-1));

                Coordinate src = new Coordinate(i,j);
                Coordinate dest = new Coordinate(i+_dx*(temp-1), j+_dy*(temp-1));
                
                if(src != dest)
                {
                    GameManager.Inst.SetPiece(new Vector2Int(dest.X, dest.Y), GameManager.Inst.GetPlayerState(src), GameManager.Inst.GetPieceState(src));
                    GameManager.Inst.RemovePiece(new Vector2Int(src.X, src.Y));
                }

                
                int changedX = i+_dx*(temp-1);
                int changedY = j+_dy*(temp-1);

                if(changedX+_dx < 0 || changedX+_dx > 3 || changedY+_dy < 0 || changedY+_dy > 3) continue;
                if(GameManager.Inst.boardPieceState[changedX+_dx, changedY+_dy] != GameManager.Inst.boardPieceState[changedX, changedY]) continue;

                if(GameManager.Inst.boardPlayerState[changedX+_dx, changedY+_dy] == GameManager.Inst.boardPlayerState[changedX, changedY])
                {
                    GameManager.Inst.SetPiece(new Vector2Int(changedX+_dx, changedY+_dy), GameManager.Inst.GetPlayerState(new Coordinate(changedX+_dx, changedY+_dy)), GameManager.Inst.boardPieceState[changedX, changedY] + 1);
                    GameManager.Inst.RemovePiece(new Vector2Int(changedX, changedY));
                }
            }
        }
        // UpdateEveryPieceClientRpc();
    }

    [ClientRpc]
    public void PaintReachableClientRpc(Vector2Int[] coordList)
    {
        if(coordList.Length == 0) return;
        foreach(var item in coordList)
        {
            GameManager.Inst.boardState[item.x, item.y].PaintBackground(Color.green);
        }
    }

    [ClientRpc]
    public void ResetPaintedClientRpc()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                GameManager.Inst.boardState[i,j].PaintBackground( (i+j)%2==0 ? new Color(60/255f,60/255f,60/255f) : new Color(200/255f,200/255f,200/255f));
            }
        }
    }

    [ClientRpc]
    public void UpdateEveryPieceClientRpc()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                GameManager.Inst.boardState[i,j].PaintPiece();
            }
        }
    }

    [ClientRpc]
    public void UpdateSinglePieceClientRpc(Vector2Int coor)
    {
        // Debug.Log(coor.ToString());
        GameManager.Inst.boardState[coor.x, coor.y].PaintPiece();
    }
}