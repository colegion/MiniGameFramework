using System.Collections;
using System.Collections.Generic;
using Helpers;
using PistiGame;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [SerializeField] private List<CardSlot> cardSlots;

    public CardSlot GetAvailableSlot()
    {
        return cardSlots.Find(c => c.IsAvailable);
    }

    public void EmptySlotByCard(Card card)
    {
        foreach (var slot in cardSlots)
        {
            if (slot.GetCardReference == card)
            {
                slot.Reset();
            }
        }
    }
}
