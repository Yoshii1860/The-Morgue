using UnityEngine;

public class Floor : GazeableObject
{
    #region Public Functions

    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (Player.Instance.ActiveMode == InputMode.TELEPORT)
        {
            TeleportPlayer(hitInfo.point);
        }
    }

    #endregion




    #region Private Functions

    private void TeleportPlayer(Vector3 destination)
    {
        destination.y = Player.Instance.transform.position.y;
        Player.Instance.transform.position = destination;
    }

    #endregion
}