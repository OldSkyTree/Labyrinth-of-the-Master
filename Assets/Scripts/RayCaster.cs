using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    private Camera currentCamera;

    private GameController gameController;
    
    void Start()
    {
        currentCamera = GetComponent<Camera>();
        gameController = GameController.GetGameController();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
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
                        Destroy(rayReceiver);
                    break;
                }
            case GameController.GameMode.FigureMoving:
                {
                    if (rayReceiver.tag == "Cell")
                    {
                        int iFrom = ZToI(gameController.GetCurrentPlayer().GetPosition.z);
                        int jFrom = XToJ(gameController.GetCurrentPlayer().GetPosition.x);
                        
                        int iTo = ZToI(rayReceiver.transform.position.z);
                        int jTo= XToJ(rayReceiver.transform.position.x);

                        Debug.Log(gameController.IsMoveAvailable(new Vector2Int(iFrom, jFrom), new Vector2Int(iTo, jTo)));
                        //Debug.Log("Up: " + gameController.GetCell(iTo, jTo).Up
                        //    + "| Right: " + gameController.GetCell(iTo, jTo).Right
                        //    + "| Down: " + gameController.GetCell(iTo, jTo).Down
                        //    + "| Left: " + gameController.GetCell(iTo, jTo).Left
                        //    + "| i=" + iTo + " j=" + jTo);
                    }
                    break;
                }
        }
    }

    int ZToI(float z)
    {
        return 3 - Mathf.RoundToInt(z / 2.05f);
    }
    int XToJ(float x)
    {
        return Mathf.RoundToInt(x / 2.05f) + 3;
    }
}
