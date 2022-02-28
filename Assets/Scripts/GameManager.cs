using Kavyansh.Core.Singletons;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;

public class GameManager : NetworkSingleton<GameManager>
{
    public NetworkList<FixedString32Bytes> playerNames;

    public NetworkList<int> cardsCount;

    private NetworkList<bool> unoFlags;

    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);
    public NetworkVariable<int> lastPlayedCardNumber = new NetworkVariable<int>(0);
    public NetworkVariable<int> lastPlayedCardColor = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentColor = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);
    public NetworkVariable<int> turnIncrementor = new NetworkVariable<int>(1);
    public NetworkVariable<int> previousTurn = new NetworkVariable<int>(0);

    public int winner = 0;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }

    
        // Start is called before the first frame update
    void Start()
    {
        lastPlayedCardNumber.OnValueChanged += CardPlayed;
        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (IsServer)
            {
                if(playerNames == null)
                {
                    cardsCount = new NetworkList<int>();
                    playerNames = new NetworkList<FixedString32Bytes>();
                    unoFlags = new NetworkList<bool>();
                }
                playerNames.Add("Player " + id);
                cardsCount.Add(0);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        if (IsHost)
        {
            gameStarted.Value = true;
            turnIncrementor.Value = 1;
            Debug.Log("gettting starter card");
            Card card = Card.GetStarterCard();
            lastPlayedCardColor.Value = card.Color;
            lastPlayedCardNumber.Value = card.Number;
            currentColor.Value = card.Color;
            cardsCount.Clear();
            for(int i = 0; i < playerNames.Count; i++)
            {
                cardsCount.Add(7);
            }
            unoFlags.Clear();
            for(int i = 0;i < cardsCount.Count; i++)
            {
                unoFlags.Add(false);
            }
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void PlayAgainServerRpc()
    {
        PlayAgainClientRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    public void IncrementCardCountServerRpc(int x,int clientId)
    {
        if (!IsServer) { return; }
        cardsCount[clientId] += x;
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(int clientId)
    {
        if(clientId == currentTurn.Value)
        {
            previousTurn.Value = currentTurn.Value;
            currentTurn.Value += turnIncrementor.Value;
            currentTurn.Value = (currentTurn.Value + playerNames.Count)%playerNames.Count;
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void CallUNOServerRpc(int clientId)
    {
        if(currentTurn.Value == clientId)
        {
            if(cardsCount[clientId] <= 2)
            {
                unoFlags[clientId] = true;
                DisplayUNOClientRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void ClaimUNOServerRpc()
    {
        if (unoFlags[previousTurn.Value] == true) return;
        if(cardsCount[previousTurn.Value] <2)
        {
            cardsCount[previousTurn.Value] += 4;
            ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { (ulong)previousTurn.Value } } };
            DrawCardClientRpc(4, clientRpcParams);
            ClaimedUNOClientRpc();
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void PlayCardServerRpc(int clientId,int cardColor,int cardNumber,int curColor)
    {
        if (!IsServer) return;
        if (currentTurn.Value == clientId)
        {
            cardsCount[clientId]--;
            unoFlags[previousTurn.Value] = false;
            currentColor.Value = curColor;
            lastPlayedCardColor.Value = cardColor;
            lastPlayedCardNumber.Value = cardNumber;
            previousTurn.Value = currentTurn.Value;
            if(cardNumber == 9)
            {
                currentTurn.Value += turnIncrementor.Value;
            }else if(cardNumber == 10)
            {
                turnIncrementor.Value *= -1;
            }else if(cardNumber == 11)
            {
                currentTurn.Value+=turnIncrementor.Value;
                currentTurn.Value = (currentTurn.Value+playerNames.Count) % playerNames.Count;
                cardsCount[currentTurn.Value]+=2;
                // make the next player draw 2 cards
                ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { (ulong)currentTurn.Value } } };
                DrawCardClientRpc(2, clientRpcParams);
            }else if(cardNumber == 12)
            {
                currentTurn.Value += turnIncrementor.Value;
                currentTurn.Value = (currentTurn.Value + playerNames.Count) % playerNames.Count;
                cardsCount[currentTurn.Value] += 4;
                // make the next player draw 4 cards
                ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { (ulong)currentTurn.Value } } };
                DrawCardClientRpc(4, clientRpcParams);
            }
            currentTurn.Value += turnIncrementor.Value;
            currentTurn.Value = (currentTurn.Value + playerNames.Count) % playerNames.Count;
            IsGameOver();
        }
    }

    [ClientRpc]
    public void DrawCardClientRpc(int numberOfCards,ClientRpcParams clientRpcParams = default)
    {
        
        Debug.Log("this is the client " + NetworkManager.Singleton.LocalClientId);
        Player.Instance.DrawCard(numberOfCards);
    }

    [ClientRpc]
    public void DisplayUNOClientRpc()
    {
        UIManager.Instance.DisplayUNO();
    }

    [ClientRpc]
    public void ClaimedUNOClientRpc()
    {
        UIManager.Instance.ClaimedUNO();
    }

    [ClientRpc]
    public void GameOverClientRpc(int clientId)
    {
        winner = clientId;
        UIManager.Instance.GameOver(clientId);
    }

    [ClientRpc]
    public void PlayAgainClientRpc()
    {
        SceneManager.LoadScene("StartGame");
    }
    void CardPlayed(int oldValue,int newValue)
    {
        Player.Instance.DisplayLastPlayedCard();
    }

    void IsGameOver()
    {
        if(cardsCount[previousTurn.Value] == 0)
        {
            gameStarted.Value = false;
            GameOverClientRpc(previousTurn.Value);
        }
    }

    
}
