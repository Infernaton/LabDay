using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI
using DG.Tweening;//Importing DOTween, an animation engine 

//This is were we manage our pokemon in the battle
public class BattleUnit : MonoBehaviour
{
    [SerializeField] bool isPlayerUnit;
    [SerializeField] BattleHud hud;

    public bool IsPlayerUnit //Property to expose if the unit is actually the player's one or the enemy one
    {
        get { return isPlayerUnit; }
    }
    public BattleHud Hud //Property to expose if the hud is the player's one or enemy one
    {
        get { return hud; }
    }
    public Pokemon Pokemon { get; set; }

    Image image; //Reference to our image, so we can just call image. instead of GetComponent<Image> everytime
    Vector3 originalPos; //Reference to the original position of the image
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition; //localPosition is to get the position relative to the canvas
        originalColor = image.color;
    }

    //We will know wich pokemon we should choose to show, and if we need it's back or front sprite
    public void Setup(Pokemon pokemon) //Parameter is a Pokemon pokemon function, to know it's base and level
    {
        Pokemon = pokemon;
        if (isPlayerUnit)
        {
            image.sprite = Pokemon.Base.BackSprite;
        }
        else
        {
            image.sprite = Pokemon.Base.FrontSprite;
        }

        hud.SetData(pokemon);

        image.color = originalColor;
        PlayEnterAnimation();
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
        {
            image.transform.localPosition = new Vector3(-700f, originalPos.y); //Move the player unit image by -500 on x axis
        }
        else
            image.transform.localPosition = new Vector3(700f, originalPos.y); //If it's the enemy unit, +500 on x axis

        image.transform.DOLocalMoveX(originalPos.x, 1f);//DOLocalMoveX is used by DOTween engine, to move our sprite back to its originalPos (only x axis), in 1 second
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence(); //DOTween give us the possibilty to use a sequence, to play multiple animations
        if (isPlayerUnit)
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 50, 0.15f)); //FIRST We put this animation in a sequence, and it'll play, then play the rest of the sequence
        }
        else
        {
            sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 50, 0.15f));
        }

        sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.33f)); //THEN it will play the animation in "reverse"
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence(); //Creating a sequence to change the color of the pokemons, so it will look like a double red/gray blink
        sequence.Append(image.DOColor(Color.gray, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence(); //Creating a sequence to play two animations one after another
        sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f)); //Taking the sprite down first
        sequence.Join(image.DOFade(0f, 0.5f)); //Join is for this animation playing WHILE the other one is playing (set the Alpha to 0, in 0.5 seconds)
    }
}
