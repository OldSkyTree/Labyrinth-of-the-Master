using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMainCamera : MonoBehaviour
{
    private Transform mainCamera;

    void Start()
    {
        mainCamera = Camera.main.transform;
    }
    
    void Update()
    {
        Vector3 lookPos = mainCamera.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
    }
}
