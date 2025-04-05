using UnityEngine;
using TMPro;

public class GameEndUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI WinLoseText;
    public Color WinColor32;
    public Color LoseColor32;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);

        GameManager.instance.OnGameWin += EndUI;
    }

    void EndUI(object sender, GameManager.GameWiningArgs e)
    {
        print(GameManager.instance.CurrentPlayerType.Value);
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
        print(GameManager.instance.CurrentPlayerType.Value);

        gameObject.SetActive(true);
    }
}