using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum InputMode
{
    NONE,
    TELEPORT,
    WALK,
    POI,
    TRANSLATE,
    ROTATE,
    INTERACT
}

public class Player : MonoBehaviour
{
    #region Variables

    public static Player Instance;

    [Header("Room Boundaries")]
    [SerializeField] private GameObject _leftWall;
    [SerializeField] private GameObject _rightWall;
    [SerializeField] private GameObject _frontWall;
    [SerializeField] private GameObject _backWall;
    [SerializeField] private GameObject _floorObject;
    [Space(10)]

    [Header("Player Objects")]
    public GameObject StoredObjects;
    public GameObject DisplayedObject;
    public GameObject currentObject;
    [Space(10)]

    [Header("Modes")]
    public InputMode ActiveMode = InputMode.NONE;
    [Space(10)]

    [Header("POIs")]
    public GameObject[] POIs;
    [Space(10)]

    [Header("Player Speed")]
    [SerializeField] private float playerSpeed = 3.0f;

    private bool _isPoiActive = false;

    #endregion




    #region Unity Functions

    private void Awake() 
    {
        if (Instance != null)
        {
            GameObject.Destroy(Instance.gameObject);
        }

        Instance = this;

        if (POIs != null)
        {
            foreach (GameObject poi in POIs) 
            {
                poi.SetActive(false);
            }
        }

        Destroy(_floorObject.GetComponent("Floor"));

        if (!GetComponent<AudioSource>().enabled)
        {
            GetComponent<AudioSource>().enabled = true;
        }
    }

    private void Update()
    {
        TryWalk();
        ActivatePOI();
        CheckFloor();
    }

    #endregion




    #region Private Functions

    private void CheckFloor()
     {
        if (ActiveMode == InputMode.TELEPORT)
        {
            if (_floorObject.GetComponent<Floor>() == null)
            {
                 _floorObject.AddComponent<Floor>();
            }
        }
        else if (ActiveMode != InputMode.TELEPORT)
        {
            if (_floorObject.GetComponent<Floor>() != null)
            {
                Destroy(_floorObject.GetComponent("Floor"));
            }
        }
     }

    private void ActivatePOI()
    {
       if (ActiveMode == InputMode.POI && _isPoiActive == false)
        {
            foreach (GameObject poi in POIs) 
            {
                if (!poi.activeSelf)
                {
                    poi.SetActive(true);
                }
                _isPoiActive = true;
            }
        }
        else if (ActiveMode != InputMode.POI && _isPoiActive == true)
        {
            foreach (GameObject poi in POIs) 
            {
                if (poi.activeSelf)
                {
                    poi.SetActive(false);
                }
                _isPoiActive = false;
            }
        }
    }

    #endregion




    #region Public Functions

    public void TryWalk()
    {
        if (Input.GetMouseButton(0) && ActiveMode == InputMode.WALK)
        {
            Vector3 forward = Camera.main.transform.forward;

            Vector3 newPosition = transform.position + forward * Time.deltaTime * playerSpeed;

            newPosition.y = transform.position.y;

            if (
                newPosition.x < _rightWall.transform.position.x - 0.5 &&
                newPosition.x > _leftWall.transform.position.x + 0.5 && 
                newPosition.z > _backWall.transform.position.z + 0.5 &&
                newPosition.z < _frontWall.transform.position.z - 0.5
                )
            {
                transform.position = newPosition;
            }
        }
    }

    #endregion
}
