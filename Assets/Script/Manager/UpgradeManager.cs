using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : CharacterInform
{
    private PlayerCharacter UpgradeCharacter;
    private int upgradeCost;
    private int upgradeEXP;
    [SerializeField]
    private GameObject EXPImage;
    [SerializeField]
    private GameObject EXPText;
    [SerializeField]
    private GameObject upgradeText;
    [SerializeField]
    private GameObject upgradeCostText;

    public override void ClearInform()
    {
        UpgradeCharacter = null;
        upgradeEXP = 0;
        upgradeCost = 0;
        mainManager.SetFalseCharacterInformCard();
        mainManager.UnselectCharacterInformSelectInventoryContent();
        upgradeText.GetComponent<Text>().text = "";
        upgradeCostText.GetComponent<Text>().text = "";
        EXPText.GetComponent<Text>().text = "";
        EXPImage.GetComponent<Image>().fillAmount = 0.0f;     
    }

    public override void SelectCharacter(int num)
    {
        if (UpgradeCharacter == null)
        {
            mainManager.SetCharacterInformCard(num);
            UpgradeCharacter = GameManager.instance.playerCharacter[num];
            mainManager.SetCharacterInformButtonActiveFalse(num);
            UpdateUpgradeInform();
            return;
        }
        mainManager.SetCharacterInventoryButton(num, true);
        upgradeEXP = upgradeEXP + GameManager.instance.GetUpgradeEXP(GameManager.instance.playerCharacter[num]);
        upgradeCost = GameManager.instance.GetUpgradeCost(upgradeEXP);
        UpdateUpgradeInform();
    }

    public override void UnselectCharacter(int num)
    {
        mainManager.SetCharacterInventoryButton(num, false);
        upgradeEXP = upgradeEXP - GameManager.instance.GetUpgradeEXP(GameManager.instance.playerCharacter[num]);
        upgradeCost = GameManager.instance.GetUpgradeCost(upgradeEXP);

        UpdateUpgradeInform();
    }

    public override void CharacterInformButton()
    {
        if(UpgradeCharacter != null)
        { 
            if (GameManager.instance.gameGold >= upgradeCost)
            {
                VoiceDatabaseManager.instance.PlayRandomVoice(UpgradeCharacter.status.voiceType);          
                GameManager.instance.gameGold = GameManager.instance.gameGold - upgradeCost;
                GameManager.instance.CharacterUpgradeEXPUp(UpgradeCharacter.GetPlayerCharacterIndex(), upgradeEXP);
                mainManager.DeleteSelectCharacter();
                ClearInform();
            }
            else
            {
                Debug.Log("���� ���� �մϴ�!");
            }
        }
        else
        { 
            Debug.Log("ĳ���Ͱ� �����Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    public override void CancleCharacterInformButton()
    {
        ClearInform();
    }

    private void UpdateUpgradeInform()
    {
        if (UpgradeCharacter == null)
        {
            ClearInform();
            return;
        }

        SetUpgradeText();
        upgradeCostText.GetComponent<Text>().text = "��� : -" + upgradeCost + "���";
        EXPText.GetComponent<Text>().text = UpgradeCharacter.status.upgradeEXP + upgradeEXP + " / " + UpgradeCharacter.status.upgradeMaxEXP;
        EXPImage.GetComponent<Image>().fillAmount = (float)(UpgradeCharacter.status.upgradeEXP + upgradeEXP) / (float)UpgradeCharacter.status.upgradeMaxEXP;
    }

    private void SetUpgradeText()
    {
        int beforeUpgrade = UpgradeCharacter.status.characterUpgrade;
        int afterUpgrade = beforeUpgrade;
        int checkEXP = UpgradeCharacter.status.upgradeEXP + upgradeEXP;
        int maxCheckEXP = UpgradeCharacter.status.upgradeMaxEXP;

        while (checkEXP >= maxCheckEXP)
        {
            checkEXP = checkEXP - maxCheckEXP;
            maxCheckEXP = GameManager.instance.UpgradeMaxEXPUp(maxCheckEXP);
            afterUpgrade++;
        }

        upgradeText.GetComponent<Text>().text = "+" + beforeUpgrade + "\n��\n" + "+" + afterUpgrade;
    }
}
