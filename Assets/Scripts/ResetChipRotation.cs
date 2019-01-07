using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChipRotation : MonoBehaviour
{
    void Update()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        for (int i = 0; i < allObjects.Length; i++)
        {
            if (allObjects[i].tag.Equals("Chip"))
            {
                allObjects[i].transform.rotation = Quaternion.identity;
            }
        }
    }
}
