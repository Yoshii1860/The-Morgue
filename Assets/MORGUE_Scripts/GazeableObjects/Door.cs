using System.Collections;
using UnityEngine;

public class Door : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("Objects")]
    [SerializeField] private GameObject _key;
    [SerializeField] private GameObject _openMenu;
    [SerializeField] private GameObject _reticle;
    [SerializeField] private GameObject _endCanvas;
    [Space(10)]

    [Header("Audio")]
    [SerializeField] private AudioClip _doorOpenClip;
    [SerializeField] private AudioClip _doorClosed;
    [Space(10)]

    [Header("Mesh")]
    [SerializeField] private MeshRenderer _blackoutRenderer;

    private AudioSource _audioSourceDoor;
    private Material _blackoutMaterial;
    private float _lerpDuration = 1f;

    #endregion




    #region Unity Functions

    private void Start()
    {
        _audioSourceDoor = GetComponent<AudioSource>();
        _blackoutMaterial = _blackoutRenderer.material;
    }

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (Player.Instance.currentObject == _key)
        {
            StartCoroutine(FinishLevel());
        }
        else
        {
            _audioSourceDoor.PlayOneShot(_doorClosed, 1f);
        }
    }

    #endregion




    #region Coroutines

    private IEnumerator FinishLevel()
    {
        _openMenu.SetActive(false);
        _audioSourceDoor.PlayOneShot(_doorOpenClip, 1f);
        Player.Instance.DisplayedObject.SetActive(false);
        _reticle.SetActive(false);
        yield return new WaitForSeconds(6);
        GetComponent<Animator>().SetTrigger("Open");
        yield return new WaitForSeconds(4);
        yield return StartCoroutine(BlackoutCoroutine());
    }

    private IEnumerator BlackoutCoroutine()
    {
        Color startColor = Color.clear;
        Color targetColor = Color.black;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / _lerpDuration;

            Color lerpedColor = Color.Lerp(startColor, targetColor, t);
            _blackoutMaterial.color = lerpedColor;

            yield return null;
        }

        _blackoutMaterial.color = targetColor;
        _endCanvas.SetActive(true);
    }

    #endregion
}