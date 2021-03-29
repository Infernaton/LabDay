using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; //We use this library bc we actually work with UI

public class HPBar : MonoBehaviour
{
    [SerializeField] GameObject health; //We could modify our GameObject health right in Unity
    [SerializeField] Text hpNumber;

    public void SetHP(float hpNormalized) //Method to set our Hp in real time while in a battle
    {
        health.transform.localScale = new Vector3(hpNormalized, 1f); //Transform our health bar at the scale of the float of our Hp
    }

    public IEnumerator SetHPSmooth(int updateHp, int maxHp) //HPBar will now reduce smootlhy
    {

        float newHp = (float) updateHp / maxHp;
        float curHp = health.transform.localScale.x; //The current Hp are now set as an x line.
        float changeAmt = curHp - newHp; //Get the amount of hp to reduce

        //This loop will move the life bar by a certain amount within a certain given time
        while (curHp - newHp > Mathf.Epsilon)
        {
            
            SetHPnumber(updateHp, maxHp);
            curHp -= changeAmt * Time.deltaTime;
            health.transform.localScale = new Vector3(curHp, 1f);
            yield return null; //This return is made to get out of the loop
        }
        health.transform.localScale = new Vector3(newHp, 1f); //We set it equals to the new Hp anyway, in case it appears to look bad
    }

    public void SetHPnumber(int curHp, int MaxHp) => hpNumber.text = $"{curHp} / {MaxHp}";
}