using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    protected override void _Initianlize()
    {
        diff = new List<Coordinate>()
        {
            new Coordinate(1,1),
            new Coordinate(1,-1),
            new Coordinate(-1,-1),
            new Coordinate(-1,1)
        };

        range = 1;
    }

    public override List<Coordinate> ReachableCoordinate()
    {
        List<Coordinate> res = new();
        
        for(int i=0; i<diff.Count; i++)
        {
            for(int j=1; j<range+1; j++)
            {
                Coordinate temp = new Coordinate(curCoordX, curCoordY) + diff[i]*j;
                if(temp.X < 0 || temp.X > 3 || temp.Y < 0 || temp.Y > 3) continue;

                if(GameManager.Inst.isTherePieceWithOppo(temp, player))
                {
                    res.Add(temp);
                    break;
                }
            }
        }

        return res;
    }
}
