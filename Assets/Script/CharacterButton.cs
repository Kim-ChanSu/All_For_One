using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CharacterButton : MonoBehaviour
{
    [SerializeField]
    protected GameObject characterFace;
    [SerializeField]
    protected GameObject characterRankText;
    [SerializeField]
    protected GameObject characterUpgradeText;

    protected int playerCharacterIndex;

    public void SetCharacterButton(PlayerCharacter character)
    { 
        characterFace.GetComponent<Image>().sprite = character.status.face;
        characterRankText.GetComponent<Text>().text = character.status.characterRank + "";
        characterUpgradeText.GetComponent<Text>().text = "+" + character.status.characterUpgrade;
        playerCharacterIndex = character.GetPlayerCharacterIndex();
    }

    public abstract void ClickCharacterButton();
}
