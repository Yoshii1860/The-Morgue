using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VRCanvas : MonoBehaviour
{
    #region Variables

    public GazeableButton CurrentActiveButton;

    [SerializeField] private Color _unselectedColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.green;

    #endregion




    #region Private Functions

    public void SetActiveButton(GazeableButton activeButton)
    {
        if (CurrentActiveButton != null)
        {
            CurrentActiveButton.SetButtonColor(_unselectedColor);
        }

        if (activeButton != null && CurrentActiveButton != activeButton)
        {
            CurrentActiveButton = activeButton;
            CurrentActiveButton.SetButtonColor(_selectedColor);
        }
        else
        {
            Debug.Log("Resetting");
            CurrentActiveButton = null;
            Player.Instance.ActiveMode = InputMode.NONE;
        }
    }

    #endregion
}
