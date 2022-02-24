using System.Collections;
using UnityEngine;

public class Card
{
    private int number;
    private int color;

    Card(int number,int color)
    {
        this.number = number;
        this.color = color;
    }

    public int Number { get { return number; } }
    public int Color { get { return color; } }

    public static Card GetStarterCard()
    {
        int cardNumber = Random.Range(0, 9);
        int cardColor = Random.Range(0, 4);
        return new Card(cardNumber, cardColor);
    }

    public static Card GetRandomCard()
    {
        int cardNumber = 1;
        int cardColor = Random.Range(0, 5);
        if (cardColor == 4)
        {
            cardNumber = Random.Range(11, 14);
        }
        else
        {
            cardNumber = Random.Range(0, 13);
        }
        return new Card(cardNumber, cardColor);
    }

    public override string ToString()
    {
        return "Card Number = " + number + " Card Color = " + color;
    }
}