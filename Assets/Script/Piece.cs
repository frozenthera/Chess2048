using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public struct Piece : INetworkSerializable
{
    //should sync these variables
    public PieceEnum pieceClass;
    public PlayerEnum player;
    public Coordinate curCoord;

    //no need to synchronize
    public List<Coordinate> diff;
    public int range;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref pieceClass);
        serializer.SerializeValue(ref player);
        serializer.SerializeValue<Coordinate>(ref curCoord);
    }

    public Piece(PieceEnum _pieceEnum, PlayerEnum _player, Coordinate _curCoord)
    {
        pieceClass = _pieceEnum;
        player = _player;
        curCoord = _curCoord;
        diff = SetDiff(_pieceEnum);
        range = SetRange(_pieceEnum);
    }

    public List<Coordinate> ReachableCoordinate(Coordinate curCoord, PlayerEnum player)
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

    public void OnRemoved(PlayerEnum playerEnum){}

    private static List<Coordinate> SetDiff(PieceEnum piece)
    {   
        switch(piece)
        {
            case PieceEnum.PAWN:
                return new List<Coordinate>()
                {
                    new Coordinate(1,1),
                    new Coordinate(1,-1),
                    new Coordinate(-1,-1),
                    new Coordinate(-1,1)
                };

            case PieceEnum.ROOK:
                return new List<Coordinate>()
                {
                    new Coordinate(0,1),
                    new Coordinate(1,0),
                    new Coordinate(0,-1),
                    new Coordinate(-1,0)
                };

            case PieceEnum.KNIGHT:
                return new List<Coordinate>()
                {
                    new Coordinate(1,2),
                    new Coordinate(2,1),
                    new Coordinate(2,-1),
                    new Coordinate(1,-2),
                    new Coordinate(-1,-2),
                    new Coordinate(-2,-1),
                    new Coordinate(-2,1),
                    new Coordinate(-1,2)
                };

            case PieceEnum.BISHOP:
                return new List<Coordinate>()
                {
                    new Coordinate(1,1),
                    new Coordinate(1,-1),
                    new Coordinate(-1,-1),
                    new Coordinate(-1,1)
                };

            case PieceEnum.QUEEN:
                return new List<Coordinate>()
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

            case PieceEnum.KING:
                return new List<Coordinate>()
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

            default:
                return null;
        }
    }

    private static int SetRange(PieceEnum piece)
    {
        switch(piece)
        {
            case PieceEnum.PAWN:
            case PieceEnum.KNIGHT:
            case PieceEnum.KING:
                return 1;

            case PieceEnum.ROOK:
            case PieceEnum.BISHOP:
            case PieceEnum.QUEEN:
                return 3;

            default:
                return 0;
        }
    }
}