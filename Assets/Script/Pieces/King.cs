using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{

    protected override void Initianlize()
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

        range = 1;
    }

    public override void OnRemoved(PlayerEnum playerEnum)
    {
        if(GameManager.Inst.isGameOver) return;
        GameManager.Inst.GameOver(playerEnum == PlayerEnum.WHITE ? PlayerEnum.BLACK : PlayerEnum.WHITE); 
    }
}
