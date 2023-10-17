using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardKind
{
    ROOM = 0,
    WEAPON,
    SUSPECT,
}

[System.Serializable]
public class Card
{
    public CardKind cardKind;
    public int cardNumber;
    public string cardName;
    public Sprite cardImage;
}

[CreateAssetMenu(menuName = "CLUE/CardKind")]
public class CardInfo : ScriptableObject
{
    public List<Card> cardLists;



    public Card FindCard(int index)
    {
        return cardLists?.Find(d => d.cardNumber == index);
    }
    public Card FindCard(string value)
    {
        return cardLists?.Find(d => d.cardName.Equals(value));
    }
}
