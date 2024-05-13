using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]

public class GazeableButton : GazeableObject
{
    #region Variables

    private protected VRCanvas parentPanel;

    #endregion




    #region Unity Functions

    private void Start()
    {
        parentPanel = GetComponentInParent<VRCanvas>();
    }

    #endregion




    #region Public Functions

    public void SetButtonColor(Color buttonColor)
    {
        GetComponent<Image>().color = buttonColor;
    }

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (parentPanel != null)
        {
            parentPanel.SetActiveButton(this);
        }
        else
        {
            Debug.LogError("Button not a child of object with VRPanel component.", this);
        }
    }

    #endregion
}
