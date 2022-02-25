using Kavyansh.Core.Singletons;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class GameManager : NetworkSingleton<GameManager>
{
    public NetworkList<FixedString32Bytes> playerNames;


    public NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);
    public NetworkVariable<int> lastPlayedCardNumber = new NetworkVariable<int>(0);
    public NetworkVariable<int> lastPlayedCardColor = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentColor = new NetworkVariable<int>(0);
    public NetworkVariable<int> currentTurn = new NetworkVariable<int>(0);
    public NetworkVariable<int> turnIncrementor = new NetworkVariable<int>(1);
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
                    playerNames = new NetworkList<FixedString32Bytes>();
                }
                playerNames.Add("Player " + id);
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
            Debug.Log("gettting starter card");
            Card card = Card.GetStarterCard();
            lastPlayedCardColor.Value = card.Color;
            lastPlayedCardNumber.Value = card.Number;
            currentColor.Value = card.Color;
        }
    }

    [ServerRpc(RequireOwnership =false)]
    public void PlayCardServerRpc(int clientId,int cardColor,int cardNumber,int curColor)
    {
        if (!IsServer) return;
        if (currentTurn.Value == clientId)
        {
            currentColor.Value = curColor;
            lastPlayedCardColor.Value = cardColor;
            lastPlayedCardNumber.Value = cardNumber;
            
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
                // make the next player draw 2 cards
                ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { (ulong)currentTurn.Value } } };
                DrawCardClientRpc(2, clientRpcParams);
            }else if(cardNumber == 12)
            {
                currentTurn.Value += turnIncrementor.Value;
                currentTurn.Value = (currentTurn.Value + playerNames.Count) % playerNames.Count;
                // make the next player draw 4 cards
                ClientRpcParams clientRpcParams = new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new ulong[] { (ulong)currentTurn.Value } } };
                DrawCardClientRpc(4, clientRpcParams);
            }
            currentTurn.Value += turnIncrementor.Value;
            currentTurn.Value = (currentTurn.Value + playerNames.Count) % playerNames.Count;
            
        }
    }

    [ClientRpc]
    public void DrawCardClientRpc(int numberOfCards,ClientRpcParams clientRpcParams = default)
    {
        
        Debug.Log("this is the client " + NetworkManager.Singleton.LocalClientId);
        Player.Instance.DrawCard(numberOfCards);
    }

    void CardPlayed(int oldValue,int newValue)
    {
        Player.Instance.DisplayLastPlayedCard();
    }
}
