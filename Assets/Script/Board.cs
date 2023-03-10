using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    DOWN,
    RIGHT,
    UP,
    LEFT
}

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    private Checker[,] boardState;

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            Debug.Log("Slide Down!");
            Slide(Direction.DOWN);
        }
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Slide Right!");
            Slide(Direction.RIGHT);
        }
        if(Input.GetKeyDown(KeyCode.UpArrow))
        {
            Debug.Log("Slide Up!");
            Slide(Direction.UP);
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Slide Left!");
            Slide(Direction.LEFT);
        }
    }

    private void Initialize()
    {
        boardState = new Checker[4,4];

        for(int i=0; i<4; i++) 
        {
            for(int j=0; j<4; j++)
            {
                Vector2 pos = new Vector2(i, j);
                boardState[i,j] = Instantiate(squarePrefab, pos, Quaternion.identity, transform).GetComponent<Checker>();
                
                boardState[i,j].name = i + ", " + j;
                boardState[i,j].coord = new Coordinate(i,j);
                boardState[i,j].curCheckerPlayer = PlayerEnum.EMPTY;
                boardState[i,j].curPiece = null;

                if((i+j) % 2 == 0)
                {
                    boardState[i,j].GetComponent<SpriteRenderer>().color = Color.black;
                }   
            }
        }

        transform.position = new Vector3(-1.5f, -1.5f, 0);
    }


    int[] dx = new int[]{0, 1, 0, -1};
    int[] dy = new int[]{-1, 0, 1, 0};
    public void Slide(Direction dir)
    {
        Debug.Log("!");
        int _dx = dx[(int)dir];
        int _dy = dy[(int)dir];

        bool isHappened = false;

        for(int i= _dx < 0 ? 0 : 3; _dx < 0 ? i<4 : i>-1 ; i -= _dx != 0 ? _dx : 1)
        {
            for(int j= _dy < 0 ? 0 : 3; _dy < 0 ? j<4 : j>-1 ; j -= _dy != 0 ? _dy : 1)
            {
                if(i+_dx < 0 || i+_dx > 3 || j+_dy < 0 || j+_dy > 3) continue;

                if(boardState[i,j].curCheckerPlayer == PlayerEnum.EMPTY) continue;
                
                if(boardState[i+_dx, j+_dy].curPiece == null)
                {
                    boardState[i,j].MovePiece(boardState[i+_dx, j+_dy]);
                    isHappened = true;
                    continue;
                }
                
                if(boardState[i+_dx, j+_dy].curPiece.pieceClass != boardState[i,j].curPiece.pieceClass) continue;

                if(boardState[i+_dx, j+_dy].curCheckerPlayer == boardState[i,j].curCheckerPlayer)
                {
                    boardState[i+_dx, j+_dy].curPiece = Merge(boardState[i+_dx, j+_dy].curPiece, boardState[i,j].curPiece);

                    boardState[i,j].curPiece = null;
                    boardState[i,j].curCheckerPlayer = PlayerEnum.EMPTY;

                    isHappened = true;
                }
                else
                {
                    //shoud destroy lower one
                    
                }
                
            }
        }

        if(isHappened) Slide(dir);
    }

    /// <summary>
    /// Merge p1 and p2 into next step of piece at p1's position
    /// </summary>
    /// <param name="p1">Merged piece</param>
    /// <param name="p2">Merging piece</param>
    public Piece Merge(Piece p1, Piece p2)
    {
        //Ignore king merge(which cause king turns into pawn)
        Debug.Log(p1.pieceClass + 1);
        GameObject go = GameManager.Inst.GetObjectByPieceEnum(p1.pieceClass + 1);
        Piece res = Instantiate(go, p1.transform.position, Quaternion.identity).GetComponent<Piece>();
        res.Initialize(p1.player);
        Destroy(p1.gameObject);
        Destroy(p2.gameObject);

        return res;
    }
}
