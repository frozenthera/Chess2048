using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public abstract class Piece : NetworkBehaviour, INetworkSerializable
{
    public NetworkVariable<PieceEnum> pieceClass;
    public PlayerEnum player;
    public int curCoordX;
    public int curCoordY;
    [SerializeField] Sprite[] spriteList = new Sprite[2];

    protected List<Coordinate> diff;
    protected int range;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue<PlayerEnum>(ref player);
        serializer.SerializeValue(ref curCoordX);
        serializer.SerializeValue(ref curCoordY);
    }

    public virtual List<Coordinate> ReachableCoordinate()
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

    public void Initialize(PlayerEnum _player)
    {
        player = _player;
        GetComponent<SpriteRenderer>().sprite = spriteList[(int)player];
        _Initianlize();
    }

    protected virtual void _Initianlize(){}

    private void OnMouseDown()
    {
        if(GameManager.Inst.curPlayer.Value != player) return;
        if(GameManager.Inst.PlayerActed) return;
        if(GameManager.Inst.TurnPhase > 2) return;

        if(GameManager.Inst.isHighlighted)
        {
            GameManager.Inst.curMovable = null;
            Board.Inst.ResetPainted();
        } 

        if(GameManager.Inst.curSelected == this) 
        {
            GameManager.Inst.curSelected = null;
            return;
        }

        GameManager.Inst.curSelected = this;
        GameManager.Inst.curMovable = ReachableCoordinate();
        if(GameManager.Inst.curMovable.Count == 0)
        {
            GameManager.Inst.curSelected = null;
            return;
        }
        Board.Inst.PaintReachable(ReachableCoordinate());
    }
}