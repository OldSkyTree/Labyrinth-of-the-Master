using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCaster : MonoBehaviour
{
    private Camera currentCamera;

    void Start()
    {
        currentCamera = GetComponent<Camera>();
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
        int i = ZToI(rayReceiver.transform.position.z);
        int j = XToJ(rayReceiver.transform.position.x);
        Debug.Log("This cell in array cells[" + i + ", " + j + "]");
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
