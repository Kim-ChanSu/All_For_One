using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformSelectInventoryButtonController : CharacterButton
{

    public override void ClickCharacterButton()
    { 
        GameManager.instance.mainManager.UnselectCharacter(playerCharacterIndex);
    }
}
