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
            Debug.LogWarning("메인 매니저가 없습니다!");
            Destroy(this.gameObject);
            return;
        }
        cardPack = newCardPack;
        mainManager = newMainManager;

        packNameText.GetComponent<Text>().text = cardPack.cardPackName;
        packCostText.GetComponent<Text>().text = cardPack.cardPackPrice + "골드";
        cardPackImage.GetComponent<Image>().sprite = cardPack.cardPackImage;
    }

    public void SetBuyCardPack()
    { 
        if(mainManager == null)
        { 
            Debug.LogWarning("메인 매니저가 없습니다!");
            Destroy(this.gameObject);
            return;
        }        

        mainManager.SetBuyCardPack(cardPack);
    }
}
