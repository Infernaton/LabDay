using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script will reference our layers
public class GameLayers : MonoBehaviour
{
    [SerializeField] LayerMask solidObjectsLayer;   //Reference our SolidObjects layer
    [SerializeField] LayerMask interactableLayer;   //Reference our Interactable layer
    [SerializeField] LayerMask grassLayer;          //Reference our LongGrass layer
    [SerializeField] LayerMask playerLayer;         //Reference our player layer
    [SerializeField] LayerMask fovLayer;            //Reference our Fov layer
    [SerializeField] LayerMask changeScene;         //Layer that permit to change our scene

    public static GameLayers i { get; set; }
    private void Awake()
    {
        i = this;
    }

    public LayerMask SolidLayer
    {
        get => solidObjectsLayer;
    }
    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }
    public LayerMask GrassLayer
    {
        get => grassLayer;
    }
    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }
    public LayerMask FovLayer
    {
        get => fovLayer;
    }
    public LayerMask ChangeScene
    {
        get => changeScene;
    }
}