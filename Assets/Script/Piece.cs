using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Piece
{
    public static List<Coordinate> ReachableCoordinate(Coordinate curCoord, PlayerEnum player, PieceEnum piece, bool isAttack = false)
    {
        List<Coordinate> res = new();
        List<Coordinate> diff = GetDiff(piece, isAttack);
        for(int i=0; i< diff.Count; i++)
        {   
            for(int j=1; j< GetRange(piece)+1; j++)
            {
                Coordinate dest = curCoord + diff[i]*j;
                if(dest.X < 0 || dest.X > 3 || dest.Y < 0 || dest.Y > 3) continue;

                if(piece == PieceEnum.PAWN)
                {
                    if(isAttack && GameManager.Inst.isTherePieceWithOppo(dest, player))
                    {    
                        res.Add(dest);
                        break;
                    }
                    else if(!isAttack && !GameManager.Inst.isTherePiece(dest))
                    {
                        res.Add(dest);
                        break;
                    }
                }
                else if(GameManager.Inst.isTherePieceWithOppo(dest, player))
                {
                    res.Add(dest);
                    break;
                }
                else if(!GameManager.Inst.isTherePiece(dest))
                {
                    res.Add(dest);
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

    private static List<Coordinate> GetDiff(PieceEnum piece, bool isAttack = false)
    {   
        switch(piece)
        {
            case PieceEnum.PAWN:
                if(!isAttack)
                {
                    return new List<Coordinate>()
                    {
                        new Coordinate(0,1),
                        new Coordinate(1,0),
                        new Coordinate(0,-1),
                        new Coordinate(-1,0)
                    };
                }
                else
                {
                    return new List<Coordinate>()
                    {
                        new Coordinate(1,1),
                        new Coordinate(1,-1),
                        new Coordinate(-1,-1),
                        new Coordinate(-1,1)
                    };
                }

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

    private static int GetRange(PieceEnum piece)
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