using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Doll : GazeableObject
{
    #region Variables

    [Space(10)]
    [SerializeField] private GameObject _table;
    [SerializeField] private int _sceneNum;
    [SerializeField] private float _waitTime = 3f;

    private bool _isTriggered = false;
    private AudioSource _tableAudioSource;

    #endregion




    #region Unity Functions

    private void Start()
    {
        _tableAudioSource = _table.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject == _table && transform.position.y >= 1.2f)
        {
            StartCoroutine(TriggerWaitSeconds());
        }
    }

    #endregion




    #region Coroutines

    private IEnumerator TriggerWaitSeconds()
    {
        if(!_isTriggered)
        {
            _isTriggered = true;
            _tableAudioSource.Play();
            yield return new WaitForSeconds(_waitTime);
            SceneManager.LoadScene(_sceneNum, LoadSceneMode.Single);
        }
    }

    #endregion
}