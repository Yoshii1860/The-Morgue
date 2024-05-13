using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : GazeableObject
{

    #region Variables

    [SerializeField] private int _sceneNum;

    #endregion




    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        StartCoroutine(LoadWithSound());
    }

    #endregion




    #region Coroutines

    IEnumerator LoadWithSound()
    {
        transform.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(_sceneNum, LoadSceneMode.Single);
    }

    #endregion
}
