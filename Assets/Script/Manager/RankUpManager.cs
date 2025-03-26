using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankUpManager : CharacterInform
{
    [SerializeField]
    private GameObject rankUpText;
    [SerializeField]
    private GameObject rankUpCostText;

    private PlayerCharacter rankUpCharacter;
    private int rankUpCost;
    private int selectCharacterCount;
    private int rankUpNeedCharacter;

    public override void ClearInform()
    {
        rankUpText.GetComponent<Text>().text = "";
        rankUpCostText.GetComponent<Text>().text = "";
        rankUpCharacter = null;
        rankUpCost = 0;
        selectCharacterCount = 0;
        rankUpNeedCharacter = 0;
        mainManager.SetFalseCharacterInformCard();
        mainManager.UnselectCharacterInformSelectInventoryContent();
    }

    public override void SelectCharacter(int num)
    {
        if (rankUpCharacter == null)
        {
            if ((int)(GameManager.instance.playerCharacter[num].status.characterRank) < (System.Enum.GetValues(typeof(CharacterRank)).Length) - 1)
            {
                mainManager.SetCharacterInformCard(num);
                rankUpCharacter = GameManager.instance.playerCharacter[num];
                mainManager.SetCharacterInformButtonActiveFalse(num);
                mainManager.SetSameCharacterInformButton(rankUpCharacter.status);
                rankUpCost = GameManager.instance.GetRankUpCost(GameManager.instance.playerCharacter[num].status.characterRank);
                rankUpNeedCharacter = GameManager.instance.GetRankUpNeedCharacter(GameManager.instance.playerCharacter[num].status.characterRank);
                UpdateRankUpInform();
            
            }
            else
            { 
                Debug.Log("�ִ� ��ũ�Դϴ�!");    
            }
            return;
        }

        if((rankUpCharacter.status.characterIndex == GameManager.instance.playerCharacter[num].status.characterIndex) && (rankUpCharacter.status.characterRank == GameManager.instance.playerCharacter[num].status.characterRank))
        {
            if (selectCharacterCount < rankUpNeedCharacter)
            {
                mainManager.SetCharacterInventoryButton(num, true);
                selectCharacterCount++;
                UpdateRankUpInform();
            }
            else
            {
                Debug.Log("���õ� ĳ���Ͱ� �ִ�ġ �Դϴ�!");
            }
            
        }  
        else
        {
            Debug.LogWarning("�ٸ� ĳ���Ͱ� ���Խ��ϴ�!");
        }
    }

    public override void UnselectCharacter(int num)
    {
        mainManager.SetCharacterInventoryButton(num, false);
        selectCharacterCount--;
        UpdateRankUpInform();
    }

    public override void CharacterInformButton()
    {
        if(rankUpCharacter != null)
        { 
            if(selectCharacterCount >= rankUpNeedCharacter)
            { 
                if (GameManager.instance.gameGold >= rankUpCost)
                {
                    VoiceDatabaseManager.instance.PlayRandomVoice(rankUpCharacter.status.voiceType);          
                    GameManager.instance.gameGold = GameManager.instance.gameGold - rankUpCost;
                    GameManager.instance.CharacterRankUp(rankUpCharacter.GetPlayerCharacterIndex());
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
                Debug.Log("��ϵ� ĳ���Ͱ� �����մϴ�!");
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

    public void UpdateRankUpInform()
    {
        if ((int)(rankUpCharacter.status.characterRank) >= (System.Enum.GetValues(typeof(CharacterRank)).Length) - 1)
        {
            Debug.LogWarning("�߸��� ĳ���Ͱ� ���� �ֽ��ϴ�!");
            ClearInform();
            return;
        }

        rankUpText.GetComponent<Text>().text = "Rank\n" + rankUpCharacter.status.characterRank.ToString() + " �� " + (rankUpCharacter.status.characterRank + 1).ToString() + "\n(" + selectCharacterCount + " / " + rankUpNeedCharacter +")";
        rankUpCostText.GetComponent<Text>().text = "��� : -" + rankUpCost + "���";
    }
}
