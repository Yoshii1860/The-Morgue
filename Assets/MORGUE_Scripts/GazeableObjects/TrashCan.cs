using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("References")]
    [Tooltip("The flashlight from the player")]
    [SerializeField] private GameObject _flashlight;
    [Tooltip("The flashlight from the trash can")]
    [SerializeField] private Animator _flashlightAnimator;

    private Player _player;

    #endregion




    #region Unity Functions

    private void Start() 
    {
        _player = Player.Instance;
    }

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);
        StartCoroutine(FlashlightPickup());
    }

    public override void OnRelease(RaycastHit hitInfo)
    {
        base.OnRelease(hitInfo);
    }

    #endregion




    #region Coroutines
    
    private IEnumerator FlashlightPickup()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1);

        _flashlightAnimator.SetTrigger("Lift");
        yield return new WaitForSeconds(_flashlightAnimator.GetCurrentAnimatorStateInfo(0).length + 1);

        _player.ActiveMode = base.SavedMode;
        _flashlightAnimator.transform.gameObject.SetActive(false);
        GetComponent<Outline>().enabled = false;
        _flashlight.SetActive(true);
        yield return new WaitForSeconds(1);
        
        Destroy(GetComponent<GazeableObject>());
    }

    #endregion
}
