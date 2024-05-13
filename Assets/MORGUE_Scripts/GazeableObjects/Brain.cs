using System.Collections;
using UnityEngine;

public class Brain : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("Bonesaw")]
    [Tooltip("The bonesaw prefab")]
    [SerializeField] private GameObject _bonesawPrefab;
    [Tooltip("The bonesaw object in the vending machine")]
    [SerializeField] private GameObject _bonesaw;
    [Space(10)]

    [Header("Bonesaw POS/ROT")]
    [SerializeField] private Vector3 _posBonesaw;
    [SerializeField] private Quaternion _rotBonesaw;
    [Space(10)]

    [Header("Audio")]
    [SerializeField] private AudioClip _cuttingClip;

    private AudioSource _audioSource;

    #endregion




    #region Unity Functions

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        if (Player.Instance.currentObject == _bonesaw)
        {
            base.OnPress(hitInfo);
            StartCoroutine(CutBrain());
        }
        else
        {
            base.OnPress(hitInfo);
        }
    }

    #endregion




    #region Coroutines

    private IEnumerator CutBrain()
    {
        Player.Instance.currentObject = null;
        Player.Instance.DisplayedObject = null;
        Destroy(Player.Instance.DisplayedObject);
        Destroy(Player.Instance.currentObject);

        GameObject newBonesaw = Instantiate(_bonesawPrefab, _posBonesaw, _rotBonesaw);
        yield return new WaitForSeconds(1);

        var bonesawAnimator = newBonesaw.GetComponent<Animator>();
        bonesawAnimator.SetTrigger("Cut");

        _audioSource.PlayOneShot(_cuttingClip);

        yield return new WaitForSeconds(5);

        newBonesaw.SetActive(false);
        var sphereCollider = transform.GetComponent<SphereCollider>();
        var meshRenderer = transform.GetComponent<MeshRenderer>();
        sphereCollider.enabled = false;
        meshRenderer.enabled = false;

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }

    #endregion
}