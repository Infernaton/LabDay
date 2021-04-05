using System;//Added this library to use Observer Design Pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    [SerializeField] new string name;
    [SerializeField] Sprite sprite;
    public float runSpeed; //Run Speed value

    private Vector2 input; // For getting the Input

    private Character character;

    private AudioSource[] myAudio;
    private AudioSource musicBackground;
    private AudioSource introTallGrass;

    //With this void Awake, we set the Animator so it plays the animation of the direction the player asked
    private void Awake()
    {
        character = GetComponent<Character>(); //Set the character object
    }
    private void Start()
    {
        myAudio = character.transform.GetChild(0).GetComponents<AudioSource>();
        musicBackground = myAudio[0];
        introTallGrass = myAudio[1];
        OnMoveOver();

    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            //Get rid of Diagonal movement
            if (input.x != 0) input.y = 0;
            if (input.y != 0) input.x = 0;
            print(input);

            //While the player is not moving, we read the input, and move the player in the choosen direction
            if (input != Vector2.zero)
            {
                if (Input.GetKey(KeyCode.B)) {
                    StartCoroutine(character.Move(input, OnMoveOver, true));
                }
                else {
                    StartCoroutine(character.Move(input, OnMoveOver));
                }
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            Interact();
    }

    //Interaction function
    void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);  //Direction the player is facing
        var interactPos = transform.position + facingDir; //this is the position of the tile the player is facing

        // Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.i.InteractableLayer); //If there is any interactable objects within a radius of 0.3, this will return a collider
        if (collider != null) //If there is an interactable object in that tile
        {
            collider.GetComponent<Interactable>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position - new Vector3(0, character.OffSetY), 0.2f, GameLayers.i.TriggerableLayers);

        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                character.Animator.IsMoving = false; //Set it to false when a battle appear
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }
    public void LoseBattle()
    {
        StartCoroutine(tpToPkmnCenter());
    }

    IEnumerator tpToPkmnCenter()
    {
        yield return SceneManager.LoadSceneAsync(5); //Load the Pokemon center scene
        character.SetPositionAndSnapToTile(new Vector2(13, 47));
        yield return character.Move(new Vector2(0.0f, 1.0f)); //To look forward the npc

        Interact(); //To interract with it
    }
    
    public void StopMusic(AudioSource audio)
    {
        audio.Stop();
        Debug.Log("Mute ");
    }
    public void PlayMusic(AudioSource audio)
    {
        audio.Play();
        Debug.Log("Play ");
    }

    //Properties to expose names and sprites
    public string Name
    {
        get => name;
    }
    public Sprite Sprite
    {
        get => sprite;
    }
    //To get the music that we play
    public AudioSource MusicBackground
    {
        get => musicBackground;
    }
    public AudioSource IntroTallGrass
    {
        get => introTallGrass;
    }
    public Character Character
    {
        get => character;
    } 
}