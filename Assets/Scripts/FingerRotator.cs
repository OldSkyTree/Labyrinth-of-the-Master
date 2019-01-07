using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerRotator : MonoBehaviour
{
    public Transform rotationPoint;
    public float speed;

    public bool rotateChip;
    
    void Update()
    {
        float a = Input.GetAxis("Horizontal");
        transform.RotateAround(rotationPoint.position, Vector3.down, a * speed);
        if (rotateChip)
        {
            Vector3 lookPos = rotationPoint.position - transform.position;
            lookPos.y = 0;
            Quaternion rotate = Quaternion.LookRotation(lookPos);

            //var enumerator = GetComponent<GameController>().GetChips().Values.GetEnumerator();
            var chips = FindObjectsOfType<GameObject>();
            for (int i = 0; i < chips.Length; i++)
            {
                if (chips[i].name.Contains("Chip"))
                {
                    chips[i].transform.rotation = rotate;
                    chips[i].transform.Rotate(Vector3.up, 180);
                }
            }
        }
    }
}
