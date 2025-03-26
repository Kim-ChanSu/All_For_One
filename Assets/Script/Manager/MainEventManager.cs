using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEventManager : MonoBehaviour
{
    private MainManager mainManager;

    private void Awake()
    { 
        InitMainEventManager();
    }

    private void InitMainEventManager()
    { 
        if(this.gameObject.GetComponent<MainManager>() == true)
        { 
            mainManager = this.gameObject.GetComponent<MainManager>();
        }
        else
        { 
            Debug.LogWarning("게임 오브젝트가 MainManager 들고 있지 않습니다!");    
        }        
    }

    public void EventCheck()
    { 
        switch(GameManager.instance.gameProgress)
        { 
            case 0:
                FirstEvent();
                break;
            default:
                break; 
        }         
    }        
    
    private void FirstEvent()
    { 
        mainManager.StartGetCharacterCard(CardPackDatabaseManager.instance.GetCardPack(0));
        GameManager.instance.gameProgress++;
    }
}
