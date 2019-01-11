using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeGameMode : MonoBehaviour
{
    private Scrollbar scrollbar;
    private GameController gameController;

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
        gameController = GameController.GetGameController();
    }

    public void Change()
    {
        if (scrollbar.value == 0)
            gameController.SetGameMode(4);
        if (scrollbar.value == 1)
            gameController.SetGameMode(2);
    }
}
