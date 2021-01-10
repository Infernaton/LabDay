using UnityEngine;

public class ChangeToTab2 : MonoBehaviour
{
    public GameController gameController;

    void OnTriggerEnter ()
    {
        gameController.ChangeTab();
    }


}
