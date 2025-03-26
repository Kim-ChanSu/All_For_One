using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterCardController : MonoBehaviour
{
    #region
    [SerializeField]
    private GameObject logo;
    [SerializeField]
    private GameObject face;
    [SerializeField]
    private GameObject cardNameText;
    [SerializeField]
    private GameObject HPText;
    [SerializeField]
    private GameObject MPText;
    [SerializeField]
    private GameObject ATKText;
    [SerializeField]
    private GameObject MAKText;
    [SerializeField]
    private GameObject DEFText;
    [SerializeField]
    private GameObject MDFText;
    [SerializeField]
    private GameObject INTText;
    [SerializeField]
    private GameObject STAText;
    [SerializeField]
    private GameObject cardRankText;
    [SerializeField]
    private GameObject cardUpgradeText;
    [SerializeField]
    private GameObject cardBlackMask;
    #endregion
    private int cardDataNum;

    public void SetCardInformation(CharacterStatus status)
    { 
        logo.GetComponent<Image>().sprite = IPLogoDatabaseManager.instance.GetCharacterIPLogo(status.characterIP);
        face.GetComponent<Image>().sprite = status.face;
        cardNameText.GetComponent<Text>().text = "< " + status.name + " >";
        HPText.GetComponent<Text>().text = status.maxHP + "";
        MPText.GetComponent<Text>().text = status.maxMP + "";
        ATKText.GetComponent<Text>().text = status.ATK + "";
        MAKText.GetComponent<Text>().text = status.MAK + "";
        DEFText.GetComponent<Text>().text = status.DEF + "";
        MDFText.GetComponent<Text>().text = status.MDF + "";
        INTText.GetComponent<Text>().text = status.INT + "";
        STAText.GetComponent<Text>().text = status.STA + "";
        cardRankText.GetComponent<Text>().text = status.characterRank + "";
        cardUpgradeText.GetComponent<Text>().text = "+" + status.characterUpgrade;
        cardDataNum = status.characterIndex;
    }

    public void SetCardMask(bool mode)
    { 
        cardBlackMask.SetActive(mode);
    }
}
