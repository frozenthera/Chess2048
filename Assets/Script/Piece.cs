using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public abstract class Piece
{
    public PieceEnum pieceClass;
    [SerializeField] Sprite[] spriteList = new Sprite[2];

    protected List<Coordinate> diff;
    protected int range;

    public virtual List<Coordinate> ReachableCoordinate(Coordinate curCoord, PlayerEnum player)
    {
        List<Coordinate> res = new();

        for(int i=0; i<diff.Count; i++)
        {
            for(int j=1; j<range+1; j++)
            {
                Coordinate temp = curCoord + diff[i]*j;
                if(temp.X < 0 || temp.X > 3 || temp.Y < 0 || temp.Y > 3) continue;

                if(GameManager.Inst.isTherePieceWithOppo(temp, player))
                {
                    res.Add(temp);
                    break;
                }
                else if(!GameManager.Inst.isTherePiece(temp))
                {
                    res.Add(temp);
                }
                else
                {
                    break;
                }
            }
        }

        return res;
    }

    protected virtual void Initianlize(){}

    public virtual void OnRemoved(PlayerEnum playerEnum){}
}