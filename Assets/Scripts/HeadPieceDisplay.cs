using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadPieceDisplay : MonoBehaviour
{
    [SerializeField] private Geobody geobody;
    [SerializeField] private GameObject displayObject; // The GameObject to activate

    private void FixedUpdate()
    {
        if (geobody != null && displayObject != null)
        {

            if (geobody.GetGeobodyType == GeobodyType.MainHead && !displayObject.activeSelf)
            {
                displayObject.SetActive(true);
            }
            else if(geobody.GetGeobodyType != GeobodyType.MainHead)
            {
                displayObject.SetActive(false);
            }

        }
    }
}