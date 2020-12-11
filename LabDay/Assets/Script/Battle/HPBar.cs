using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health; //We could modify our GameObject health right in Unity

    public void SetHP(float hpNormalized) //Method to set our Hp in real time while in a battle
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f); //Transform our health bar at the scale of the float of our Hp
    }
}