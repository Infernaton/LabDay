using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] List<Sprite> walkDownSprites;
    [SerializeField] List<Sprite> walkUpSprites;
    [SerializeField] List<Sprite> walkRightSprites;
    [SerializeField] List<Sprite> walkLeftSprites;
    [SerializeField] FacingDirection defaultDirection = FacingDirection.Down;

    //Parameters
    public float MoveX { get; set; }
    public float MoveY { get; set; }
    public bool IsMoving { get; set; }

    //States like walkUp, WalkDown, etc
    SpriteAnimator walkDownAnim;
    SpriteAnimator walkUpAnim;
    SpriteAnimator walkRightAnim;
    SpriteAnimator walkLeftAnim;

    SpriteAnimator currentAnim; //Keep track of the current animation
    bool wasPrevMoving;

    //References
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //Initialize all animations
        walkDownAnim = new SpriteAnimator(walkDownSprites, spriteRenderer);
        walkUpAnim = new SpriteAnimator(walkUpSprites, spriteRenderer);
        walkRightAnim = new SpriteAnimator(walkRightSprites, spriteRenderer);
        walkLeftAnim = new SpriteAnimator(walkLeftSprites, spriteRenderer);
        SetFacingDirection(defaultDirection);

        currentAnim = walkDownAnim; //Initialy, this will be our walkDownAnim
    }

    private void Update()
    {
        var prevAnim = currentAnim;

        if (MoveX == 1)
            currentAnim = walkRightAnim;
        else if (MoveX == -1)
            currentAnim = walkLeftAnim;
        else if (MoveY == 1)
            currentAnim = walkUpAnim;
        else if (MoveY == -1)
            currentAnim = walkDownAnim;

        if (currentAnim != prevAnim || IsMoving != wasPrevMoving)
            currentAnim.Start();

        //If the player is moving, we show the whole animation, else we just show the first frame since she is just standing up
        if (IsMoving)
            currentAnim.HandleUpdate();
        else
            spriteRenderer.sprite = currentAnim.Frames[0];

        wasPrevMoving = IsMoving;
    }

    public void SetFacingDirection(FacingDirection dir) //Function to simply change the facing direction
    {
        switch (dir)
        {
            case FacingDirection.Right:
                MoveX = 1;
                break;
            case FacingDirection.Left:
                MoveX = -1;
                break;
            case FacingDirection.Up:
                MoveY = 1;
                break;
            case FacingDirection.Down:
                MoveY = -1;
                break;
        }
    }
    public FacingDirection DefaultDirection //Property to use the facing direction in the trainer script
    { get => defaultDirection; }
}

public enum FacingDirection { Up, Down, Left, Right} //List the four directions