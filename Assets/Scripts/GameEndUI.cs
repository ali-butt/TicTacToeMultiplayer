using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WinLoseText;
    [SerializeField] Color WinColor32;
    [SerializeField] Color LoseColor32;
    [SerializeField] Button Rematch;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);

        GameManager.instance.OnGameWin += EndUI;
        GameManager.instance.OnRematch += OnRematch;

        Rematch.onClick.AddListener(
            () =>
            {
                GameManager.instance.RematchRpc();
            }
        );
    }

    void EndUI(object sender, GameManager.GameWiningArgs e)
    {
        if (GameManager.instance.CurrentPlayerType.Value == GameManager.instance.playerType)
        {
            WinLoseText.text = "You Win";
            WinLoseText.color = WinColor32;
        }
        else
        {
            WinLoseText.text = "You Lose";
            WinLoseText.color = LoseColor32;
        }

        gameObject.SetActive(true);
    }

    void OnRematch(object sender, EventArgs args)
    {
        OnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void OnRematchRpc()
    {
        gameObject.SetActive(false);
    }
}