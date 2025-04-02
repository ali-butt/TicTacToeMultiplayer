using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] GameObject CrossImage;
    [SerializeField] GameObject CircleImage;
    [SerializeField] GameObject CrossText;
    [SerializeField] GameObject CircleText;
    [SerializeField] GameObject CrossArrowImage;
    [SerializeField] GameObject CircleArrowImage;


    void OnEnable()
    {
        GameManager.instance.OnGameStarted += GameIsjustStarting;
        GameManager.instance.OnTurnChanged += TurnChanged;
    }

    void Start()
    {
        CrossText.SetActive(false);
        CircleText.SetActive(false);
        CrossArrowImage.SetActive(false);
        CircleArrowImage.SetActive(false);
    }

    void GameIsjustStarting(object sender, System.EventArgs e)
    {
        if (GameManager.instance.playerType == GameManager.PlayerType.Cross)
        {
            CrossImage.SetActive(true);
            CrossText.SetActive(true);
        }
        else
        {
            CircleImage.SetActive(true);
            CircleText.SetActive(true);
        }

        UpdateArrowImages();
    }

    void TurnChanged(object sender, System.EventArgs e)
    {
        UpdateArrowImages();
    }

    void UpdateArrowImages()
    {
        CrossArrowImage.SetActive(GameManager.instance.CurrentPlayerType.Value == GameManager.PlayerType.Cross);
        CircleArrowImage.SetActive(GameManager.instance.CurrentPlayerType.Value != GameManager.PlayerType.Cross);   
    }
}