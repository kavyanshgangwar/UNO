using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private RawImage playerNameDisplayPrefab;

    [SerializeField]
    private Button startGameButton;

    private int playersTillNow = 0;
    // Start is called before the first frame update
    void Start()
    {
        startGameButton.onClick.AddListener(() => {
            if (NetworkManager.Singleton.IsHost)
            {
                Debug.Log("Starting the game...");
                GameManager.Instance.StartGame();
                //Destroy(startGameButton);
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
    }
}
