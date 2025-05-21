using System;
using System.Collections;
using System.Collections.Generic;
using PistiGame;
using UnityEngine;

public class Player : User
{
    public override void OnTurnStart()
    {
        foreach (var card in Cards)
        {
            card.ToggleInteractable(true);
        }
    }

    public void OnTurnEnd()
    {
        foreach (var card in Cards)
        {
            card.ToggleInteractable(false);
        }
    }
}
