using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
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
    }

    public void SetTurnEndButton(bool avail)
    {
        Image img = turnEndButton.GetComponent<Image>();
        if(avail)
        {
            img.color = Color.green;
        }
        else
        {
            img.color = Color.white;
        }
    }

    public void UpdateNextPiece()
    {
        if(GameManager.Inst.WHITE_Idx > 15)
        {
            whiteNextSprite.sprite = null;
        }
        else
        {
            whiteNextSprite.sprite = whiteSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx]];
        }

        if(GameManager.Inst.BLACK_Idx > 15)
        {
            blackNextSprite.sprite = null;
        }
        else
        {
            blackNextSprite.sprite = blackSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx]];
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
        SetTurnEndButton(false);
        UpdateNextPiece();
    }
}
