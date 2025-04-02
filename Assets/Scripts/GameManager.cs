using UnityEngine;
using Unity.Netcode;
using System;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }

    public enum PlayerType { none, Cross, Circle }
    public PlayerType playerType;
    public NetworkVariable<PlayerType> CurrentPlayerType = new NetworkVariable<PlayerType>();

    public event EventHandler OnGameStarted;
    public event EventHandler OnTurnChanged;

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

    public void ManageGridClick(PlayerType type, GridPosition gridPosition, Vector2Int position)
    {
        ManageGridClickRpc(type, gridPosition.transform.position.x, gridPosition.transform.position.y);
    }

    [Rpc(SendTo.Server)]
    void ManageGridClickRpc(PlayerType type, float x, float y)
    {
        if (GameManager.instance.CurrentPlayerType.Value != type)
            return;

        GameVisualManager.Instance.ManageGridVisuals(type, new Vector3(x, y, 0));

        CurrentPlayerType.Value = (PlayerType)Mathf.Clamp(((int)CurrentPlayerType.Value + 1) % Enum.GetValues(typeof(PlayerType)).Length, 1, 2);

    }



    void OnClientConnectedCB(ulong obj)
    {
        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            GameStartedRpc();
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void GameStartedRpc()
    {
        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }



    #region  Network Methods

    public override void OnNetworkSpawn()
    {
        playerType = NetworkManager.Singleton.LocalClientId == 0 ? PlayerType.Cross : PlayerType.Circle;

        CurrentPlayerType.Value = IsServer ? PlayerType.Cross : PlayerType.Circle;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCB;
        }

        CurrentPlayerType.OnValueChanged += (PlayerType oldType, PlayerType newtype) => {OnTurnChanged?.Invoke(this, EventArgs.Empty);};
    }

    #endregion
}