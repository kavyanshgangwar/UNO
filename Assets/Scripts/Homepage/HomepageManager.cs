using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class HomepageManager : MonoBehaviour
{
    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    [SerializeField]
    private TMP_InputField joinCodeInput;

    [SerializeField]
    private TMP_InputField userName;

    [SerializeField]
    private Button joinRoomButton;

    [SerializeField]
    private Button createRoomButton;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startHostButton.onClick.AddListener(() => {
            userName.gameObject.SetActive(true);
            createRoomButton.gameObject.SetActive(true);
            startHostButton.gameObject.SetActive(false);
            startClientButton.gameObject.SetActive(false);
        });
        createRoomButton.onClick.AddListener(async () =>
        {
            if (!string.IsNullOrEmpty(userName.text))
            {
                GameManager.Instance.localUserName = userName.text;
            }
            else
            {
                GameManager.Instance.localUserName = "Player";
            }
            if (RelayManager.Instance.IsRelayEnabled)
            {
                await RelayManager.Instance.SetupRelay();

            }
            if (NetworkManager.Singleton.StartHost())
            {


                Debug.Log("Host Started...");
                SceneManager.LoadScene("StartGame");
            }
        });
        startClientButton.onClick.AddListener(() => {
            userName.gameObject.SetActive(true);
            joinCodeInput.gameObject.SetActive(true);
            joinRoomButton.gameObject.SetActive(true);
            startHostButton.gameObject.SetActive(false);
            startClientButton.gameObject.SetActive(false);
        });
        joinRoomButton.onClick.AddListener(async () =>
        {
            if (!string.IsNullOrEmpty(userName.text))
            {
                GameManager.Instance.localUserName = userName.text;
            }
            else
            {
                GameManager.Instance.localUserName = "Player";
            }
            if (RelayManager.Instance.IsRelayEnabled)
            {
                if (!string.IsNullOrEmpty(joinCodeInput.text))
                {
                    await RelayManager.Instance.JoinRelay(joinCodeInput.text);
                }
            } 
            if (NetworkManager.Singleton.StartClient())
            {
                Debug.Log("Client Started...");
                SceneManager.LoadScene("StartGame");
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
