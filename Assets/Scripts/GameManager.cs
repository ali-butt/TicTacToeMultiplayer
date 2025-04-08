using UnityEngine;
using Unity.Netcode;
using System;
using System.Linq;
using UnityEngine.UIElements;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance { get; private set; }

    public enum PlayerType { none, Cross, Circle }
    public PlayerType playerType;
    public NetworkVariable<PlayerType> CurrentPlayerType = new NetworkVariable<PlayerType>();

    PlayerType[,] playerTypesArray = new PlayerType[3, 3];

    public event EventHandler OnGameStarted;
    public event EventHandler OnTurnChanged;

    public event EventHandler OnRematch;

    public event EventHandler<GameWiningArgs> OnGameWin;

    public class GameWiningArgs : EventArgs
    {
        public Transform trans;
    }


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
        ManageGridClickRpc(type, gridPosition.transform.position.x, gridPosition.transform.position.y, position);
    }

    [Rpc(SendTo.Server)]
    void ManageGridClickRpc(PlayerType type, float x, float y, Vector2Int position)
    {
        if (GameManager.instance.CurrentPlayerType.Value != type)
            return;

        if (playerTypesArray[position.x, position.y] != PlayerType.none)
            return;
        else
            playerTypesArray[position.x, position.y] = type;

        GameVisualManager.Instance.ManageGridVisuals(type, new Vector3(x, y, 0));

        Transform trans = IsThereAWinner();
        if (trans.position != Vector3.zero)
        {
            WinTriggerRpc(trans.position, trans.rotation.eulerAngles);

            CurrentPlayerType.Value = PlayerType.none;
        }
        else
        {
            Destroy(trans.gameObject);

            CurrentPlayerType.Value = (PlayerType)Mathf.Clamp(((int)CurrentPlayerType.Value + 1) % Enum.GetValues(typeof(PlayerType)).Length, 1, 2);
        }

    }

    [Rpc(SendTo.ClientsAndHost)]
    void WinTriggerRpc(Vector3 pos, Vector3 rot)
    {
        GameObject temp = new GameObject("lolo");
        temp.transform.position = pos;
        temp.transform.rotation = Quaternion.Euler(rot);

        OnGameWin?.Invoke(this, new GameWiningArgs { trans = temp.transform });

        Destroy(temp);
    }

    Transform IsThereAWinner()
    {
        GameObject temp = new GameObject("tempppppppppppppppppppp");
        temp.transform.localScale = new Vector3(6, 0, 0);

        if (playerTypesArray[0, 0] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(1)).Select(x => playerTypesArray[0, x]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(0, -4.7f, 0);
            temp.transform.rotation = Quaternion.identity;
        }
        else if (playerTypesArray[1, 0] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(1)).Select(x => playerTypesArray[1, x]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(0, -1.6f, 0);
            temp.transform.rotation = Quaternion.identity;
        }
        else if (playerTypesArray[2, 0] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(1)).Select(x => playerTypesArray[2, x]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(0, 1.5f, 0);
            temp.transform.rotation = Quaternion.identity;
        }
        else if (playerTypesArray[0, 0] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(0)).Select(x => playerTypesArray[x, 0]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(-3.1f, -1.6f, 0);
            temp.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (playerTypesArray[0, 1] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(0)).Select(x => playerTypesArray[x, 1]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(0, -1.6f, 0);
            temp.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (playerTypesArray[0, 2] != PlayerType.none && Enumerable.Range(0, playerTypesArray.GetLength(0)).Select(x => playerTypesArray[x, 2]).ToArray().Distinct().Count() == 1)
        {
            temp.transform.position = new Vector3(3.1f, -1.6f, 0);
            temp.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if ((playerTypesArray[0, 0] != PlayerType.none) && (playerTypesArray[0, 0] == playerTypesArray[1, 1]) && (playerTypesArray[1, 1] == playerTypesArray[2, 2]))
        {
            temp.transform.position = new Vector3(0, -1.6f, 0);
            temp.transform.rotation = Quaternion.Euler(0, 0, 45);
        }
        else if ((playerTypesArray[2, 0] != PlayerType.none) && (playerTypesArray[2, 0] == playerTypesArray[1, 1]) && (playerTypesArray[1, 1] == playerTypesArray[0, 2]))
        {
            temp.transform.position = new Vector3(0, -1.6f, 0);
            temp.transform.rotation = Quaternion.Euler(0, 0, -45);
        }
        else
        {
            temp.transform.position = new Vector3(0, 0, 0);
        }

        return temp.transform;
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

    [Rpc(SendTo.ClientsAndHost)]
    public void RematchRpc()
    {
        for (int i = 0; i < playerTypesArray.GetLength(0); i++)
        {
            for (int j = 0; j < playerTypesArray.GetLength(1); j++)
                playerTypesArray[i, j] = PlayerType.none;
        }

        if (IsServer)
            CurrentPlayerType.Value = (PlayerType)UnityEngine.Random.Range(1, 3);

        OnRematch?.Invoke(this, EventArgs.Empty);
    }





    #region  Network Methods

    public override void OnNetworkSpawn()
    {
        playerType = NetworkManager.Singleton.LocalClientId == 0 ? PlayerType.Cross : PlayerType.Circle;

        if (NetworkManager.Singleton.IsServer)
            CurrentPlayerType.Value = PlayerType.Cross;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCB;
        }

        CurrentPlayerType.OnValueChanged += (PlayerType oldType, PlayerType newtype) => { OnTurnChanged?.Invoke(this, EventArgs.Empty); };
    }

    #endregion
}