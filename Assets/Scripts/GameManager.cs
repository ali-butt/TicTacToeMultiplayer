using UnityEngine;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }

    public enum PlayerType { none, Cross, Circle }
    public PlayerType playerType;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnEnable()
    {
        GridPosition.GridClicked += ManageGridClick;
    }

    public void ManageGridClick(PlayerType type, GridPosition gridPosition, Vector2Int position)
    {
        print(position);
    }

    void OnDisable()
    {
        GridPosition.GridClicked -= ManageGridClick;
    }




    #region  Network Methods

    public override void OnNetworkSpawn()
    {
        playerType = NetworkManager.Singleton.LocalClientId == 0 ? PlayerType.Cross : PlayerType.Circle;
    }

    #endregion
}