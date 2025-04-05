using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] Transform CrossPrefab;
    [SerializeField] Transform CirclePrefab;
    [SerializeField] Transform GreenLinePrefab;

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

    void Start()
    {
        GameManager.instance.OnGameWin += SpawnGreenLine;
    }

    void SpawnGreenLine(object sender, GameManager.GameWiningArgs e)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Instantiate(GreenLinePrefab, e.trans.position, e.trans.rotation).GetComponent<NetworkObject>().Spawn(true);

            Destroy(e.trans.gameObject);
        }
    }

    public void ManageGridVisuals(GameManager.PlayerType playerType, Vector3 position)
    {
        SpawnObjectRpc(playerType, position);
    }

    [Rpc(SendTo.Server)]
    void SpawnObjectRpc(GameManager.PlayerType playerType, Vector2 pos)
    {
        if (GameManager.instance.CurrentPlayerType.Value != playerType)
            return;

        Transform obj = Instantiate(playerType == GameManager.PlayerType.Cross ? CrossPrefab : CirclePrefab, pos, quaternion.identity);
        obj.GetComponent<NetworkObject>().Spawn(true);
    }
}