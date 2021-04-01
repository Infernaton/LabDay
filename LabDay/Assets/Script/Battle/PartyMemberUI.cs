using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberUI : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] HPBar hpBar;
    [SerializeField] Image frontSprite;
    [SerializeField] Color highlightedColor;

    public void SetData(Pokemon pokemon) //function to set the value of our two texts and HPBar
    {
        nameText.text = pokemon.Base.Name;
        levelText.text = "Lvl " + pokemon.Level;
        hpBar.SetHP((float)pokemon.HP / pokemon.MaxHp);
        if (pokemon.Base.FrontSprite != null)
            frontSprite.sprite = pokemon.Base.FrontSprite;
        else frontSprite.sprite = pokemon.Base.BackSprite;
    }

    public void SetSelected(bool selected) //Function to highlight and know wich pokemon is actually selected
    {
        if (selected)
        {
            nameText.color = highlightedColor;
            if (nameText.text[0] != '>')
                nameText.text = "> " + nameText.text;
        }
        else
        {
            nameText.color = Color.black;
            if (nameText.text[0] == '>')
                nameText.text = nameText.text.Substring(2);
        }
    }
}