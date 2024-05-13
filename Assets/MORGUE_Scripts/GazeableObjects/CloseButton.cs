using UnityEngine;

public class CloseButton : GazeableObject
{
    #region Variables

    [Space(10)]
    [Header("Menu")]
    [SerializeField] private GameObject _menuCanvas;
    [SerializeField] private GameObject _menuButton;

    #endregion




    #region Unity Functions

    private void Start()
    {
        _menuButton.SetActive(false);
    }

    #endregion



    
    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (!_menuButton.activeSelf && Player.Instance.ActiveMode != InputMode.NONE)
        {
            ToggleUIElements(false);
        }
        else if (_menuButton.activeSelf)
        {
            ToggleUIElements(true);
        }
    }

    #endregion




    #region Private Functions

    private void ToggleUIElements(bool showCanvas)
    {
        _menuCanvas.SetActive(showCanvas);
        _menuButton.SetActive(!showCanvas);
    }

    #endregion
}