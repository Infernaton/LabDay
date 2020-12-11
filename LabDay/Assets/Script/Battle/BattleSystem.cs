using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    //We want to setup everything at the very first frame of the battle
    private void Start()
    {
        SetupBattle(); //We call our SetupBattle function
    }

    public void SetupBattle() //We use the data created in the BattleUnit and BattleHud scripts
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
    }
}
