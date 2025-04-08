using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class GameVisualManager : NetworkBehaviour
{
    [SerializeField] Transform CrossPrefab;
    [SerializeField] Transform CirclePrefab;
    [SerializeField] Transform GreenLinePrefab;
    List<Transform> SpawnedObjs = new List<Transform>();

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
        GameManager.instance.OnRematch += Rematch;
    }

    void SpawnGreenLine(object sender, GameManager.GameWiningArgs e)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            Transform temp = Instantiate(GreenLinePrefab, e.trans.position, e.trans.rotation);
            temp.GetComponent<NetworkObject>().Spawn(true);

            SpawnedObjs.Add(temp);

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

        SpawnedObjs.Add(obj);
    }

    void Rematch(object sender, EventArgs args)
    {
        OnRematchRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    void OnRematchRpc()
    {
        for (int i = 0; i < SpawnedObjs.Count; i++)
        {
            Destroy(SpawnedObjs[i].gameObject);
        }

        SpawnedObjs.Clear();
    }
}