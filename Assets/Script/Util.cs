using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Util 
{

}

public enum Direction
{
    DOWN,
    RIGHT,
    UP,
    LEFT
}

[Serializable]
public enum PlayerEnum
{
    WHITE,
    BLACK,
    EMPTY
}

[Serializable]
public enum PieceEnum
{
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING,
    NONE
}