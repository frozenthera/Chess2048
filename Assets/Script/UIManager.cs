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

    [SerializeField] Button TurnEndButton;
    [SerializeField] TextMeshProUGUI TurnPlayerInfoText;
    private void Start()
    {
        TurnPlayerInfoText.text = "Current Player\n: " + GameManager.Inst.player.ToString();
        TurnEndButton.onClick.AddListener(()=>{
            GameManager.Inst.SwapTurn();
            TurnPlayerInfoText.text = "Current Player\n: " + GameManager.Inst.player.ToString();
        });
    }

}
