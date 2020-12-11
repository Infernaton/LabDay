using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;

    public void SetData(Pokemon pokemon) //function to set the value of our two texts and HPBar
    {
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp );
    }
}
