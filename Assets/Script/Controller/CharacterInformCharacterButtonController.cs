using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInformCharacterButtonController : CharacterButton
{
    public override void ClickCharacterButton()
    { 
        GameManager.instance.mainManager.SetSelectCharacter(playerCharacterIndex);
    }
}
