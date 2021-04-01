using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongGrass : MonoBehaviour, IPlayerTriggerable
{
    public void OnPlayerTriggered(PlayerController playerController)
    {
        if (Random.Range(1, 101) <= 10) //If, within a range of 1 to 100, we hit below 10 (10% chances), we will encounter a creature
        {
            playerController.StopMusic(playerController.MusicBackground);
            playerController.PlayMusic(playerController.IntroTallGrass);
            GameController.Instance.StartBattle();//We call our BattleSystem by changing the GameState to battle
        }
    }
}
