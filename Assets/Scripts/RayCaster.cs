using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    private Camera currentCamera;

    private GameController gameController;
    private CellsHolder cellsHolder;
    
    void Start()
    {
        currentCamera = GetComponent<Camera>();
        gameController = GameController.GetGameController();
        cellsHolder = CellsHolder.GetCellsHolder();
    }

    void Update()
    {
        Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100))
        {
            if (hit.collider.gameObject.tag == "Cell")
                cellsHolder.ShowPath(hit.collider.gameObject);
            if (Input.GetMouseButtonUp(0))
                HitHandler(hit);
        }
    }

    void HitHandler(RaycastHit hit)
    {
        GameObject rayReceiver = hit.collider.gameObject;
            
        switch (gameController.GetGameMode())
        {
            case GameController.GameMode.LineChoose:
                {
                    if (rayReceiver.tag == "Arrow")
                        rayReceiver.GetComponent<CellsMover>().Move();
                    break;
                }
            case GameController.GameMode.FigureMoving:
                {
                    //if (rayReceiver.tag == "Cell")
                    //{
                    //    int iFrom = GameController.ZToI(gameController.GetCurrentPlayer().GetPosition.z);
                    //    int jFrom = GameController.XToJ(gameController.GetCurrentPlayer().GetPosition.x);
                        
                    //    int iTo = GameController.ZToI(rayReceiver.transform.position.z);
                    //    int jTo= GameController.XToJ(rayReceiver.transform.position.x);
                        
                    //    List<Cell> list = cellsHolder.GetVisitedCells(new Vector2Int(iFrom, jFrom), new Vector2Int(iTo, jTo));
                    //    if (!cellsHolder.IsUp)
                    //        cellsHolder.LiftCells(list);
                    //    else
                    //    {
                    //        cellsHolder.ResetLift();
                    //        cellsHolder.LiftCells(list);
                    //    }
                    //}
                    break;
                }
        }
    }
}
