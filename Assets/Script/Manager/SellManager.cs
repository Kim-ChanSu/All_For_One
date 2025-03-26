using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SellManager : CharacterInform
{
    private int selectCharacterCount;
    private int sellGetGold;

    [SerializeField]
    private GameObject sellText;
    [SerializeField]
    private GameObject sellGetGoldText;

    public override void ClearInform()
    {
        sellText.GetComponent<Text>().text = "";
        sellGetGoldText.GetComponent<Text>().text = "";
        sellGetGold = 0;
        selectCharacterCount = 0;
        mainManager.SetFalseCharacterInformCard();
        mainManager.UnselectCharacterInformSelectInventoryContent();   
    }

    public override void SelectCharacter(int num)
    {
        if (selectCharacterCount < GameManager.instance.playerCharacter.Count - 1)
        {
            mainManager.SetCharacterInformCard(num);
            mainManager.SetCharacterInventoryButton(num, true);
            selectCharacterCount++;
            sellGetGold = sellGetGold + GameManager.instance.GetSellGold(GameManager.instance.playerCharacter[num]);
            UpdateSellInform();
        }
        else
        { 
            Debug.Log("캐릭터는 1개이상 남겨야 합니다!");    
        }
    }

    public override void UnselectCharacter(int num)
    {
        mainManager.SetCharacterInformCard(num);
        mainManager.SetCharacterInventoryButton(num, false);
        selectCharacterCount--;
        sellGetGold = sellGetGold - GameManager.instance.GetSellGold(GameManager.instance.playerCharacter[num]);
        UpdateSellInform();
    }

    public override void CharacterInformButton()
    {   
        GameManager.instance.gameGold = GameManager.instance.gameGold + sellGetGold;
        mainManager.DeleteSelectCharacter();
        ClearInform();
    }

    public override void CancleCharacterInformButton()
    {
        ClearInform();
    }

    private void UpdateSellInform()
    {
        sellText.GetComponent<Text>().text = selectCharacterCount + "명";
        sellGetGoldText.GetComponent<Text>().text = "골드 : +" + sellGetGold;
    }
}
