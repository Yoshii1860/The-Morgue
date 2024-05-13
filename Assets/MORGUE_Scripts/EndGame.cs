using UnityEngine;

public class EndGame : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButton(0)) 
        {
            Application.Quit();
        }
    }
}
