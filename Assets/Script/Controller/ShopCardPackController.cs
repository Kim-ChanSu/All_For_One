using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopCardPackController : MonoBehaviour
{
    private CardPack cardPack;
    private MainManager mainManager;
    [SerializeField]
    private GameObject packNameText;
    [SerializeField]
    private GameObject packCostText;
    [SerializeField]
    private GameObject cardPackImage;


    public void SetShopCardPack(CardPack newCardPack, MainManager newMainManager)
    { 
        if(newMainManager == null)
        { 
            Debug.LogWarning("���� �Ŵ����� �����ϴ�!");
            Destroy(this.gameObject);
            return;
        }
        cardPack = newCardPack;
        mainManager = newMainManager;

        packNameText.GetComponent<Text>().text = cardPack.cardPackName;
        packCostText.GetComponent<Text>().text = cardPack.cardPackPrice + "���";
        cardPackImage.GetComponent<Image>().sprite = cardPack.cardPackImage;
    }

    public void SetBuyCardPack()
    { 
        if(mainManager == null)
        { 
            Debug.LogWarning("���� �Ŵ����� �����ϴ�!");
            Destroy(this.gameObject);
            return;
        }        

        mainManager.SetBuyCardPack(cardPack);
    }
}
