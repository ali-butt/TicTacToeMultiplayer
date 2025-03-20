using UnityEngine;

public class GridPosition : MonoBehaviour
{
    [SerializeField] Vector2Int Position;

    
    private void OnMouseDown()
    {
        GameManager.instance.ManageGridClick(Position);
    }
}