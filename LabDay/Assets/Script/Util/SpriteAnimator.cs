using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//We create a custom animation script to easily create new animations
public class SpriteAnimator
{
    SpriteRenderer spriteRenderer; //Reference the sprite renderer
    List<Sprite> frames; //List of sprites for every frames we'll have in one animation
    float frameRate; //Speed of the animation

    int currentFrame; //Keep track of the frames
    float timer; //Keep track of the time

    public SpriteAnimator(List<Sprite> frames, SpriteRenderer spriteRenderer, float frameRate=0.16f) //This framerate equals 60 fps
    {
        this.frames = frames; //initialize our variables
        this.spriteRenderer = spriteRenderer;
        this.frameRate = frameRate;
    }

    public void Start()
    {
        currentFrame = 0;
        timer = 0f;
        spriteRenderer.sprite = frames[0];
    }
    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if (timer > frameRate)
        {
            currentFrame = (currentFrame + 1) % frames.Count;
            spriteRenderer.sprite = frames[currentFrame];
            timer -= frameRate;
        }
    }
}
                            