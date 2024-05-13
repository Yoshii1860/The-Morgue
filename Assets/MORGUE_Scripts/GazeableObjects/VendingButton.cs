using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VendingButton : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject _bonesaw;
    [SerializeField] private GameObject _coins;
    [Space(10)]

    [Header("Audio")]
    [SerializeField] private AudioClip _useMachineClip;
    [SerializeField] private AudioClip _clickButton;

    private AudioSource _audioSource;
    private bool _isUsed = false;

    #endregion




    #region Unity Functions

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _bonesaw.GetComponent<BoxCollider>().enabled = false;
        _bonesaw.GetComponent<MeshRenderer>().enabled = false;
    }

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        if (Player.Instance.currentObject == _coins && !_isUsed)
        {
            base.OnPress(hitInfo);
            _isUsed = true;
            StartCoroutine(VendingTrigger());
        }
        else
        {
            GetComponent<AudioSource>().PlayOneShot(_clickButton);
        }
    }

    #endregion




    #region Coroutines

    private IEnumerator VendingTrigger()
    {
        Destroy(Player.Instance.DisplayedObject);
        Destroy(Player.Instance.currentObject);
        Player.Instance.currentObject = null;
        Player.Instance.DisplayedObject = null;
        GetComponent<AudioSource>().PlayOneShot(_useMachineClip);
        yield return new WaitWhile (()=> _audioSource.isPlaying);
        _bonesaw.GetComponent<BoxCollider>().enabled = true;
        _bonesaw.GetComponent<MeshRenderer>().enabled = true;
    }

    #endregion
}
