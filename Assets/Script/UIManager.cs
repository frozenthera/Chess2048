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
    [SerializeField] TextMeshProUGUI turnPlayerInfoText;
    [SerializeField] List<Sprite> whiteSpriteList = new();
    [SerializeField] List<Sprite> blackSpriteList = new();

    [SerializeField] Image whiteNextSprite;
    [SerializeField] Image blackNextSprite;
    private void Start()
    {
        turnPlayerInfoText.text = "Current Player\n: " + GameManager.Inst.player.ToString();
        turnEndButton.onClick.AddListener(()=>{
            GameManager.Inst.SwapTurn();
            turnPlayerInfoText.text = "Current Player\n: " + GameManager.Inst.player.ToString();
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
        whiteNextSprite.sprite = whiteSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.WHITE_Idx]];
        blackNextSprite.sprite = blackSpriteList[(int)GameManager.Inst.spawnList[GameManager.Inst.BLACK_Idx]];
    }
}
