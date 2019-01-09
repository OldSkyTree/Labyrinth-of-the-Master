using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowVisible : MonoBehaviour
{
    private GameController gameController;

    void Start()
    {
        gameController = GameController.GetGameController();
    }
    
    void Update()
    {
        if (gameController.GetGameMode().Equals(GameController.GameMode.LineChoose))
            GetComponentInChildren<MeshRenderer>().enabled = true;
        else
            GetComponentInChildren<MeshRenderer>().enabled = false;
    }
}
