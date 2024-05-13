using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIspots : GazeableObject
{
    public override void OnPress(RaycastHit hitInfo)
    {
        base.OnPress(hitInfo);

        if (Player.Instance.ActiveMode == InputMode.POI)
        {
            Vector3 destLocation = hitInfo.transform.position;

            destLocation.y = Player.Instance.transform.position.y;

            Player.Instance.transform.position = destLocation;
        }
    }
}
