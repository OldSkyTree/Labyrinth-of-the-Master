using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellsMover : MonoBehaviour
{
    private int direction;
    private int number;
    private bool isLine;

    private CellsHolder cellsHolder;

    void Start()
    {
        cellsHolder = CellsHolder.GetCellsHolder();
        if (Mathf.RoundToInt(transform.rotation.eulerAngles.y) == 180 || Mathf.RoundToInt(transform.rotation.eulerAngles.y) == 0)
        {
            isLine = true;
            number = GameController.ZToI(transform.position.z);
            direction = -Mathf.RoundToInt(transform.position.x);
        }
        else if (Mathf.RoundToInt(transform.rotation.eulerAngles.y) == 90 || Mathf.RoundToInt(transform.rotation.eulerAngles.y) == 270)
        {
            isLine = false;
            number = GameController.XToJ(transform.position.x);
            direction = -Mathf.RoundToInt(transform.position.z);
        }
    }

    public void Move()
    {
        float x = transform.position.x;
        float z = transform.position.z; 
        if (isLine)
            cellsHolder.MoveLine(number, direction);
        else
            cellsHolder.MoveColumn(number, direction);
    }
}
