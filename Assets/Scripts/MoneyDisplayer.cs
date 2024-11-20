using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyDisplayer : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image background;
    Nation playerNation;
    int playerTeam = 0;

    private void Start()
    {
        playerTeam = NationsManager.GetPlayerTeam();
        playerNation = NationsManager.GetNation(playerTeam);
        if (background != null )
            background.color = playerNation.GetTeamColor();
    }


    private void Update()
    {
        if (playerNation != null && text != null)
        {
            text.text = $"Money : {playerNation.GetMoney()},-";
        }
    }
}
