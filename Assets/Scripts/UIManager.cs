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

    private Button red;
    private Button green;
    private Button blue;
    private Button yellow;

    private int playersTillNow = 0;
    // Start is called before the first frame update
    void Start()
    {
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
    }

    public void DisplayChooseColorButtons(Card card)
    {
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
            
            CardPlayed();
        });
        green = GameObject.Instantiate(greenButton, canvas.transform);
        green.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 1);
            CardPlayed();
        });
        blue = GameObject.Instantiate(blueButton, canvas.transform);
        blue.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 3);
            CardPlayed();
        });
        yellow = GameObject.Instantiate(yellowButton, canvas.transform);
        yellow.onClick.AddListener(() => {
            GameManager.Instance.PlayCardServerRpc((int)NetworkManager.Singleton.LocalClientId, card.Color, card.Number, 2);
            CardPlayed();
        });
    }

    public void CardPlayed()
    {
        red.gameObject.SetActive(false);
        green.gameObject.SetActive(false);
        blue.gameObject.SetActive(false);
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
}
