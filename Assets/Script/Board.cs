using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board Inst;
    [SerializeField] private GameObject squarePrefab;

    private void Awake()
    {
        Inst = this;
    }

    private void Start()
    {
        Initialize();
    }

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

        if(Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Slide Down!");
            Slide(Direction.DOWN);
            GameManager.Inst.PlayerActed = true;
            UIManager.Inst.SetTurnEndButton(true);
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Slide Right!");
            Slide(Direction.RIGHT);
            GameManager.Inst.PlayerActed = true;
            UIManager.Inst.SetTurnEndButton(true);
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Slide Up!");
            Slide(Direction.UP);
            GameManager.Inst.PlayerActed = true;
            UIManager.Inst.SetTurnEndButton(true);
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Slide Left!");
            Slide(Direction.LEFT);
            GameManager.Inst.PlayerActed = true;
            UIManager.Inst.SetTurnEndButton(true);
        }
    }

    private void Initialize()
    {
        for(int i=0; i<4; i++) 
        {
            for(int j=0; j<4; j++)
            {
                Vector2 pos = new Vector2(i, j);
                GameManager.Inst.boardState[i,j] = Instantiate(squarePrefab, pos, Quaternion.identity, transform).GetComponent<Checker>();
                
                GameManager.Inst.boardState[i,j].name = i + ", " + j;
                GameManager.Inst.boardState[i,j].coord = new Coordinate(i,j);
                GameManager.Inst.boardState[i,j].curCheckerPlayer = PlayerEnum.EMPTY;
                GameManager.Inst.boardState[i,j].curPiece = null;

                if((i+j) % 2 == 0)
                {
                    GameManager.Inst.boardState[i,j].GetComponent<SpriteRenderer>().color = new Color(60/255f,60/255f,60/255f);
                }   
            }
        }

        transform.position = new Vector3(-1.5f, -1.5f, 0);
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

                if(GameManager.Inst.boardState[i,j].curCheckerPlayer == PlayerEnum.EMPTY) continue;
                
                int temp = 1;
                while(i+_dx*temp > -1 && i+_dx*temp < 4 && j+_dy*temp > -1 && j+_dy*temp <4 && GameManager.Inst.boardState[i+_dx*temp, j+_dy*temp].curPiece == null)
                {
                    temp++;
                }
                GameManager.Inst.boardState[i,j].MovePiece(GameManager.Inst.boardState[i+_dx*(temp-1), j+_dy*(temp-1)]);
                
                int changedX = i+_dx*(temp-1);
                int changedY = j+_dy*(temp-1);

                if(changedX+_dx < 0 || changedX+_dx > 3 || changedY+_dy < 0 || changedY+_dy > 3) continue;
                if(GameManager.Inst.boardState[changedX+_dx, changedY+_dy].curPiece.pieceClass != GameManager.Inst.boardState[changedX, changedY].curPiece.pieceClass) continue;

                if(GameManager.Inst.boardState[changedX+_dx, changedY+_dy].curCheckerPlayer == GameManager.Inst.boardState[changedX, changedY].curCheckerPlayer)
                {
                    GameManager.Inst.boardState[changedX+_dx, changedY+_dy].curPiece = Merge(GameManager.Inst.boardState[changedX+_dx, changedY+_dy].curPiece, GameManager.Inst.boardState[changedX, changedY].curPiece);

                    GameManager.Inst.boardState[changedX, changedY].curPiece = null;
                    GameManager.Inst.boardState[changedX, changedY].curCheckerPlayer = PlayerEnum.EMPTY;

                }
                else
                {
                    //shoud destroy lower one
                    GameManager.Inst.boardState[changedX+_dx, changedY+_dy].RemovePiece();
                    GameManager.Inst.boardState[changedX, changedY].MovePiece(GameManager.Inst.boardState[changedX+_dx, changedY+_dy]);
                }
            }
        }
    }

    /// <summary>
    /// Merge p1 and p2 into next step of piece at p1's position
    /// </summary>
    /// <param name="p1">Merged piece</param>
    /// <param name="p2">Merging piece</param>
    public Piece Merge(Piece p1, Piece p2)
    {
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(p1.pieceClass + 1);
        Piece res = null;
        if(go != null)
        {
            res = Instantiate(go, p1.transform.position, Quaternion.identity, GameManager.Inst.Pieces).GetComponent<Piece>();
            res.curCoord = p1.curCoord;
            res.Initialize(p1.player);
        }
        Destroy(p1.gameObject);
        Destroy(p2.gameObject);

        return res;
    }

    public void PaintReachable(List<Coordinate> coordList)
    {
        if(coordList == null) return;
        foreach(var item in coordList)
        {
            GameManager.Inst.boardState[item.X, item.Y].Paint(Color.green);
        }
    }

    public void ResetPainted()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                GameManager.Inst.boardState[i,j].Paint( (i+j)%2==0 ? new Color(60/255f,60/255f,60/255f) : new Color(200/255f,200/255f,200/255f));
            }
        }
    }
}