using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] int sceneToLoad = -1;
    [SerializeField] bool keepOldPos;
    [SerializeField] Transform spawnPoint;

    PlayerController player;
    GameController gameController;

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        
        StartCoroutine(SwitchScene());
        gameController.State = GameState.Cutscene;
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);

        //
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        var destPortal = FindObjectsOfType<Portal>().First(x => x != this);

        Debug.Log(destPortal.SpawnPoint.position);

        if (!keepOldPos)
        {
            player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
        }

        Destroy(gameObject);
        gameController.State = GameState.FreeRoam;
    }
    public Transform SpawnPoint => spawnPoint;
}
