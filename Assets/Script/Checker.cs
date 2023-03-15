using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : MonoBehaviour
{
    private SpriteRenderer sp;
    public Coordinate coord;
    [SerializeField] private SpriteRenderer pieceRenderer;
    [SerializeField] private List<Sprite> whitePieceSprite;
    [SerializeField] private List<Sprite> blackPieceSprite;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    public void SetPiece(PieceEnum pieceEnum, PlayerEnum playerEnum)
    {
        GameManager.Inst.SetPieceState(coord, pieceEnum);
        GameManager.Inst.SetPlayerState(coord, playerEnum);

        PaintPiece();
    }

    public void RemovePiece()
    {
        GameManager.Inst.SetPieceState(coord, PieceEnum.NULL);
        GameManager.Inst.SetPlayerState(coord, PlayerEnum.EMPTY);

        PaintPiece();
    }

    public void MovePiece(Checker dest)
    {
        if(dest == this) return;

        dest.SetPiece(GameManager.Inst.GetPieceState(coord), GameManager.Inst.GetPlayerState(coord));
        RemovePiece();

        dest.PaintPiece();
        PaintPiece();
    }

    public void PaintBackground(Color color)
    {
        sp.color = color;
    }

    public void PaintPiece()
    {
        if(GameManager.Inst.GetPlayerState(coord) == PlayerEnum.EMPTY) pieceRenderer.sprite = null;
        
        if(GameManager.Inst.GetPlayerState(coord) == PlayerEnum.WHITE)
            pieceRenderer.sprite = whitePieceSprite[(int)GameManager.Inst.GetPieceState(coord)];

        else pieceRenderer.sprite = blackPieceSprite[(int)GameManager.Inst.GetPieceState(coord)]; 
    }
}