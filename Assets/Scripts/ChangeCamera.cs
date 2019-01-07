using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeCamera : MonoBehaviour
{
    public Camera main;
    public Camera top;

    public void Change()
    {
        top.enabled = !top.enabled;
        top.GetComponent<ResetChipRotation>().enabled = !top.GetComponent<ResetChipRotation>().enabled;
        main.GetComponent<FingerRotator>().enabled = !main.GetComponent<FingerRotator>().enabled;

        Text buttonText = GetComponentInChildren<Text>();
        if (buttonText.text.EndsWith("3D"))
            buttonText.text = buttonText.text.Replace("3D", "2D");
        else
            buttonText.text = buttonText.text.Replace("2D", "3D");
    }
}
