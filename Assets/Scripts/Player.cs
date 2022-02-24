using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kavyansh.Core.Singletons;

public class Player : Singleton<Player>
{
    public bool inGame = false;
    public List<Card> cards;

    [SerializeField]
    private GameObject[] cardBackgrounds;

    [SerializeField]
    private GameObject[] cardSymbols;

    [SerializeField]
    private GameObject lastPlayedCard;

    private List<GameObject> hand;
    
    // Start is called before the first frame update
    void Start()
    {
        cards = new List<Card>();
        hand = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameStarted.Value)
        {
            if (!inGame)
            {
                StartGameForPlayer();
                inGame = true;
            }
        }
    }

    private void StartGameForPlayer()
    {
        cards.Clear();
        for(int i = 0; i < 7; i++)
        {
            cards.Add(Card.GetRandomCard());
        }
        for(int i = 2; i < 9; i++)
        {
            Destroy(GameObject.Find("card back ("+i+")"));
        }
        DisplayCards();
    }

    private void DisplayCards()
    {
        DisplayLastPlayedCard();
        for(int i=0;i<cards.Count;i++)
        {
            Debug.Log(cards[i]);
            GameObject temp = GameObject.Instantiate(cardBackgrounds[cards[i].Color]);
            float cardXPos = -4.5f + (9f /(cards.Count-1f)) * i;
            temp.transform.position = new Vector3(cardXPos, -2f, -1f -(4f/(cards.Count-1))*i);
            GameObject symbol1 = GameObject.Instantiate(cardSymbols[cards[i].Number], temp.transform);
            symbol1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            symbol1.transform.localPosition = new Vector3(-0.7f, 1.2f, -0.05f);
            GameObject symbol2 = GameObject.Instantiate(cardSymbols[cards[i].Number], temp.transform);
            symbol2.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            symbol2.transform.localPosition = new Vector3(0.7f, -1.3f, -0.05f);
            GameObject symbol3 = GameObject.Instantiate(cardSymbols[cards[i].Number],temp.transform);
            symbol3.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            symbol3.transform.localPosition = new Vector3(0, 0, -0.05f);
            hand.Add(temp);
        }
    }

    private void DisplayLastPlayedCard()
    {
        Destroy(lastPlayedCard);
        lastPlayedCard = GameObject.Instantiate(cardBackgrounds[GameManager.Instance.lastPlayedCardColor.Value]);
        lastPlayedCard.transform.position = new Vector3(0.6f, 2, -0.2f);
        GameObject symbol1 = GameObject.Instantiate(cardSymbols[GameManager.Instance.lastPlayedCardNumber.Value],lastPlayedCard.transform);
        symbol1.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        symbol1.transform.localPosition = new Vector3(-0.7f, 1.2f, -0.05f);
        GameObject symbol2 = GameObject.Instantiate(cardSymbols[GameManager.Instance.lastPlayedCardNumber.Value], lastPlayedCard.transform);
        symbol2.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        symbol2.transform.localPosition = new Vector3(0.7f, -1.3f, -0.05f);
        GameObject symbol3 = GameObject.Instantiate(cardSymbols[GameManager.Instance.lastPlayedCardNumber.Value], lastPlayedCard.transform);
        symbol3.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
        symbol3.transform.localPosition = new Vector3(0, 0, -0.05f);
    }
}
