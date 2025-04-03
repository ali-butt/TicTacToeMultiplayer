using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] Button HostBtn;
    [SerializeField] Button clientBtn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HostBtn.onClick.AddListener(
            () =>
            {
                NetworkManager.Singleton.StartHost();

                gameObject.SetActive(false);
            }
        );

        clientBtn.onClick.AddListener(
            () =>
            {
                NetworkManager.Singleton.StartClient();

                gameObject.SetActive(false);
            }
        );
    }
}