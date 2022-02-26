using System.Collections;
using System.Collections.Generic;
using Kavyansh.Core.Singletons;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RawImage playerNameDisplayPrefab;

    [SerializeField]
    private Button startGameButton;

    [SerializeField]
    private Button redButton;

    [SerializeField]
    private Button greenButton;

    [SerializeField]
    private Button blueButton;

    [SerializeField]
    private Button yellowButton;

    [SerializeField]
    private Button drawCard;

    [SerializeField]
    private Button endTurn;

    [SerializeField]
    private Button callUno;

    [SerializeField]
    private Button claimUno;

    [SerializeField]
    private GameObject cardDisplay;

    [SerializeField]
    private GameObject uno;

    private Color[] colorList;

    private Button red;
    private Button green;
    private Button blue;
    private Button yellow;

    public bool choosingColor=false;

    private int playersTillNow = 0;

    private List<GameObject> cardsCountDisplay;
    // Start is called before the first frame update
    void Start()
    {
        colorList = new Color[4];
        colorList[0] = new Color(0.961f, 0.165f, 0.020f, 1f);
        colorList[1] = new Color(0.047f, 0.655f, 0.220f,1f);
        colorList[2] = new Color(0.988f, 0.792f, 0.012f,1f);
        colorList[3] = new Color(0.086f, 0.369f, 0.733f,1f);
        cardsCountDisplay = new List<GameObject>();
        startGameButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Starting the game...");
                GameManager.Instance.StartGame();
                startGameButton.gameObject.SetActive(false);
                drawCard.gameObject.SetActive(true);
                callUno.gameObject.SetActive(true);
                claimUno.gameObject.SetActive(true);
            }
        });
        drawCard.onClick.AddListener(() => {
            drawCard.gameObject.SetActive(false);
            DrawCardButtonCall();

        });
        endTurn.onClick.AddListener(() => {
            endTurn.gameObject.SetActive(false);
            GameManager.Instance.EndTurnServerRpc((int)NetworkManager.Singleton.LocalClientId);
            drawCard.gameObject.SetActive(true);
        });
        callUno.onClick.AddListener(() => {
            if (Player.Instance.cards.Count <= 2)
            {
                GameManager.Instance.CallUNOServerRpc((int)NetworkManager.Singleton.LocalClientId);
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.playerNames != null &&  playersTillNow < GameManager.Instance.playerNames.Count)
        {
            
            for (int i = playersTillNow; i < GameManager.Instance.playerNames.Count; i++)
            {
                // Display Players on UI
                RawImage curName = GameObject.Instantiate(playerNameDisplayPrefab, canvas.transform);
                curName.transform.position = new Vector3(180, 1020 - 70 * i, -1);
                TextMeshProUGUI name = curName.GetComponentInChildren<TextMeshProUGUI>();
                name.text = GameManager.Instance.playerNames[i].ToString();
            }
            playersTillNow = GameManager.Instance.playerNames.Count;
        }
        UpdateCardsCount();
    }

    void UpdateCardsCount()
    {
        if (GameManager.Instance.cardsCount == null) return;
        if (cardsCountDisplay.Count < GameManager.Instance.cardsCount.Count)
        {
            for (int i = cardsCountDisplay.Count; i < GameManager.Instance.cardsCount.Count; i++)
            {
                GameObject a = GameObject.Instantiate(cardDisplay, canvas.transform);
                TextMeshProUGUI count = a.GetComponentInChildren<TextMeshProUGUI>();
                count.text = GameManager.Instance.cardsCount[i].ToString();
                a.transform.position = new Vector3(310, 1020 - 70 * i, -1);
                cardsCountDisplay.Add(a);
            }
        }
        for (int i = 0; i < cardsCountDisplay.Count; i++)
        {
            cardsCountDisplay[i].GetComponentInChildren<TextMeshProUGUI>().text = GameManager.Instance.cardsCount[i].ToString();
            cardsCountDisplay[i].GetComponent<Image>().color = Color.black;
        }
        cardsCountDisplay[GameManager.Instance.currentTurn.Value].GetComponent<Image>().color = colorList[GameManager.Instance.currentColor.Value];
        
    }

    public void DisplayChooseColorButtons(Card card)
    {
        choosingColor = true;
        drawCard.gameObject.SetActive(false);
        callUno.gameObject.SetActive(false);
        endTurn.gameObject.SetActive(false);
        claimUno.gameObject.SetActive(false);
        Destroy(red);
        Destroy(green);
        Destroy(blue);
        Destroy(yellow);
        red = GameObject.Instantiate(redButton, canvas.transform);
        red.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 0);
            choosingColor = false;
            CardPlayed();
        });
        green = GameObject.Instantiate(greenButton, canvas.transform);
        green.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 1);
            choosingColor = false;
            CardPlayed();
        });
        blue = GameObject.Instantiate(blueButton, canvas.transform);
        blue.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 3);
            choosingColor = false;
            CardPlayed();
        });
        yellow = GameObject.Instantiate(yellowButton, canvas.transform);
        yellow.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 2);
            choosingColor = false;
            CardPlayed();
        });
    }

    public void CardPlayed()
    {
        if(red!=null)
        red.gameObject.SetActive(false);
        if(green!=null)
        green.gameObject.SetActive(false);
        if(blue!=null)
        blue.gameObject.SetActive(false);
        if(yellow!=null)
        yellow.gameObject.SetActive(false);
        drawCard.gameObject.SetActive(true);
        callUno.gameObject.SetActive(true);
        claimUno.gameObject.SetActive(true);
        endTurn.gameObject.SetActive(false);
    }
    void DrawCardButtonCall()
    {
        if(GameManager.Instance.currentTurn.Value == (int)NetworkManager.Singleton.LocalClientId)
        {
            Card card = Card.GetRandomCard();
            Player.Instance.AddCard(card);
            GameManager.Instance.IncrementCardCountServerRpc(1,(int)NetworkManager.Singleton.LocalClientId);
            if (Player.Instance.IsValidCardToPlay(card))
            {
                endTurn.gameObject.SetActive(true);

            }
            else
            {
                GameManager.Instance.EndTurnServerRpc((int)NetworkManager.Singleton.LocalClientId);
                drawCard.gameObject.SetActive(true);
            }
            
        }
    }

    public void DisplayUNO()
    {
        Debug.Log("UNO!");
        Destroy(GameObject.Instantiate(uno, canvas.transform), 1f);
    }
}
