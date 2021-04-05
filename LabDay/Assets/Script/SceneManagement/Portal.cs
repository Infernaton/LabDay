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

    public void OnPlayerTriggered(PlayerController player)
    {
        this.player = player;
        
        StartCoroutine(SwitchScene());
    }

    Fader fader;

    private void Start()
    {
        fader = FindObjectOfType<Fader>();
    }

    IEnumerator SwitchScene()
    {
        DontDestroyOnLoad(gameObject);
        yield return fader.FadeIn(0.5f);

        GameController.Instance.PauseGame(true);
        yield return SceneManager.LoadSceneAsync(sceneToLoad);

        try
        {
            var destPortal = FindObjectsOfType<Portal>().First(x => x != this && x.destinationPortal == this.destinationPortal);
            if (!keepOldPos)
            {
                player.Character.SetPositionAndSnapToTile(destPortal.SpawnPoint.position);
            }

        }
        catch
        {
            Debug.Log("InitScene");
        }

        yield return fader.FadeOut(0.5f);

        GameController.Instance.PauseGame(false);
        Destroy(gameObject);
    }
    public Transform SpawnPoint => spawnPoint;
}
