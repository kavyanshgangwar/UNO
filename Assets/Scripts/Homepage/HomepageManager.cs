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
    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startHostButton.onClick.AddListener(async () =>
        {
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
        startClientButton.onClick.AddListener(async () =>
        {
            if(RelayManager.Instance.IsRelayEnabled)
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
