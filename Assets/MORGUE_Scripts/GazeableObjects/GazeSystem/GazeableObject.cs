using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GazeableObject : MonoBehaviour
{
    #region Variables

    [HideInInspector] public delegate void ButtonPressHandler(int ButtonIndex);
    [HideInInspector] public int ButtonIndex;
    [HideInInspector] public static event ButtonPressHandler OnButtonPress;

    [Header("Object Properties")]
    [SerializeField] private bool _isTranslateable = false;
    [SerializeField] private bool _isDisplaySave = false;
    [SerializeField] private bool _isRotateable = false;
    [Space(10)]

    [Header("Emission Colors")]
    [SerializeField] private Color _emissionColor = new Color(0.8f, 0.8f, 0.8f, 1f);
    [SerializeField] private Color _emissionColorRevert = new Color(0, 0, 0, 1f);
    [Space(10)]

    [Header("Object Label")]
    [SerializeField] private GameObject _label;
    [SerializeField] private string _objectName;
    public float DistanceToObjectThreshold = 3.5f;
    [Space(10)]

    [Header("Collision Audio")]
    [SerializeField] private AudioClip collisionAudioClip;

    [HideInInspector] public InputMode SavedMode;
    private RaycastHit _savedHitInfo;
    private Vector3 _initialObjectRotation;
    private Vector3 _initialPlayerRotation;
    private Outline _outlinerScript;
    private AudioSource _audioSourceForAll;

    #endregion




    #region Unity Functions

    private void OnEnable() 
    {

        if (_label != null && _label.activeSelf)
        {
            _label.SetActive(false);
        }
        
        if (transform.tag != "Floor" && transform.tag != "POI" && transform.tag != "Button")
        {
            _outlinerScript = GetComponent<Outline>();
            if (_outlinerScript != null)
            {
                _outlinerScript.enabled = false;
            }
        }

        _audioSourceForAll = GetComponent<AudioSource>();
        if (_audioSourceForAll == null && _isTranslateable && !_isDisplaySave)
        {
            _audioSourceForAll = gameObject.AddComponent<AudioSource>();
            _audioSourceForAll.playOnAwake = false;
            _audioSourceForAll.spatialBlend = 1f; // Set to 3D audio
        }
    }

    #endregion




    #region Public Functions

    public virtual void OnGazeEnter(RaycastHit hitInfo)
    {
        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null && transform.tag != "Floor" && transform.tag != "POI")
        {
            renderer.material.SetColor("_EmissionColor", _emissionColor);
            if (_outlinerScript != null)
            {
                _outlinerScript.enabled = true;
            }

            float distance = Vector3.Distance(hitInfo.point, Camera.main.transform.position);
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Interactive") && distance <= DistanceToObjectThreshold)
            {
                _label.SetActive(true);
                _label.GetComponentInChildren<TextMeshProUGUI>().text = _objectName;
            }
        }
    }

    public virtual void OnGaze (RaycastHit hitInfo)
    {
        Debug.Log("Gaze");
    }

    public virtual void OnGazeExit()
    {
        if (_label != null && _label.activeSelf)
        {
            _label.SetActive(false);
        }

        Renderer renderer = transform.GetComponent<Renderer>();
        if (renderer != null && transform.tag != "Floor" && transform.tag != "POI")
        {
            renderer.material.SetColor("_EmissionColor", _emissionColorRevert);
            if (_outlinerScript != null)
            {
                _outlinerScript.enabled = false;
            }
        }
    }

    public virtual void OnPress(RaycastHit hitInfo)
    {
        SavedMode = Player.Instance.ActiveMode;
        _savedHitInfo = hitInfo;

        if (_isTranslateable)
        {
            Player.Instance.ActiveMode = InputMode.TRANSLATE;
        }
        else if (_isRotateable)
        {
            Player.Instance.ActiveMode = InputMode.ROTATE;
            _initialObjectRotation = transform.rotation.eulerAngles;
            _initialPlayerRotation = Camera.main.transform.eulerAngles;
        }

        if (_label != null && _label.activeSelf)
        {
            _label.SetActive(false);
        }

        if (transform.gameObject.tag == "PinPad")
        {
            if (ButtonIndex >= 0)
            {
                OnButtonPress?.Invoke(ButtonIndex);
            }
        }
    }

    public virtual void OnHold(RaycastHit hitInfo)
    {
        if (_isTranslateable)
        {
            if(_isDisplaySave)
            {
                DisableColliders(hitInfo.transform);
            }

            Vector3 desiredPosition = Camera.main.transform.position + (Camera.main.transform.forward * _savedHitInfo.distance);
            transform.position = desiredPosition;
        }

        if (_isRotateable)
        {
            float rotationSpeed = 10.0f;

            Vector3 currentPlayerRotation = Camera.main.transform.rotation.eulerAngles;
            Vector3 currentObjectRotation = transform.rotation.eulerAngles;

            Vector3 rotationDelta = currentPlayerRotation - _initialPlayerRotation;

            Vector3 newRotation = new Vector3(currentObjectRotation.x, _initialObjectRotation.y + (rotationDelta.y * rotationSpeed), currentObjectRotation.z);

            transform.rotation = Quaternion.Euler(newRotation);
        }
    }
        
    public virtual void OnRelease(RaycastHit hitInfo)
    {
        if (Player.Instance.ActiveMode != SavedMode && _savedHitInfo.transform.gameObject.layer != LayerMask.NameToLayer("UI"))
        {
            Player.Instance.ActiveMode = SavedMode;
        }

        if (_isDisplaySave)
        {
            Player.Instance.currentObject = _savedHitInfo.transform.gameObject;
            string childName = _savedHitInfo.transform.gameObject.name;
            _savedHitInfo.transform.gameObject.SetActive(false);
            GameObject childObject = Player.Instance.StoredObjects.transform.Find(childName).gameObject;
            childObject.SetActive(true);
            Player.Instance.DisplayedObject = childObject;
        }

        if (_isTranslateable && !_isDisplaySave && hitInfo.collider != null)
        {
            PlayCollisionSound();
        }
    }

    #endregion




    #region Private Functions

    private protected virtual void DisableColliders(Transform transform)
    {
        Collider collider = transform.GetComponent<Collider>();
        if (collider != null && collider.enabled)
        {
            collider.enabled = false;
        }

        foreach (Transform child in transform)
        {
            DisableColliders(child);
        }
    }

    private protected virtual void PlayCollisionSound()
    {
        if (_audioSourceForAll != null && collisionAudioClip != null)
        {
            _audioSourceForAll.clip = collisionAudioClip;
            _audioSourceForAll.Play();
        }
    }

    #endregion
}
