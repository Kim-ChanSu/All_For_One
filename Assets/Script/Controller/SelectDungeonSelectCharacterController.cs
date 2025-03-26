using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDungeonSelectCharacterController : CharacterButton
{
    public override void ClickCharacterButton()
    {
        GameManager.instance.mainManager.SelectDungeonPlayerCharacter(playerCharacterIndex);
    }
}
