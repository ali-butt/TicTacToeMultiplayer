using UnityEngine;
using System;

public class GridPosition : MonoBehaviour
{
    [SerializeField] Vector2Int Position;

    //    public static event Action<GameManager.PlayerType, GridPosition, Vector2Int> GridClicked;


    private void OnMouseDown()
    {
        //GridClicked?.Invoke(GameManager.instance.playerType, this, Position);
        GameManager.instance.ManageGridClick(GameManager.instance.playerType, this, Position);
    }
}