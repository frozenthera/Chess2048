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

    private void Start()
    {
        // Initialize();
    }

    public void Initialize()
    {
        for(int i=0; i<4; i++) 
        {
            for(int j=0; j<4; j++)
            {
                Vector2 pos = new Vector2(i, j);
                Checker checker = GameManager.Inst.boardState[i,j] = Instantiate(squarePrefab, pos, Quaternion.identity, transform).GetComponent<Checker>();
                checker.name = i + ", " + j;
                if((i+j) % 2 == 0)
                {
                    checker.GetComponent<SpriteRenderer>().color = new Color(60/255f,60/255f,60/255f);
                }
                checker.GetComponent<NetworkObject>().Spawn();
                checker.transform.parent = this.transform;
            }
        }

        transform.localScale = new Vector3(2,2,1);
        transform.position = new Vector3(-2f, -3f, 0);
    }

    int[] dx = new int[]{0, 1, 0, -1};
    int[] dy = new int[]{-1, 0, 1, 0};
    public void Slide(Direction dir)
    {
        int _dx = dx[(int)dir];
        int _dy = dy[(int)dir];

        for(int i= _dx < 0 ? 0 : 3; _dx < 0 ? i<4 : i>-1 ; i -= _dx != 0 ? _dx : 1)
        {
            for(int j= _dy < 0 ? 0 : 3; _dy < 0 ? j<4 : j>-1 ; j -= _dy != 0 ? _dy : 1)
            {
                if(i+_dx < 0 || i+_dx > 3 || j+_dy < 0 || j+_dy > 3) continue;

                if(GameManager.Inst.boardPlayerState.Value[i,j] == PlayerEnum.EMPTY) continue;
                
                int temp = 1;
                while(i+_dx*temp > -1 && i+_dx*temp < 4 && j+_dy*temp > -1 && j+_dy*temp <4 && GameManager.Inst.boardPieceState.Value[i+_dx*temp, j+_dy*temp] == PieceEnum.NULL)
                {
                    temp++;
                }
                GameManager.Inst.boardState[i,j].MovePiece(GameManager.Inst.boardState[i+_dx*(temp-1), j+_dy*(temp-1)]);
                
                int changedX = i+_dx*(temp-1);
                int changedY = j+_dy*(temp-1);

                if(changedX+_dx < 0 || changedX+_dx > 3 || changedY+_dy < 0 || changedY+_dy > 3) continue;
                if(GameManager.Inst.boardPieceState.Value[changedX+_dx, changedY+_dy] != GameManager.Inst.boardPieceState.Value[changedX, changedY]) continue;

                if(GameManager.Inst.boardPlayerState.Value[changedX+_dx, changedY+_dy] == GameManager.Inst.boardPlayerState.Value[changedX, changedY])
                {
                    Merge(new Coordinate(changedX+_dx, changedY+_dy), new Coordinate(changedX, changedY));

                    GameManager.Inst.boardPieceState.Value[changedX, changedY] = PieceEnum.NULL;
                    GameManager.Inst.boardPlayerState.Value[changedX, changedY] = PlayerEnum.EMPTY;

                }
                // else
                // {
                //     //Destroying cruncing piece
                //     GameManager.Inst.boardState[changedX+_dx, changedY+_dy].RemovePiece();
                //     GameManager.Inst.boardState[changedX, changedY].MovePiece(GameManager.Inst.boardState[changedX+_dx, changedY+_dy]);
                // }
            }
        }
    }

    /// <summary>
    /// Merge p1 and p2 into next step of piece at p1's position
    /// </summary>
    /// <param name="p1">Merged piece</param>
    /// <param name="p2">Merging piece</param>
    public void Merge(Coordinate p1, Coordinate p2)
    {
        GameManager.Inst.SetPlayerState(p2, PlayerEnum.EMPTY);
        GameManager.Inst.SetPieceState(p1, GameManager.Inst.boardPieceState.Value[p1.X, p2.Y] + 1);
        GameManager.Inst.SetPieceState(p2, PieceEnum.NULL);
    }

    public void PaintReachable(List<Coordinate> coordList)
    {
        if(coordList == null) return;
        foreach(var item in coordList)
        {
            GameManager.Inst.boardState[item.X, item.Y].PaintBackground(Color.green);
        }
    }

    public void ResetPainted()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                GameManager.Inst.boardState[i,j].PaintBackground( (i+j)%2==0 ? new Color(60/255f,60/255f,60/255f) : new Color(200/255f,200/255f,200/255f));
            }
        }
    }

    public void UpdatePiecePaint()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                GameManager.Inst.boardState[i,j].PaintPiece();
            }
        }
    }

}