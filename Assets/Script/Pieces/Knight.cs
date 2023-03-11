using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    protected override void _Initianlize()
    {
        diff = new List<Coordinate>()
        {
            new Coordinate(1,2),
            new Coordinate(2,1),
            new Coordinate(2,-1),
            new Coordinate(1,-1),
            new Coordinate(-1,-2),
            new Coordinate(-1,-2),
            new Coordinate(-2,-1),
            new Coordinate(-1,2)
        };

        range = 1;
    }
}