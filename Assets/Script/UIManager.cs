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

    // [SerializeField] Button turnEndButton;
    [SerializeField] List<Sprite> whiteSpriteList = new();
    [SerializeField] List<Sprite> blackSpriteList = new();

    [SerializeField] Image whiteNextSprite;
    [SerializeField] Image blackNextSprite;

    [SerializeField] public TextMeshProUGUI turnPlayerInfoText;
    [SerializeField] TextMeshProUGUI winText;
    [SerializeField] Button gameRestartButton;
    [SerializeField] RectTransform ResultPanel;

    [ClientRpc]
    public void UpdateNextPieceClientRpc()
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

    [ClientRpc]
    public void InitializeClientRpc(bool isServerBlack)
    {
        PlayerEnum player = isServerBlack ^ IsServer ? PlayerEnum.WHITE : PlayerEnum.BLACK;
        UpdateTurnEndButton(player, isServerBlack ^ IsServer);
    }

    public void UpdateTurnEndButton() => UpdateTurnEndButton(GameManager.Inst.IsMyTurn);
    public void UpdateTurnEndButton(bool isMyTurn)
    {
        turnPlayerInfoText.text = GameManager.Inst.LocalPlayerSide.ToString() + "\n" + (isMyTurn ? "My Turn!" : "Waiting for Opponent...");
    }
    public void UpdateTurnEndButton(PlayerEnum playerEnum, bool isMyTurn)
    {
        turnPlayerInfoText.text = playerEnum.ToString() + "\n" + (isMyTurn ? "My Turn!" : "Waiting for Opponent...");
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
        UpdateNextPieceClientRpc();
    }
}
