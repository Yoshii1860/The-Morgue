using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBillboard : MonoBehaviour
{
    #region Variables

    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _followDistance = 2f; // Distance between the menu and camera
    [SerializeField] private float _smoothTime = 0.3f; // Smoothing time for the menu movement
    [SerializeField] private float _rotationThreshold = 45f; // Rotation threshold in degrees

    private Vector3 _targetPosition; // Target position of the menu
    private Quaternion _targetRotation; // Target rotation of the menu
    private float _initialXRotation; // Initial x-rotation of the menu
    private float _initialYPosition; // Initial y-position of the menu
    private Vector3 _smoothVelocity; // Smooth velocity for SmoothDamp

    #endregion




    #region Unity Methods

    private void Start()
    {
        _initialXRotation = transform.rotation.eulerAngles.x;
        _initialYPosition = transform.position.y;
    }

    private void Update()
    {
        // Calculate the angle between the forward vector of the camera and the menu's forward vector
        float angle = Vector3.Angle(_cameraTransform.forward, transform.forward);

        // Only update the menu's position and rotation if the angle is beyond the rotation threshold
        if (angle >= _rotationThreshold)
        {
            // Calculate the target position
            _targetPosition = _cameraTransform.position + _cameraTransform.forward * _followDistance;
            _targetPosition.y = _initialYPosition; // Maintain the initial y-position

            // Update the menu position with smoothing
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _smoothVelocity, _smoothTime);

            // Update the menu rotation to face the player
            _targetRotation = Quaternion.LookRotation(transform.position - _cameraTransform.position);
            _targetRotation.eulerAngles = new Vector3(_initialXRotation, _targetRotation.eulerAngles.y, _targetRotation.eulerAngles.z);
            transform.rotation = _targetRotation;
        }
    }

    #endregion
}





