using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
public class UIManager : NetworkBehaviour
{
    public static UIManager Inst;

    private void Awake()
    {
        Inst = this;
    }

    [SerializeField] Button turnEndButton;
    [SerializeField] public TextMeshProUGUI turnPlayerInfoText;
    [SerializeField] List<Sprite> whiteSpriteList = new();
    [SerializeField] List<Sprite> blackSpriteList = new();

    [SerializeField] Image whiteNextSprite;
    [SerializeField] Image blackNextSprite;

    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] Button gameRestartButton;
    [SerializeField] RectTransform ResultPanel;

    [SerializeField] TextMeshProUGUI slideText;
    [SerializeField] TextMeshProUGUI moveText;
    [SerializeField] TextMeshProUGUI spawnText;

    private void Start()
    {
        UpdateTurnEndButton();
        turnEndButton.onClick.AddListener(()=>{
            GameManager.Inst.SwapTurn();
            UpdateTurnEndButton();
        });

        gameRestartButton.onClick.AddListener(()=>{
            GameManager.Inst.ResetGame();
            ResultPanel.gameObject.SetActive(false);
        });

        GameManager.Inst.PlayerActed.OnValueChanged += SetTurnEndButton;
    }

    public void SetTurnEndButton(bool previous, bool current)
    {
        Image img = turnEndButton.GetComponent<Image>();
        if(current)
        {
            img.color = Color.green;
        }
        else
        {
            img.color = Color.white;
        }
    }

    public void SetTurnPhaseIndicator()
    {
        int num = GameManager.Inst.TurnPhase;
        slideText.color = num < 1 ? Color.white : Color.gray; 
        moveText.color  = num < 2 ? Color.white : Color.gray;
        spawnText.color = num < 3 ? Color.white : Color.gray;
    }

    public void UpdateNextPiece()
    {
        if(GameManager.Inst.WHITE_Idx.Value > 15)
        {
            whiteNextSprite.sprite = null;
        }
        else
        {
            whiteNextSprite.sprite = whiteSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx.Value]];
        }

        if(GameManager.Inst.BLACK_Idx.Value > 15)
        {
            blackNextSprite.sprite = null;
        }
        else
        {
            blackNextSprite.sprite = blackSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx.Value]];
        }        
    }

    public void UpdateTurnEndButton()
    {
        turnPlayerInfoText.text = "Current Player\n" + GameManager.Inst.player.ToString();
    }

    public void SetResultPanel(PlayerEnum winner)
    {
        ResultPanel.gameObject.SetActive(true);
        winText.text = $"{winner.ToString()} WINS!!";
    }

    public void ResetUI()
    {
        UpdateTurnEndButton();
        GameManager.Inst.PlayerActed.Value = false;
        // SetTurnEndButton(true, false);
        UpdateNextPiece();
    }
}
