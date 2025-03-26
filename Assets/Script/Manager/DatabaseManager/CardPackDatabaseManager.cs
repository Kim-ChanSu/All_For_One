using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class CardPack
{ 
    #region
    public string cardPackName;
    public Sprite cardPackImage;
    public int cardPackPrice;
    public int cardPackNeedGameProgress;
    public CharacterIP cardpackType;

    public List<CardPackLineUp> cardPackLineUp;
    #endregion
}

[System.Serializable]
public struct CardPackLineUp
{ 
    public string name;
    public Character character;
    public int getChance;
}

public struct GetChanceCard
{ 
    public int minNum;
    public int maxNum;
    public Character character;
}

public class CardPackDatabaseManager : MonoBehaviour
{
    [SerializeField]
    private CardPack[] cardPack;
    public static CardPackDatabaseManager instance;

    private void Start() 
    {
        InitCardPackDatabaseManager();
    }

    private void InitCardPackDatabaseManager()
    { 
        #region
        // �̱���
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� CardPackDatabaseManager�� 2���̻� �����մϴ�.");
            Destroy(this.gameObject);
        }

        SetCardPack();
        #endregion
    }

    //ī���� �ڵ�ȭ��
    #region
    private void SetCardPack()
    { 
        for(int i = 0; i < cardPack.Length; i++)
        { 
            switch(cardPack[i].cardpackType)
            { 
                case CharacterIP.Ignore:
                    break;
                case CharacterIP.All:
                    SetAllCharacterCardPack(i);
                    break;
                default:
                    SetIPCharacterCardPack(i, cardPack[i].cardpackType);
                    break;                    
            }
        }
    }

    private void SetAllCharacterCardPack(int cardPackNum)
    { 
        if((cardPackNum < cardPack.Length) && (0 <= cardPackNum))
        {
            cardPack[cardPackNum].cardPackLineUp = new List<CardPackLineUp>();
            for(int i = 0; i < CharacterDatabaseManager.instance.GetPlayerCharacterLength(); i++)
            { 
                CardPackLineUp newCardPackLineUp = new CardPackLineUp();
                newCardPackLineUp.name = CharacterDatabaseManager.instance.GetPlayerCharacter(i).status.name;
                newCardPackLineUp.character = CharacterDatabaseManager.instance.GetPlayerCharacter(i);
                newCardPackLineUp.getChance = 1;

                cardPack[cardPackNum].cardPackLineUp.Add(newCardPackLineUp);
            }
        }
        else
        {
            Debug.LogWarning("SetAllCharacterCardPack�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + cardPackNum);
        }               
    }

    private void SetIPCharacterCardPack(int cardPackNum, CharacterIP ip)
    { 
        if((cardPackNum < cardPack.Length) && (0 <= cardPackNum))
        {
            cardPack[cardPackNum].cardPackLineUp = new List<CardPackLineUp>();
            for(int i = 0; i < CharacterDatabaseManager.instance.GetPlayerCharacterLength(); i++)
            { 
                if(ip == CharacterDatabaseManager.instance.GetPlayerCharacter(i).status.characterIP)
                { 
                    CardPackLineUp newCardPackLineUp = new CardPackLineUp();
                    newCardPackLineUp.name = CharacterDatabaseManager.instance.GetPlayerCharacter(i).status.name;
                    newCardPackLineUp.character = CharacterDatabaseManager.instance.GetPlayerCharacter(i);
                    newCardPackLineUp.getChance = 1;

                    cardPack[cardPackNum].cardPackLineUp.Add(newCardPackLineUp);
                }
            }
        }
        else
        {
            Debug.LogWarning("SetIPCharacterCardPack�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + cardPackNum);
        }               
    }
    #endregion

    public CardPack GetCardPack(int num)
    { 
        if((num < cardPack.Length) && (0 <= num))
        {
            return cardPack[num];
        }
        else
        {
            Debug.LogWarning("GetCardPack�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + num);
            return cardPack[0];
        }
    }

    public int GetCardPackLength()
    {
        return cardPack.Length;
    }

}
