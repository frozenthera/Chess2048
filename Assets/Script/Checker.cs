using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class Checker : NetworkBehaviour
{
    private SpriteRenderer sp;
    public NetworkVariable<Coordinate> coord;
    public NetworkVariable<PlayerEnum> player;
    public NetworkVariable<PieceEnum> piece;
    public int x;
    public int y;

    private bool isChanged = false;

    [SerializeField] private SpriteRenderer pieceRenderer;
    [SerializeField] private List<Sprite> whitePieceSprite;
    [SerializeField] private List<Sprite> blackPieceSprite;

    public override void OnNetworkSpawn()
    {
        sp = GetComponent<SpriteRenderer>();
        Initialize();
    }

    public void LateUpdate()
    {
        if(IsServer) return;
        if(!isChanged) return;
        PaintPiece();
        isChanged = false;
    }

    public void Initialize()
    {        
        player.Value = PlayerEnum.EMPTY;
        piece.Value = PieceEnum.NONE;
        piece.OnValueChanged += PaintPiece;
        if(IsServer) return;
        PaintBackground((coord.Value.X + coord.Value.Y) % 2 == 0 ? new Color(60/255f,60/255f,60/255f) : new Color(200/255f,200/255f,200/255f));
        GameManager.Inst.boardState[coord.Value.X, coord.Value.Y] = this;
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
        // if(GameManager.Inst.GetPlayerState(this.coord.Value) == PlayerEnum.EMPTY)
        if(player.Value == PlayerEnum.EMPTY)
        {
            pieceRenderer.sprite = null;
        } 
        else if(player.Value == PlayerEnum.WHITE)
        {
            pieceRenderer.sprite = whitePieceSprite[(int)piece.Value];
        }
        else
        {
            pieceRenderer.sprite = blackPieceSprite[(int)piece.Value]; 
        } 
    }

    public void PaintPiece(PieceEnum past, PieceEnum cur)
    {
        // Debug.Log(coord.Value.ToString() + " : " + player.Value.ToString() + ", " + cur.ToString());        
        if(IsServer)
        {
            if(player.Value == PlayerEnum.EMPTY)
            {
                pieceRenderer.sprite = null;
            } 
            else if(player.Value == PlayerEnum.WHITE)
            {
                pieceRenderer.sprite = whitePieceSprite[(int)cur];
            }
            else
            {
                pieceRenderer.sprite = blackPieceSprite[(int)cur]; 
            } 
        }
        else
        {
            isChanged = true;    
        }
    }
}