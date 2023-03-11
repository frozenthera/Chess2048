using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{

    protected override void _Initianlize()
    {
        diff = new List<Coordinate>()
        {
            new Coordinate(0,1),
            new Coordinate(1,0),
            new Coordinate(0,-1),
            new Coordinate(-1,0),
            new Coordinate(1,1),
            new Coordinate(1,-1),
            new Coordinate(-1,-1),
            new Coordinate(-1,1)
        };

        range = 3;
    }
}
