using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerEnum
{
    WHITE,
    BLACK,
    EMPTY
}

public abstract class Piece : MonoBehaviour
{
    public PieceEnum pieceClass;
    public PlayerEnum player;
    [SerializeField] Sprite[] spriteList = new Sprite[2];

    public abstract Coordinate[] ReachableCoordinate();

    public void Initialize(PlayerEnum _player)
    {
        player = _player;
        GetComponent<SpriteRenderer>().sprite = spriteList[(int)player];
    }

}
