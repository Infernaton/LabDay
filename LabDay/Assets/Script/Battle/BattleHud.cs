using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI

public class BattleHud : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;

    //Set all the text colors we'll use
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;

    Pokemon _pokemon; //reference to our PokemonClass (_pokemon) is bc the name pokemon is already taken

    //Store the colors in a dictionnary
    Dictionary<ConditionID, Color> statusColors;

    public void SetData(Pokemon pokemon) //function to set the value of our two texts and HPBar
    {
        _pokemon = pokemon;

        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float) pokemon.HP / pokemon.MaxHp );

        statusColors = new Dictionary<ConditionID, Color>() //Initialize the dictionnary
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor }
        };

        SetStatusText();
        _pokemon.OnStatusChanged += SetStatusText;//Link the SetStatuxText with the OnStatusChanged, to know when we should change the status
    }

    public void SetStatusText() //Function to set the status text (psn, frz, slp..)
    {
        if (_pokemon.Status == null)
            statusText.text = "";
        else
        {
            statusText.text = _pokemon.Status.Id.ToString().ToUpper(); //call the ID of the status, to know what we should print in the StatusText, and convert it to string
            statusText.color = statusColors[_pokemon.Status.Id]; //Set the color of the text from our dictionnary
        }
    }

    //We use coroutine here to set the Hp after a hit was taken
    public IEnumerator UpdateHP()
    {
        if (_pokemon.HpChanged)
        {
            yield return hpBar.SetHPSmooth((float)_pokemon.HP / _pokemon.MaxHp);
            _pokemon.HpChanged = false;
        }
    }
}
