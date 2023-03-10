using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject squarePrefab;
    private Checker[,] boardState;

    private void Start()
    {
        Initialize();
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
                
                boardState[i,j].coord = new Coordinate(i,j);
                boardState[i,j].curCheckerPlayer = CheckerStateEnum.EMPTY;
                boardState[i,j].curPiece = null;

                if((i+j) % 2 == 0)
                {
                    boardState[i,j].GetComponent<SpriteRenderer>().color = Color.black;
                }   
            }
        }

        transform.position = new Vector3(-1.5f, -1.5f, 0);
    }
}
