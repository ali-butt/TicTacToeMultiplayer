using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] Transform CrossPrefab;
    [SerializeField] Transform CirclePrefab;

    public static GameVisualManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ManageGridVisuals(GameManager.PlayerType playerType, Vector3 position)
    {
        SpawnObjectRpc(playerType, position);
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(GameManager.PlayerType playerType, Vector2 pos)
    {
        print(GameManager.instance.CurrentPlayerType.Value + " " + playerType);
        if (GameManager.instance.CurrentPlayerType.Value != playerType)
            return;

        Transform obj = Instantiate(playerType == GameManager.PlayerType.Cross ? CrossPrefab : CirclePrefab, pos, quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }
}