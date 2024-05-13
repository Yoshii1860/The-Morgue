using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class PinPad : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("Settings")]
    [SerializeField] private float _lerpSpeed = 5f;
    [SerializeField] private Vector3 _targetPosition = new Vector3(-4.808f, 1.926f, -1.963f);
    [SerializeField] private Vector3 _targetRotationAngles = new Vector3(7.772f, 270.588f, -0.405f);
    private Quaternion _targetRotation;
    private Vector3 _originalPosition;
    [Space(10)]

    [Header("Audio")]
    [SerializeField] private AudioClip _wrongButtonClip;
    [SerializeField] private AudioClip _triggerButtonClip;
    [SerializeField] private AudioClip _clickClip;
    private AudioSource _audioSourcePinPad;
    [SerializeField] private AudioSource audioSourceCorpseDrawer;
    [Space(10)]

    [Header("Particles")]
    [SerializeField] ParticleSystem steamParticles;

    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject _player;
    [SerializeField] private Animator _corpseDrawerAnimator;
    [SerializeField] private BoxCollider _corpseDrawerCollider;

    private List<int> pressedButtons = new List<int>();

    #endregion




    #region Unity Functions

    private void Start()
    {
        // Adjust target rotation angles to match the device's coordinate system
        if (XRSettings.enabled && XRDevice.GetTrackingSpaceType() == TrackingSpaceType.RoomScale)
        {
            _targetRotationAngles.y += 180f;
            _targetRotationAngles.z *= -1f; // Invert the Z-axis rotation
        }
        _audioSourcePinPad = GetComponent<AudioSource>();

        _targetRotation = Quaternion.Euler(_targetRotationAngles);

        GazeableObject.OnButtonPress += HandleButtonPress;
    }

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);
        float distance = Vector3.Distance(hitInfo.point, Camera.main.transform.position);
        if (distance <= DistanceToObjectThreshold)
        {
            transform.GetComponent<BoxCollider>().enabled = false;

            // Save the original position before lerping
            _originalPosition = _player.transform.position;

            // Disable head tracking
            XRDevice.DisableAutoXRCameraTracking(Camera.main, true);

            // Smoothly interpolate the position and rotation
            StartCoroutine(LerpPlayerToTarget());
        }
    }

    #endregion




    #region Private Functions

    private void HandleButtonPress(int buttonIndex)
    {
        // Store the pressed button index
        pressedButtons.Add(buttonIndex);

        _audioSourcePinPad.PlayOneShot(_clickClip, 1f);

        // Check if the correct sequence was entered
        if (CheckButtonSequence())
        {
            // Correct button sequence entered, trigger something here
            StartCoroutine(OpenDoor());
        }
    }

    private bool CheckButtonSequence()
    {
        // Define the correct button sequence
        List<int> correctSequence = new List<int> { 1, 8, 6, 0 };

        // Check if the pressed button sequence matches the correct sequence
        if (pressedButtons.Count == correctSequence.Count)
        {
            for (int i = 0; i < correctSequence.Count; i++)
            {
                if (pressedButtons[i] != correctSequence[i])
                {
                    pressedButtons.Clear();
                    transform.GetComponent<BoxCollider>().enabled = true;
                    // Disable head tracking
                    XRDevice.DisableAutoXRCameraTracking(Camera.main, true);
                    StartCoroutine(LerpPlayerToOriginalPosition());
                    _audioSourcePinPad.PlayOneShot(_wrongButtonClip, 1f);
                    return false; // Incorrect sequence, go back to original state, play sound and empty list
                }
            }
            return true; // Correct sequence - play sound, go back to original state and trigger animation
        }

        return false; // Incomplete sequence
    }

    #endregion




    #region Coroutines

    private IEnumerator LerpPlayerToTarget()
    {
        Player.Instance.ActiveMode = InputMode.INTERACT;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _lerpSpeed;

            // Lerp the position
            _player.transform.position = Vector3.Lerp(_player.transform.position, _targetPosition, t);

            yield return null;
        }

        // Enable head tracking after lerping
        XRDevice.DisableAutoXRCameraTracking(Camera.main, false);
    }

    private IEnumerator LerpPlayerToOriginalPosition()
    {
        float t = 0f;
        Vector3 currentPosition = _player.transform.position;

        while (t < 1f)
        {
            t += Time.deltaTime * _lerpSpeed;

            // Lerp the position back to the original position
            _player.transform.position = Vector3.Lerp(currentPosition, _originalPosition, t);

            yield return null;
        }

        // Enable head tracking after lerping back to the original position
        XRDevice.DisableAutoXRCameraTracking(Camera.main, false);

        Player.Instance.ActiveMode = base.SavedMode;
    }

    private IEnumerator OpenDoor()
    {
        XRDevice.DisableAutoXRCameraTracking(Camera.main, true);
        _audioSourcePinPad.PlayOneShot(_triggerButtonClip, 1f);
        StartCoroutine(LerpPlayerToOriginalPosition());
        _corpseDrawerAnimator.SetTrigger("PinPad");
        steamParticles.Play();
        audioSourceCorpseDrawer.Play();
        yield return new WaitWhile(()=>audioSourceCorpseDrawer.isPlaying);
        steamParticles.Stop();
        _corpseDrawerCollider.enabled = false;
    }

    #endregion
}