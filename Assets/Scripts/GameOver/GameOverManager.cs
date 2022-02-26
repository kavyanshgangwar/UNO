using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Kavyansh.Core.Singletons;
using TMPro;

public class GameOverManager : Singleton<GameOverManager>
{
    [SerializeField]
    private Button playAgain;

    [SerializeField]
    private GameObject gameOverText;
    // Start is called before the first frame update
    void Start()
    {
        gameOverText.GetComponent<TextMeshProUGUI>().text = GameManager.Instance.playerNames[GameManager.Instance.previousTurn.Value] + " Wins!";
        playAgain.onClick.AddListener(() => { GameManager.Instance.PlayAgainServerRpc(); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
