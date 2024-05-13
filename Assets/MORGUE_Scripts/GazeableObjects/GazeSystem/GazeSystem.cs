using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeSystem : MonoBehaviour 
{
    #region Variables

    [SerializeField] private GameObject _reticle;

    [SerializeField] private Color _inactiveReticleColor = Color.gray;
    [SerializeField] private Color _activeReticleColor;

    private GazeableObject _currentGazeObject;
    private GazeableObject _currentSelectedObject;

    private RaycastHit lastHit;

    private bool isCrRunning = false;
    private Coroutine _scaleCoroutine;

    #endregion




    #region Unity Functions

    private void Start () 
    {
        _reticle.GetComponent<Renderer>().material.color = _inactiveReticleColor;
    }
        
    private void Update () 
    {
        ProcessGaze ();
        CheckForInput(lastHit); 
    }

    #endregion




    #region Public Functions

    public void ProcessGaze()
    {
        Ray raycastRay = new Ray (transform.position, transform.forward);
        RaycastHit hitInfo;

        Debug.DrawRay (raycastRay.origin, raycastRay.direction * 100);

        if(Physics.Raycast(raycastRay, out hitInfo))
        {
            // Do something to the object

            // Check if the object is interactable

            // Get the GameObject from the hitInfo
            GameObject hitObj = hitInfo.collider.gameObject;

            // Get the GazeableObject from the hit Object
            GazeableObject gazeObj = hitObj.GetComponentInParent<GazeableObject> ();

            // Object has a GazeableObject componenet
            if (gazeObj != null) 
            {
    
                // Object we're looking at is different
                if (gazeObj != _currentGazeObject) 
                {
                    ClearCurrentObject ();
                    _currentGazeObject = gazeObj;
                    _currentGazeObject.OnGazeEnter (hitInfo);
                    if (isCrRunning)
                    {
                        StopCoroutine(_scaleCoroutine);
                    }
                    _scaleCoroutine = StartCoroutine(ScaleOverTime(_reticle.transform, new Vector3(0.025f, 0.025f, 0.025f), 1f, true));
                } 
                else 
                {
                    _currentGazeObject.OnGaze (hitInfo);
                }

                lastHit = hitInfo;
            } 
            else
            {
                ClearCurrentObject ();
            }
        }
    }

    #endregion




    #region Private Functions

    private void CheckForInput(RaycastHit hitinfo)
    {
        // Check for down
        if (Input.GetMouseButtonDown (0) && _currentGazeObject != null) 
        {
            _currentSelectedObject = _currentGazeObject;
            _currentSelectedObject.OnPress(hitinfo);
        } 

        // Check for hold
        else if (Input.GetMouseButton (0) && _currentSelectedObject != null) 
        {
            _currentSelectedObject.OnHold(hitinfo);
        } 

        // Check for release
        else if (Input.GetMouseButtonUp (0) && _currentSelectedObject != null) 
        {
            _currentSelectedObject.OnRelease(hitinfo);
            _currentSelectedObject = null;
        } 
    }

    private void ClearCurrentObject()
    {
        if(_currentGazeObject != null)
        {
            if (isCrRunning)
            {
                StopCoroutine(_scaleCoroutine);
            }
            _scaleCoroutine = StartCoroutine(ScaleOverTime(_reticle.transform, new Vector3(0.01f, 0.01f, 0.01f), 0.5f, false));
            _currentGazeObject.OnGazeExit();
            _currentGazeObject = null;
        }
    }

    #endregion




    #region Coroutines

    private IEnumerator ScaleOverTime(Transform objectToScale, Vector3 toScale, float duration, bool enter)
    {
        isCrRunning = true;
        float counter = 0;

        //Get the current scale of the object to be moved
        Vector3 startScaleSize = objectToScale.localScale;
        Vector3 targetScaleSize = toScale;

        if (_currentGazeObject.tag == "Floor" && Player.Instance.ActiveMode == InputMode.TELEPORT || _currentGazeObject.tag == "Floor" && Player.Instance.ActiveMode == InputMode.POI)
        {
            _activeReticleColor = Color.green;
        }
        else
        {
           _activeReticleColor = Color.red;
        }

        if (objectToScale.localScale == targetScaleSize)
        {
            isCrRunning = false;
            yield break;
        }

        while (counter < duration)
        {
            counter += Time.deltaTime;

            // Use a minimum duration for scaling
            float t = Mathf.Clamp01(counter / duration);
            if (t >= 1f)
            {
                // Snap to the target scale if the duration is reached
                objectToScale.localScale = targetScaleSize;
            }
            else
            {
                objectToScale.localScale = Vector3.Lerp(startScaleSize, targetScaleSize, t);
            }

            if (enter == true)
            {
                _reticle.GetComponent<Renderer>().material.color = Color.Lerp(_inactiveReticleColor, _activeReticleColor, counter / duration);
            }
            else if (enter == false)
            {
                _reticle.GetComponent<Renderer>().material.color = Color.Lerp(_activeReticleColor, _inactiveReticleColor, counter / duration);
            }
            yield return null;
        }
        isCrRunning = false;
    }

    #endregion
}
