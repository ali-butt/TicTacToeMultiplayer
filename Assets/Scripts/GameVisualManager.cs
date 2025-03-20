using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] Transform CrossPrefab;
    [SerializeField] Transform CirclePrefab;

    void OnEnable()
    {
        GridPosition.GridClicked += ManageGridVisuals;
    }

    void ManageGridVisuals(GameManager.PlayerType playerType, GridPosition gridPosition, Vector2Int position)
    {
        SpawnObjectRpc(playerType, gridPosition.transform.position);
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(GameManager.PlayerType playerType, Vector2 pos)
    {
        Transform obj = Instantiate(playerType == GameManager.PlayerType.Cross ? CrossPrefab : CirclePrefab, pos, quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }

    void OnDisable()
    {
        GridPosition.GridClicked -= ManageGridVisuals;
    }
}