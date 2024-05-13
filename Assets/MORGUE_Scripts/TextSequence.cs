using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSequence : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject _flashlight;
    [SerializeField] private GameObject[] _texts;
    private int _num = 0;
    private int _length = 0;

    #endregion




    #region Unity Functions

    private void Start()
    {
        foreach(GameObject text in _texts)
        {
            text.SetActive(false);
            _length++;
        }
        _texts[_num].SetActive(true);
    }

    private void Update()
    {
        TextSequenceFunc();
    }

    #endregion




    #region Functions

    private void TextSequenceFunc()
    {
        if(Input.GetMouseButtonDown (0))
        {
            _texts[_num].SetActive(false);
            _num++;
            if(_num < _length)
            {
                _texts[_num].SetActive(true);
            }
            else
            {
                _flashlight.SetActive(true);
                Player.Instance.ActiveMode = InputMode.TELEPORT;
                transform.gameObject.SetActive(false);
            }
        }
    }

    #endregion
}
