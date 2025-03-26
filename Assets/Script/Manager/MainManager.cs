using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip mainBGM;
    [SerializeField]
    private GameObject mainButtons;
    private MainEventManager mainEventManager;

    //던전용
    [SerializeField]
    private GameObject selectDungeonCanvas;
    [SerializeField]
    private GameObject selectDungeonWindow;
    [SerializeField]
    private GameObject selectDungeonText;
    [SerializeField]
    private GameObject dungeonClearGoldText;
    [SerializeField]
    private GameObject dungeonLineUpContent;
    [SerializeField]
    private GameObject selectDungeonPrefab;
    [SerializeField]
    private GameObject selectCharacterWindow;
    [SerializeField]
    private GameObject selectDungeonNameText;
    [SerializeField]
    private GameObject dungeonClearGoldText_SC;
    [SerializeField]
    private GameObject dungeonLengthText;
    [SerializeField]
    private GameObject selectCharacterInventoryContent;
    [SerializeField]
    private GameObject dungeonSelectCharacterCard;
    [SerializeField]
    private GameObject dungeonExplainText;
    [SerializeField]
    private GameObject selectDungeonSelectCharacterPrefab;

    //뽑기용
    [SerializeField]
    private GameObject getCardCanvas;
    [SerializeField]
    private GameObject card;
    [SerializeField]
    private GameObject cardPackImage;
    private CardPack cardPack;

    //상점용
    [SerializeField]
    private GameObject shopCanvas;
    [SerializeField]
    private GameObject cardPackLineUpContent;
    [SerializeField]
    private GameObject selectCardPackNameText;
    [SerializeField]
    private GameObject shopGoldText;
    [SerializeField]
    private GameObject buyCardPackButton;
    [SerializeField]
    private GameObject shopCardPackPrefab;

    private CardPack buyCardPack;

    public enum GetCharacterCardPhase
    {
        None,
        CardPackStay,
        OpenCardPack,
        Animation    
    }

    private GetCharacterCardPhase getCharacterCardPhase = GetCharacterCardPhase.None;
    //---
    //캐릭터 정보용
    [SerializeField]
    private GameObject characterInformCanvas;
    [SerializeField]
    private GameObject characterInformText;
    [SerializeField]
    private GameObject characterInformButtonText;
    [SerializeField]
    private GameObject characterInformGoldText;
    [SerializeField]
    private GameObject characterInformCharacterInventoryContent;
    [SerializeField]
    private GameObject characterInformCard;
    [SerializeField]
    private GameObject characterInformSelectInventoryContent;
    [SerializeField]
    private GameObject upgradeWindow;
    [SerializeField]
    private GameObject rankUpWindow;
    [SerializeField]
    private GameObject sellWindow;
    [SerializeField]
    private GameObject characterInformCharacterButtonPrefab;
    [SerializeField]
    private GameObject characterInformSelectInventoryButton;

    public enum CharacterInformMode
    {
        None,
        Upgrade,
        RankUp,
        Sell
    }

    private CharacterInformMode characterInformMode = CharacterInformMode.None;
    private GameObject characterInformObject;
    //---
    //메뉴용
    [SerializeField]
    private GameObject menuCanvas;    
    //--
    private bool isSelectDungeon = false;
    private bool isGetCharacterCard = false;
    private bool isShop = false;
    private bool isCharacterInform = false;
    private bool isMenu = false;
    

    private void Start()
    { 
        InitMainManager();
    }

    private void Update()
    { 
        CheckGetCharacterStat();
    }

    private void InitMainManager()
    { 
        cardPack = null;
        buyCardPack = null;

        GameManager.instance.mainManager = this; 
        GameManager.instance.ChangeBGM(mainBGM);

        upgradeWindow.GetComponent<CharacterInform>().SetMainManager(this);
        rankUpWindow.GetComponent<CharacterInform>().SetMainManager(this);
        sellWindow.GetComponent<CharacterInform>().SetMainManager(this);

        if(this.gameObject.GetComponent<MainEventManager>() == true)
        { 
            mainEventManager = this.gameObject.GetComponent<MainEventManager>();
            mainEventManager.EventCheck();
        }
        else
        { 
            Debug.LogWarning("게임 오브젝트가 MainEventManager를 들고 있지 않습니다!");
        }
    }

    private void SetMainButton(bool mode)
    { 
        mainButtons.SetActive(mode);
    }

    public void ChangeCharacterInformModeUpgrade()
    {
        ChangeCharacterInformMode(CharacterInformMode.Upgrade);
    }

    public void ChangeCharacterInformModeRankUp()
    {
        ChangeCharacterInformMode(CharacterInformMode.RankUp);
    }

    public void ChangeCharacterInformModeSell()
    {
        ChangeCharacterInformMode(CharacterInformMode.Sell);
    }

    //던전선택용---
    #region
    public void SetSelectDungeonCanvas(bool mode)
    {
        selectDungeonCanvas.SetActive(mode);
        isSelectDungeon = mode;
        if (mode == true)
        {
            SetMainButton(false);
            selectDungeonWindow.SetActive(true);
            CreateSelectDungeonButton();
            SetSelectDungeonText();
            CreateSelectDungeonSelectCharacterButton();
            selectCharacterWindow.SetActive(false);
        }
        else
        { 
            SetMainButton(true);
            selectDungeonWindow.SetActive(false);   
            DeleteSelectDungeonButton();
            DeleteSelectDungeonSelectCharacterButton();
        }
    }

    private void CreateSelectDungeonButton()
    {
        #region
        for(int i = 0; i < DungeonDatabaseManager.instance.GetDungeonLength(); i++)
        { 
            if(DungeonDatabaseManager.instance.GetDungeon(i).dungeonNeedGameProgress <= GameManager.instance.gameProgress)
            { 
                GameObject newSelectDungeonPrefab = Instantiate(selectDungeonPrefab);
                newSelectDungeonPrefab.name = "selectDungeon" + i;
                newSelectDungeonPrefab.GetComponent<SelectDungeonController>().SetSelectDungeon(DungeonDatabaseManager.instance.GetDungeon(i), this);
                newSelectDungeonPrefab.transform.SetParent(dungeonLineUpContent.transform); 
                newSelectDungeonPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);                 
            }
        }
        #endregion
    }

    private void DeleteSelectDungeonButton()
    {
        #region
        for(int i = 0; i < dungeonLineUpContent.transform.childCount; i++)
        { 
             Destroy(dungeonLineUpContent.transform.GetChild(i).gameObject);
        }   
        #endregion
    }

    private void SetSelectDungeonText()
    {
        if (GameManager.instance.selectDungeon == null)
        {
            selectDungeonText.GetComponent<Text>().text = "";
            dungeonClearGoldText.GetComponent<Text>().text = "";
            return;
        }
        selectDungeonText.GetComponent<Text>().text = GameManager.instance.selectDungeon.dungeonName;
        dungeonClearGoldText.GetComponent<Text>().text = "보상 : "+ GameManager.instance.selectDungeon.dungeonClearGold + "골드";
    }

    public void SetSelectDungeon(Dungeon selectDungeon)
    {
        GameManager.instance.selectDungeon = selectDungeon;
        SetSelectDungeonText();
    }

    public void CheckSelectCharacter()
    {
        if(GameManager.instance.selectDungeon == null)
        {
            Debug.Log("던전이 선택되지 않았습니다!");
            return;
        }
        SetSelectCharacterWindow(true);
    }

    public void SetSelectCharacterWindow(bool mode)
    {
        ClearSelectCharacter();
        if((mode == true) && (GameManager.instance.selectDungeon == null))
        {
            Debug.Log("던전이 선택되지 않았습니다!");
            return;
        }
        selectCharacterWindow.SetActive(mode);
        if (mode == true)
        {
            selectDungeonWindow.SetActive(false);
            SetSelectCharacterSelectDungeonInform();
        }
        else
        {
            selectDungeonWindow.SetActive(true);
        }
    }

    private void CreateSelectDungeonSelectCharacterButton()
    {
        #region
        for (int i = 0; i < GameManager.instance.playerCharacter.Count; i++)
        {
            GameObject newSelectDungeonSelectCharacterPrefab = Instantiate(selectDungeonSelectCharacterPrefab);
            newSelectDungeonSelectCharacterPrefab.name = "Character" + i;
            newSelectDungeonSelectCharacterPrefab.GetComponent<CharacterButton>().SetCharacterButton(GameManager.instance.playerCharacter[i]);
            newSelectDungeonSelectCharacterPrefab.transform.SetParent(selectCharacterInventoryContent.transform); 
            newSelectDungeonSelectCharacterPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
        }
        #endregion
    }

    private void DeleteSelectDungeonSelectCharacterButton()
    {
        #region
        for(int i = 0; i < selectCharacterInventoryContent.transform.childCount; i++)
        { 
             Destroy(selectCharacterInventoryContent.transform.GetChild(i).gameObject);
        }   
        #endregion
    }

    private void ClearSelectCharacter()
    {
        GameManager.instance.selectCharacterNum = GameManager.RESETINDEX;
        selectDungeonNameText.GetComponent<Text>().text = "";
        dungeonClearGoldText_SC.GetComponent<Text>().text = "";
        dungeonLengthText.GetComponent<Text>().text = "";
        dungeonExplainText.GetComponent<Text>().text = "";
        dungeonSelectCharacterCard.SetActive(false);
    }

    private void SetSelectCharacterSelectDungeonInform()
    {
        if(GameManager.instance.selectDungeon == null)
        {
            Debug.LogError("던전이 선택되지 않았습니다!");
            return;
        }

        selectDungeonNameText.GetComponent<Text>().text = GameManager.instance.selectDungeon.dungeonName;
        dungeonClearGoldText_SC.GetComponent<Text>().text = "보상 : "+ GameManager.instance.selectDungeon.dungeonClearGold + "골드";
        dungeonLengthText.GetComponent<Text>().text = "던전길이 : " + GameManager.instance.selectDungeon.dungeonLength;
        dungeonExplainText.GetComponent<Text>().text = GameManager.instance.selectDungeon.dungeonExplain;
    }

    public void SelectDungeonPlayerCharacter(int num)
    {
        if (GameManager.instance.SetSelectCharacterNum(num) == true)
        {
            VoiceDatabaseManager.instance.PlayVoice(GameManager.instance.playerCharacter[num].status.voiceType, VoiceType.Select);
            SetDungeonSelectCharacterCard(num);
        }
        else
        {
            Debug.LogWarning("잘못된 값이 들어왔습니다!");
        }
    }

    public void SetDungeonSelectCharacterCard(int num)
    {
        dungeonSelectCharacterCard.SetActive(true);
        dungeonSelectCharacterCard.GetComponent<CharacterCardController>().SetCardInformation(GameManager.instance.playerCharacter[num].status);
    }

    public void StartDungeon()
    {
        if ((GameManager.instance.selectDungeon == null) || (GameManager.instance.selectCharacterNum == GameManager.RESETINDEX))
        {
            Debug.Log("캐릭터나 던전이 설정되어있지 않습니다!");
        }
        else
        {
            Debug.Log("던전시작!");
            GameManager.instance.ChangeScene(GameManager.DUNGEONSCENENAME);
        }
    }

    #endregion
    //---

    //뽑기용--
    #region
    private void CheckGetCharacterStat()
    { 
        if(isGetCharacterCard == true)
        { 
            if ((getCharacterCardPhase == GetCharacterCardPhase.CardPackStay) && (Input.anyKeyDown))
            {
                GetCharacter();
            }
            else if ((getCharacterCardPhase == GetCharacterCardPhase.OpenCardPack) && (Input.anyKeyDown))
            {
                EndGetCharacterCard();
            }
        }
    }

    public void StartGetCharacterCard(CardPack newCardPack)
    {
        if(newCardPack == null)
        { 
            Debug.LogWarning("카드팩이 없습니다!");
            return;
        }

        cardPack = newCardPack;
        GameManager.instance.StopBGM();
        SetGetCardCanvas(true);
        cardPackImage.GetComponent<Image>().sprite = newCardPack.cardPackImage;
        getCharacterCardPhase = GetCharacterCardPhase.CardPackStay;
    }

    private void GetCharacter()
    { 
        int minRange = 0;
        int maxRange = 0;
        
        GetChanceCard[] getChanceCard = new GetChanceCard[cardPack.cardPackLineUp.Count];
        if(getChanceCard.Length < 1)
        { 
            Debug.LogWarning("카드팩에 등록 된 카드가 없습니다!"); 
            EndGetCharacterCard();
            return;
        }

        for(int i = 0; i < cardPack.cardPackLineUp.Count; i++)
        { 
            getChanceCard[i].character = cardPack.cardPackLineUp[i].character;
            getChanceCard[i].minNum = maxRange;
            maxRange = maxRange + cardPack.cardPackLineUp[i].getChance;
            getChanceCard[i].maxNum = maxRange;
        }

        int getCardNum = Random.Range(minRange, maxRange);

        for (int i = 0; i < getChanceCard.Length; i++)
        {
            if ((getChanceCard[i].minNum <= getCardNum) && (getCardNum < getChanceCard[i].maxNum))
            {
                card.GetComponent<CharacterCardController>().SetCardInformation(getChanceCard[i].character.status);
                GameManager.instance.GetPlayerCharacter(getChanceCard[i].character);
                VoiceDatabaseManager.instance.PlayVoice(getChanceCard[i].character.status.voiceType, VoiceType.GetCharacter);
                GameManager.instance.PlaySE(IPLogoDatabaseManager.instance.GetCharacterIPSE(getChanceCard[i].character.status.characterIP));
                break;
            }
        } 
        cardPackImage.SetActive(false);
        getCharacterCardPhase = GetCharacterCardPhase.OpenCardPack;
    }

    private void EndGetCharacterCard()
    { 
        SetGetCardCanvas(false);
        GameManager.instance.StopVoice();
        GameManager.instance.StopSE();
        GameManager.instance.StartBGM();
        getCharacterCardPhase = GetCharacterCardPhase.None;
        cardPack = null;
    }

    public void SetGetCardCanvas(bool mode)
    {
        getCardCanvas.SetActive(mode);
        isGetCharacterCard = mode;
        cardPackImage.SetActive(true);
    }
    #endregion
    //--

    //상점용----
    #region
    public void SetShopCanvas(bool mode)
    {
        shopCanvas.SetActive(mode);
        isShop = mode;
        ClearCardPack();

        if(mode == true)
        { 
            SetMainButton(false);
            CreateShopCardPack();
            UpdateShopGoldText();
        }
        else
        { 
            SetMainButton(true);
            DeleteShopCardPack();
        }
    }

    private void CreateShopCardPack()
    { 
        #region
        for(int i = 0; i < CardPackDatabaseManager.instance.GetCardPackLength(); i++)
        { 
            if(CardPackDatabaseManager.instance.GetCardPack(i).cardPackNeedGameProgress <= GameManager.instance.gameProgress)
            { 
                GameObject newShopCardPackPrefab = Instantiate(shopCardPackPrefab);
                newShopCardPackPrefab.name = "shopCardPack" + i;
                newShopCardPackPrefab.GetComponent<ShopCardPackController>().SetShopCardPack(CardPackDatabaseManager.instance.GetCardPack(i), this);
                newShopCardPackPrefab.transform.SetParent(cardPackLineUpContent.transform); 
                newShopCardPackPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);                 
            }
        }
        #endregion
    }

    private void DeleteShopCardPack()
    { 
        for(int i = 0; i < cardPackLineUpContent.transform.childCount; i++)
        { 
             Destroy(cardPackLineUpContent.transform.GetChild(i).gameObject);
        }        
    }

    private void UpdateShopGoldText()
    { 
        shopGoldText.GetComponent<Text>().text = "골드: " + GameManager.instance.gameGold;
    }

    public void SetBuyCardPack(CardPack newCardPack)
    { 
        buyCardPack = newCardPack;
        selectCardPackNameText.GetComponent<Text>().text = buyCardPack.cardPackName;
        CheckCanBuyCardPack();
    }

    private void CheckCanBuyCardPack()
    {
        if (buyCardPack == null)
        {
            Debug.LogWarning("등록된 카드팩이 없습니다!");
            buyCardPackButton.SetActive(false);
            return;
        }

        if (GameManager.instance.gameGold >= buyCardPack.cardPackPrice)
        { 
            buyCardPackButton.SetActive(true);
        }
        else
        { 
            buyCardPackButton.SetActive(false);
        }
    }
    
    public void BuyCardPack()
    { 

        if (buyCardPack == null)
        {
            Debug.LogWarning("등록된 카드팩이 없습니다!");
            ClearCardPack();
            return;
        }

        if(GameManager.instance.gameGold >= buyCardPack.cardPackPrice)
        { 
            GameManager.instance.gameGold = GameManager.instance.gameGold - buyCardPack.cardPackPrice;
            UpdateShopGoldText();
            StartGetCharacterCard(buyCardPack);
            ClearCardPack();
        }
        else
        { 
            Debug.LogWarning("돈이 부족합니다!");
        }        
    }

    private void ClearCardPack()
    { 
        buyCardPack = null;
        selectCardPackNameText.GetComponent<Text>().text = "";
        buyCardPackButton.SetActive(false);       
    }
    #endregion
    //----

    //캐릭터 정보용
    #region
    public void SetCharacterInformCanvas(bool mode)
    { 
        characterInformCanvas.SetActive(mode);
        isCharacterInform = mode;
        ClearCharacterInform();

        if(mode == true)
        { 
            CreateCharacterInformButtons();
            ChangeCharacterInformMode(CharacterInformMode.Upgrade);
            SetMainButton(false);
        }
        else
        { 
            DeleteCharacterInformSelectInventoryContent();
            DeleteCharacterInformCharacterInventoryContent();
            ChangeCharacterInformMode(CharacterInformMode.None);
            SetMainButton(true);
        }        
    }

    private void CreateCharacterInformButtons()
    {
        #region
        for (int i = 0; i < GameManager.instance.playerCharacter.Count; i++)
        {
            GameObject newCharacterInformCharacterButtonPrefab = Instantiate(characterInformCharacterButtonPrefab);
            newCharacterInformCharacterButtonPrefab.name = "Character" + i;
            newCharacterInformCharacterButtonPrefab.GetComponent<CharacterInformCharacterButtonController>().SetCharacterButton(GameManager.instance.playerCharacter[i]);
            newCharacterInformCharacterButtonPrefab.transform.SetParent(characterInformCharacterInventoryContent.transform); 
            newCharacterInformCharacterButtonPrefab.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 

            GameObject newCharacterInformSelectInventoryButton = Instantiate(characterInformSelectInventoryButton);
            newCharacterInformSelectInventoryButton.name = "Select" + i;
            newCharacterInformSelectInventoryButton.GetComponent<CharacterInformSelectInventoryButtonController>().SetCharacterButton(GameManager.instance.playerCharacter[i]);
            newCharacterInformSelectInventoryButton.transform.SetParent(characterInformSelectInventoryContent.transform); 
            newCharacterInformSelectInventoryButton.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f); 
        }
        #endregion
    }

    private void DeleteCharacterInformSelectInventoryContent()
    {
        for (int i = 0; i < characterInformSelectInventoryContent.transform.childCount; i++)
        {
            Destroy(characterInformSelectInventoryContent.transform.GetChild(i).gameObject);
        }
    }

    private void DeleteCharacterInformCharacterInventoryContent()
    {
        for (int i = 0; i < characterInformCharacterInventoryContent.transform.childCount; i++)
        {
            Destroy(characterInformCharacterInventoryContent.transform.GetChild(i).gameObject);
        }
    }

    public void UnselectCharacterInformSelectInventoryContent()
    {
        for (int i = 0; i < characterInformSelectInventoryContent.transform.childCount; i++)
        {
            characterInformSelectInventoryContent.transform.GetChild(i).gameObject.SetActive(false);
            characterInformCharacterInventoryContent.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    private void ClearCharacterInform()
    { 
        upgradeWindow.SetActive(false);
        rankUpWindow.SetActive(false);
        sellWindow.SetActive(false);
        characterInformCard.SetActive(false);
        UnselectCharacterInformSelectInventoryContent();
        characterInformObject = null;
        characterInformGoldText.GetComponent<Text>().text = "보유골드 : " + GameManager.instance.gameGold;
    }

    public void UpdateCharacterInformButton()
    {
        DeleteCharacterInformCharacterInventoryContent();
        DeleteCharacterInformSelectInventoryContent();
        CreateCharacterInformButtons();
        characterInformGoldText.GetComponent<Text>().text = "보유골드 : " + GameManager.instance.gameGold;
    }

    private void ChangeCharacterInformMode(CharacterInformMode mode)
    {
        ClearCharacterInform();
        characterInformMode = mode;
        if(mode == CharacterInformMode.None)
        { 
            characterInformObject = null;
            return;
        }
        else if (mode == CharacterInformMode.Upgrade)
        {
            characterInformObject = upgradeWindow;
            characterInformText.GetComponent<Text>().text = "강화";
            characterInformButtonText.GetComponent<Text>().text = "강화";
        }
        else if (mode == CharacterInformMode.RankUp)
        {
            characterInformObject = rankUpWindow;
            characterInformText.GetComponent<Text>().text = "랭크업";
            characterInformButtonText.GetComponent<Text>().text = "랭크업";
        }
        else if (mode == CharacterInformMode.Sell)
        {
            characterInformObject = sellWindow;
            characterInformText.GetComponent<Text>().text = "판매";
            characterInformButtonText.GetComponent<Text>().text = "판매";
        }
        characterInformObject.GetComponent<CharacterInform>().ClearInform();        
        characterInformObject.SetActive(true);        
    }

    public void SetSelectCharacter(int num)
    {
        characterInformObject.GetComponent<CharacterInform>().SelectCharacter(num);
    }

    public void UnselectCharacter(int num)
    {
        if(characterInformObject == null)
        { 
            return;    
        }

        characterInformObject.GetComponent<CharacterInform>().UnselectCharacter(num);
    }

    public void SetCharacterInventoryButton(int num, bool isSelect)
    {
        if (isSelect == true)
        {
            characterInformCharacterInventoryContent.transform.GetChild(num).gameObject.SetActive(false);
            characterInformSelectInventoryContent.transform.GetChild(num).gameObject.SetActive(true);
        }
        else
        {
            characterInformCharacterInventoryContent.transform.GetChild(num).gameObject.SetActive(true);
            characterInformSelectInventoryContent.transform.GetChild(num).gameObject.SetActive(false);
        }
    }

    public void SetCharacterInformButtonActiveFalse(int num)
    {
        characterInformCharacterInventoryContent.transform.GetChild(num).gameObject.SetActive(false);
    }
    public void SetCharacterInformCard(int num)
    {
        characterInformCard.SetActive(true);
        characterInformCard.GetComponent<CharacterCardController>().SetCardInformation(GameManager.instance.playerCharacter[num].status);
    }

    public void SetFalseCharacterInformCard()
    {
        characterInformCard.SetActive(false);
    }

    public void CharacterInformButton()
    {
        if (characterInformObject != null)
        {
            characterInformObject.GetComponent<CharacterInform>().CharacterInformButton();
        }
    }

    public void CancleCharacterInformButton()
    {
        if (characterInformObject != null)
        {
            characterInformObject.GetComponent<CharacterInform>().CancleCharacterInformButton();
        }
    }

    public void DeleteSelectCharacter()
    {
        for (int i = characterInformSelectInventoryContent.transform.childCount - 1; i >= 0; i--)
        {
            if (characterInformSelectInventoryContent.transform.GetChild(i).gameObject.activeSelf == true)
            {
                GameManager.instance.RemovePlayerCharacter(i);
            }
        }

        UpdateCharacterInformButton();
    }

    //랭크업 용
    public void SetSameCharacterInformButton(CharacterStatus status)
    {
        for (int i = 0; i < characterInformCharacterInventoryContent.transform.childCount; i++)
        {
            if((status.characterIndex != GameManager.instance.playerCharacter[i].status.characterIndex) || (status.characterRank != GameManager.instance.playerCharacter[i].status.characterRank))
            {
                characterInformCharacterInventoryContent.transform.GetChild(i).gameObject.SetActive(false);
            }            
        }
    }

    #endregion
    //---

    //메뉴용
    #region
    public void SetMenuCanvas(bool mode)
    {
        menuCanvas.SetActive(mode);
        isMenu = mode;
        if (mode == true)
        {
            SetMainButton(false);
        }
        else
        {
            SetMainButton(true);
        }
    }

    public void ReturnTitle()
    {
        GameManager.instance.ReturnTitle();
    }

    public void ExitGame()
    {
        GameManager.instance.ExitGame();
    }
    #endregion
    //--
}
