using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    #region Variables

    private Transform _mainCamera;

    #endregion




    #region Unity Methods

    void Start()
    {
        _mainCamera = GetComponentInParent<GazeSystem>().transform;
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(_mainCamera.forward);
    }

    #endregion
}