using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class HomepageManager : MonoBehaviour
{
    [SerializeField]
    private Button startHostButton;

    [SerializeField]
    private Button startClientButton;

    private void Awake()
    {
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        startHostButton.onClick.AddListener(() =>
        {
            if (NetworkManager.Singleton.StartHost())
            {
                Debug.Log("Host Started...");
                SceneManager.LoadScene("StartGame");
            }
        });
        startClientButton.onClick.AddListener(() =>
        {
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
