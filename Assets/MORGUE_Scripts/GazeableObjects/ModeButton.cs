using UnityEngine;

public class ModeButton : GazeableButton
{
    [Space(10)]
    [Header("Mode")]
    [SerializeField] private InputMode _mode;

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (parentPanel.CurrentActiveButton != null)
        {
            Player.Instance.ActiveMode = _mode;
        }
    }
}
