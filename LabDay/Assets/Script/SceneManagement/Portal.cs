using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] char destinationPortal;
    [SerializeField] bool keepOldPos;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    GameController gameController;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        
        StartCoroutine(SwitchScene());
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log("YEYE");

        GameController.Instance.PauseGame(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        try
        {
            var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
            if (!keepOldPos)
            {
                Debug.Log("AAAAAA");
                Debug.Log(destPortal.SpawnPoint.position);
                player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
            }

        }
        catch
        {
            Debug.Log("InitScene");
        }

        
        Destroy(gameObject);
        GameController.Instance.PauseGame(false);
    }
    public Transform SpawnPoint => spawnPoint;
}
