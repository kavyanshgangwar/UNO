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
    private void Awake()
    {
        DontDestroyOnLoad(this);
        
    }

    
        // Start is called before the first frame update
    void Start()
    {
        
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
}
