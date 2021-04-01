using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IMenuController
{
    void NotVisible();

    void HandleChoiceSelection(Action<int> onSelected); //Same logic as every other Handle***

    void UpdateMenuUISelection(int selection);
}
