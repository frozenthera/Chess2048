using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    public PieceEnum pieceClass;

    public abstract Coordinate[] ReachableCoordinate();



}
