using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : MonoBehaviour
{
    private SpriteRenderer sp;
    public NetworkVariable<Coordinate> coord;
    public int x;
    public int y;

    [SerializeField] private SpriteRenderer pieceRenderer;
    [SerializeField] private List<Sprite> whitePieceSprite;
    [SerializeField] private List<Sprite> blackPieceSprite;

    private void Start()
    {
        sp = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        x = coord.Value.X;
        y = coord.Value.Y;
    }

    public void PaintBackground(Color color)
    {
        sp.color = color;
    }

    public void PaintPiece()
    {
        // Debug.Log(this.coord.Value.ToString() + ", " + GameManager.Inst.GetPieceState(this.coord.Value) + ", " + GameManager.Inst.GetPlayerState(this.coord.Value));
        if(GameManager.Inst.GetPlayerState(this.coord.Value) == PlayerEnum.EMPTY)
        {
            pieceRenderer.sprite = null;
        } 
        else if(GameManager.Inst.GetPlayerState(this.coord.Value) == PlayerEnum.WHITE)
        {
            pieceRenderer.sprite = whitePieceSprite[(int)GameManager.Inst.GetPieceState(this.coord.Value)];
        }
        else
        {
            pieceRenderer.sprite = blackPieceSprite[(int)GameManager.Inst.GetPieceState(this.coord.Value)]; 
        } 
    }
}