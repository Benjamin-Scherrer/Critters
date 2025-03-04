using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leveler : MonoBehaviour
{
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
    }
}
