using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowHideInfo : MonoBehaviour
{
    [SerializeField]
    private GameObject showHideObject;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
            showHideObject.SetActive(!showHideObject.activeSelf);
    }
}
