using System;//Added this library to use Observer Design Pattern
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] string name;
    [SerializeField] Sprite sprite;

    public event Action<Collider2D> OnEnterTrainersView; //Action for entering a trainer's view, with a collider 2d as parameter to know wich trainer we saw
    public event Action OnEncountered; //Creating an action we'll cal when an encounter appears

    private Vector2 input; // For getting the Input

    private Character character;

    private AudioSource[] myAudio;
    private AudioSource background;
    private AudioSource introTallGrass;

    //With this void Awake, we set the Animator so it plays the animation of the direction the player asked
    private void Awake()
    {
        character = GetComponent<Character>(); //Set the character object
    }
    private void Start()
    {
        myAudio = character.transform.GetChild(0).GetComponents<AudioSource>();
        background = myAudio[0];
        introTallGrass = myAudio[1];
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

            //While the player is not moving, we read the input, and move the player in the choosen direction
            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
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
        CheckForEncounter();
        CheckIfInTrainersView();
    }

    //Function to know if the player walk on a grass tile
    private void CheckForEncounter()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.1f, GameLayers.i.GrassLayer) != null) //Check if there is something in a radius, in the grass layer
        {
            if (UnityEngine.Random.Range(1, 101) <= 10) //If, within a range of 1 to 100, we hit below 10 (10% chances), we will encounter a creature
            {
                background.mute = true;
                Debug.Log("Mute music");
                introTallGrass.Play();
                character.Animator.IsMoving = false; //Set it to false when a battle appear
                OnEncountered();//We call our BattleSystem by changing the GameState to battle
            }
        }
    }
    private void CheckIfInTrainersView()
    {
        var collider = Physics2D.OverlapCircle(transform.position, 0.2f, GameLayers.i.FovLayer);
        if (collider != null) //Same as CheckForEncounter but with trainers
        {
            character.Animator.IsMoving = false; //Set it to false when a battle appear
            OnEnterTrainersView?.Invoke(collider); //Call the trainer battle when the player collide with the field of view
        }
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
}