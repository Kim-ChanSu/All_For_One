using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DungeonPhase
{ 
    RouteStart,
    DungeonSetting,
    ChooseRoute,
    Event,
    Battle
}

public enum DungeonRoute
{
    Enemy,
    NormalEvent
}

[System.Serializable]
public struct DungeonCharacter
{
    public CharacterStatus status;
    public int HP; 
    public int MP;
    public int level;
    public int maxEXP;
    public int EXP;
}

public class DungeonManager : MonoBehaviour
{
    [HideInInspector]
    public TalkManager talkManager;
    [HideInInspector]
    public BattleManager battleManager;
    private DungeonPhase dungeonPhase;
    [SerializeField] //확인용
    private DungeonRoute[] dungeonRoute;
    public DungeonCharacter originalStatus;
    public DungeonCharacter playerCharacter;
    private int dungeonRouteLength = 3;
    private int dungeonCount = 0;
    private int dungeonGold = 0;

    [SerializeField]
    private GameObject playerCharacterFaceImage;
    [SerializeField]
    private GameObject playerCharacterNameText;
    [SerializeField]
    private GameObject playerCharacterStatusText;
    [SerializeField]
    private GameObject playerCharacterHPMask;
    [SerializeField]
    private GameObject playerCharacterMPMask;
    [SerializeField]
    private GameObject playerCharacterEXPMask;
    [SerializeField]
    private GameObject playerCharacterATKText;
    [SerializeField]
    private GameObject playerCharacterMAKText;
    [SerializeField]
    private GameObject playerCharacterINTText;
    [SerializeField]
    private GameObject playerCharacterDEFText;
    [SerializeField]
    private GameObject playerCharacterMDFText;
    [SerializeField]
    private GameObject playerCharacterSTAText;
    [SerializeField]
    private GameObject playerCharacterLVText;

    [SerializeField]
    private GameObject playerGoldText;
    [SerializeField]
    private GameObject dungeonCountText;

    private void Awake()
    {
        InitDungeonManager();
    }

    private void Update()
    {
        DungeonUpdate();
    }

    private void InitDungeonManager()
    {
        GameManager.instance.dungeonManager = this;
        dungeonCount = 0;
        dungeonRoute = new DungeonRoute[dungeonRouteLength];
        ChangeDungeonPhase(DungeonPhase.RouteStart);
        SetDungeonPlayerCharacter();
        UpdateStatus();

        if (this.gameObject.GetComponent<TalkManager>() == true)
        {
            talkManager = this.gameObject.GetComponent<TalkManager>();
        }
        else
        {
            Debug.LogWarning("게임오브젝트가 TalkManager를 들고 있지 않습니다!");
        }

        if (this.gameObject.GetComponent<BattleManager>() == true)
        {
            battleManager = this.gameObject.GetComponent<BattleManager>();
        }
        else
        {
            Debug.LogWarning("게임오브젝트가 battleManager를 들고 있지 않습니다!");
        }      

        StartDungeonBGM();
    }

    private void SetDungeonPlayerCharacter()
    {
        playerCharacter = new DungeonCharacter();
        playerCharacter.status = GameManager.instance.playerCharacter[GameManager.instance.selectCharacterNum].status;
        playerCharacter.HP = GameManager.instance.playerCharacter[GameManager.instance.selectCharacterNum].status.maxHP;
        playerCharacter.MP = GameManager.instance.playerCharacter[GameManager.instance.selectCharacterNum].status.maxMP;
        playerCharacter.level = 1;
        playerCharacter.maxEXP = 10;
        dungeonGold = (GameManager.instance.playerCharacter[GameManager.instance.selectCharacterNum].status.STA) * (100 / GameManager.instance.selectDungeon.dungeonDifficulty);
        originalStatus = new DungeonCharacter();
        originalStatus = playerCharacter;
    }


    private void UpdatePlayerCharacterInterface()
    {
        playerCharacterFaceImage.GetComponent<Image>().sprite = playerCharacter.status.face;
        playerCharacterNameText.GetComponent<Text>().text = "[ " + playerCharacter.status.name + " ]";
        playerCharacterStatusText.GetComponent<Text>().text = "상태 : 정상";
        playerCharacterHPMask.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)playerCharacter.HP / (float)playerCharacter.status.maxHP;
        playerCharacterHPMask.transform.GetChild(1).GetComponent<Text>().text = playerCharacter.HP + " / " + playerCharacter.status.maxHP;
        playerCharacterMPMask.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)playerCharacter.MP / (float)playerCharacter.status.maxMP;
        playerCharacterMPMask.transform.GetChild(1).GetComponent<Text>().text = playerCharacter.MP + " / " + playerCharacter.status.maxMP;
        playerCharacterEXPMask.transform.GetChild(0).GetComponent<Image>().fillAmount = (float)playerCharacter.EXP / (float)playerCharacter.maxEXP;
        playerCharacterEXPMask.transform.GetChild(1).GetComponent<Text>().text = playerCharacter.EXP + " / " + playerCharacter.maxEXP;
        playerCharacterATKText.GetComponent<Text>().text = playerCharacter.status.ATK + "";
        playerCharacterMAKText.GetComponent<Text>().text = playerCharacter.status.MAK + "";
        playerCharacterINTText.GetComponent<Text>().text = playerCharacter.status.INT + "";
        playerCharacterDEFText.GetComponent<Text>().text = playerCharacter.status.DEF + "";
        playerCharacterMDFText.GetComponent<Text>().text = playerCharacter.status.MDF + "";
        playerCharacterSTAText.GetComponent<Text>().text = playerCharacter.status.STA + "";
        playerCharacterLVText.GetComponent<Text>().text = "LV : " + playerCharacter.level;

        playerGoldText.GetComponent<Text>().text = "골드 : " + dungeonGold;
        dungeonCountText.GetComponent<Text>().text = "던전진행도\n" + dungeonCount + " / " + GameManager.instance.selectDungeon.dungeonLength;
    }

    private void StartDungeonBGM()
    {
        if (GameManager.instance.selectDungeon.dungeonBGM != null)
        {
            GameManager.instance.ChangeBGM(GameManager.instance.selectDungeon.dungeonBGM);
        }
    }

    private void StartBattleBGM()
    {
        if (GameManager.instance.selectDungeon.dungeonBattleBGM != null)
        {
            GameManager.instance.ChangeBGM(GameManager.instance.selectDungeon.dungeonBattleBGM);
        }
    }

    private void DungeonUpdate()
    {
        if (dungeonPhase == DungeonPhase.RouteStart)
        {
            DungeonSetting();
        }
    }

    private void DungeonSetting()
    {
        if (GameManager.instance.selectDungeon.dungeonLength < dungeonCount)
        {
            ClearDungeon();
            return;
        }

        SetDungeonRoute();
        talkManager.SetDungeonChooseRoute();
        ChangeDungeonPhase(DungeonPhase.DungeonSetting);
    }

    public void ChangeDungeonPhase(DungeonPhase newPhase)
    {
        UpdateStatus();
        dungeonPhase = newPhase;
    }

    public DungeonPhase GetDungeonPhase()
    {
        return dungeonPhase;
    }

    private void SetDungeonRoute()
    {
        for (int i = 0; i < dungeonRoute.Length; i++)
        {
            int dungeonRouteNum = Random.Range(0, System.Enum.GetValues(typeof(DungeonRoute)).Length);

            if (dungeonRouteNum >= System.Enum.GetValues(typeof(DungeonRoute)).Length)
            {
                Debug.LogWarning("잘못 된 값이 들어왔습니다!");
                dungeonRouteNum = 0;
            }

            dungeonRoute[i] = (DungeonRoute)dungeonRouteNum;
        }
    }

    private void ClearDungeon()
    {
        Debug.Log("던전 클리어!");
        GameManager.instance.gameGold = GameManager.instance.gameGold + GameManager.instance.selectDungeon.dungeonClearGold;
        if (GameManager.instance.selectDungeon.isDungeonClear != true)
        {
            GameManager.instance.selectDungeon.isDungeonClear = true;
            GameManager.instance.gameProgress += 1;
        }
        GameManager.instance.ChangeScene(GameManager.MAINSCENENAME);
    }

    private void LoseDungeon()
    {
        Debug.Log("쳐발림!");
        GameManager.instance.ChangeScene(GameManager.MAINSCENENAME);
    }

    public void SelectRoute(int num)
    {
        dungeonCount++;
        UpdateStatus();
        if ((num < 0) && (num >= dungeonRouteLength))
        {
            Debug.LogError("잘못 된 값이 들어왔습니다!");
            num = 0;
        }

        for (int i = 0; i < GameManager.instance.selectDungeon.dungeonSpecialEnemy.Length; i++)
        {
            if (GameManager.instance.selectDungeon.dungeonSpecialEnemy[i].enemyMeetCount == this.dungeonCount)
            {
                BattleWithSpecialEnemy(GameManager.instance.selectDungeon.dungeonSpecialEnemy[i]);
                return;
            }
        }

        SetRouteEvent(num);
    }

    private void SetRouteEvent(int num)
    {
        if ((num < 0) && (num >= dungeonRouteLength))
        {
            Debug.LogError("잘못 된 값이 들어왔습니다!");
            num = 0;
        }

        switch (dungeonRoute[num])
        {
            case DungeonRoute.Enemy:
                BattleStart();
                /*
                if (GetRandomEvent(DungeonRoute.Enemy.ToString()) == false)
                {
                    NextRouteStart();
                }
                */
                break;
            case DungeonRoute.NormalEvent:
                if (GetRandomEvent(DungeonRoute.NormalEvent.ToString()) == false)
                {
                    NextRouteStart();
                }
                break;
            default:
                NextRouteStart();
                break;
        }
    }

    private bool GetRandomEvent(string eventPath)
    { 
        TextAsset[] eventCSV;
        eventCSV = Resources.LoadAll<TextAsset>(GameManager.instance.CSVFolder + eventPath);

        Debug.Log("파악된 이벤트의 수는 " + eventCSV.Length);
        if(eventCSV.Length < 1)
        { 
            Debug.LogWarning("이벤트가 한 개도 존재하지 않습니다!");
            return false;
        }
        else
        {
            int eventNum = Random.Range(0, eventCSV.Length);
            Debug.Log("선택된 이벤트는 " + eventCSV[eventNum].name);
            ChangeDungeonPhase(DungeonPhase.Event);
            talkManager.SetCSV(eventPath + "/" + eventCSV[eventNum].name); 
            return true;
        }
    }

    private void BattleWithSpecialEnemy(DunGeonSpecialEnemy specialEnemy)
    {
        Debug.Log("SpecialEnemy");
        SpecialEnemyBattle(specialEnemy);
    }

    public void NextRouteStart()
    {
        if (GameManager.instance.selectDungeon.dungeonLength <= dungeonCount)
        {
            ClearDungeon();
            return;
        }
        ChangeDungeonPhase(DungeonPhase.RouteStart);
        UpdateStatus();
    }

    //스테이터스 조작
    public void UpdateStatus()
    {
        playerCharacter.status.maxHP = originalStatus.status.maxHP;
        playerCharacter.HP = originalStatus.HP;
        playerCharacter.status.maxMP = originalStatus.status.maxMP;
        playerCharacter.MP = originalStatus.MP;
        playerCharacter.status.ATK = originalStatus.status.ATK;
        playerCharacter.status.MAK = originalStatus.status.MAK;
        playerCharacter.status.DEF = originalStatus.status.DEF;
        playerCharacter.status.MDF = originalStatus.status.MDF;
        playerCharacter.status.INT = originalStatus.status.INT;
        playerCharacter.status.STA = originalStatus.status.STA;
        #region
        playerCharacter.level = originalStatus.level;
        playerCharacter.maxEXP = originalStatus.maxEXP;
        playerCharacter.EXP = originalStatus.EXP;
        playerCharacter.status.characterDefaultAttackNum = originalStatus.status.characterDefaultAttackNum;
        playerCharacter.status.maxHPValue = originalStatus.status.maxHPValue;
        playerCharacter.status.maxMPValue = originalStatus.status.maxMPValue;
        playerCharacter.status.ATKValue = originalStatus.status.ATKValue;
        playerCharacter.status.MAKValue = originalStatus.status.MAKValue;
        playerCharacter.status.DEFValue = originalStatus.status.DEFValue;
        playerCharacter.status.MDFValue = originalStatus.status.MDFValue;
        playerCharacter.status.INTValue = originalStatus.status.INTValue;
        playerCharacter.status.STAValue = originalStatus.status.STAValue;
        #endregion
        UpdatePlayerCharacterInterface();
    }


    public void IncreaseMaxHP(int count)
    {
        originalStatus.status.maxHP += count;
        UpdateStatus();
    }

    public void IncreaseHP(int count)
    {
        originalStatus.HP += count;
        if (originalStatus.HP > originalStatus.status.maxHP)
        {
            originalStatus.HP = originalStatus.status.maxHP;
        }
        else if(originalStatus.HP <= 0)
        {
            PlayVoice("Die");
            LoseDungeon();
            return;
        }
        UpdateStatus();
    }

    public void IncreaseMaxMP(int count)
    {
        originalStatus.status.maxMP += count;
        UpdateStatus();
    }

    public void IncreaseMP(int count)
    {
        originalStatus.MP += count;
        if (originalStatus.MP > originalStatus.status.maxMP)
        {
            originalStatus.MP = originalStatus.status.maxMP;
        }
        else if(originalStatus.MP <= 0)
        {
            originalStatus.MP = 0;
            return;
        }
        UpdateStatus();
    }

    public void IncreaseATK(int count)
    {
        originalStatus.status.ATK += count;
        if (originalStatus.status.ATK <= 0)
        {
            originalStatus.status.ATK = 1;
        }
        UpdateStatus();
    }

    //
    public void IncreaseMAK(int count)
    {
        originalStatus.status.MAK += count;
        if (originalStatus.status.MAK <= 0)
        {
            originalStatus.status.MAK = 1;
        }
        UpdateStatus();
    }

    public void IncreaseDEF(int count)
    {
        originalStatus.status.DEF += count;
        if (originalStatus.status.DEF <= 0)
        {
            originalStatus.status.DEF = 1;
        }
        UpdateStatus();
    }

    public void IncreaseMDF(int count)
    {
        originalStatus.status.MDF += count;
        if (originalStatus.status.MDF <= 0)
        {
            originalStatus.status.MDF = 1;
        }
        UpdateStatus();
    }

    public void IncreaseINT(int count)
    {
        originalStatus.status.INT += count;
        if (originalStatus.status.INT <= 0)
        {
            originalStatus.status.INT = 1;
        }
        UpdateStatus();
    }

    public void IncreaseSTA(int count)
    {
        originalStatus.status.STA += count;
        if (originalStatus.status.STA <= 0)
        {
            originalStatus.status.STA = 1;
        }
        UpdateStatus();
    }

    public void IncreaseGold(int count)
    {
        dungeonGold += count;
        if (dungeonGold <= 0)
        {
            dungeonGold = 0;
        }
        UpdateStatus();
    }

    public int GetDungeonGold()
    {
        return dungeonGold;
    }

    public void IncreaseEXP(int count)
    {
        originalStatus.EXP += count;
        if (originalStatus.EXP <= 0)
        {
            originalStatus.EXP = 1;
        }
        LevelUPCheck();
        UpdateStatus();
    }

    private void LevelUPCheck()
    {
        while (originalStatus.EXP >= originalStatus.maxEXP)
        {
            originalStatus.EXP -= originalStatus.maxEXP;
            LevelUP();
        }
    }

    private void MaxEXPUP()
    {
        int value = GameManager.instance.selectDungeon.dungeonDifficulty;
        originalStatus.maxEXP += (originalStatus.level + value) * (originalStatus.level + value) + (value + 4) * (originalStatus.level - 1); 
    }

    private void LevelUP() //나중에 스킬 추가해야함
    {
        #region
        originalStatus.level++;
        MaxEXPUP();

        int SMAXValue = 11;
        int SMINValue = 8;

        int AMAXValue = 9;
        int AMINValue = 6;

        int BMAXValue = 8;
        int BMINValue = 5;

        int CMAXValue = 6;
        int CMINValue = 3;

        int DMAXValue = 4;
        int DMINValue = 2;

        int EMAXValue = 3; 
        int EMINValue = 1;

        int FMAXValue = 2;
        int FMINValue = 0;

        int HPCorrection = 5;
        int MPCorrection = 5;

        //HP
        int maxHPUp = 0;
        switch(originalStatus.status.maxHPValue)
        { 
            case StatusValue.S:
            maxHPUp = Random.Range(SMAXValue, SMINValue) * HPCorrection;
            break;

            case StatusValue.A:
            maxHPUp = Random.Range(AMAXValue, AMINValue) * HPCorrection;
            break;

            case StatusValue.B:
            maxHPUp = Random.Range(BMAXValue, BMINValue) * HPCorrection;
            break;

            case StatusValue.C:
            maxHPUp = Random.Range(CMAXValue, CMINValue) * HPCorrection;
            break;

            case StatusValue.D:
            maxHPUp = Random.Range(DMAXValue, DMINValue) * HPCorrection;
            break;

            case StatusValue.E:
            maxHPUp = Random.Range(EMAXValue, EMINValue) * HPCorrection;
            break;

            case StatusValue.F:
            maxHPUp = Random.Range(FMAXValue, FMINValue) * HPCorrection;
            break;

            default:
            Debug.LogWarning("HPValue가 이상합니다. 현재 HPValue의 값 = " + originalStatus.status.maxHPValue);
            break; 
        }
        IncreaseMaxHP(maxHPUp);
        IncreaseHP(maxHPUp);

        //MP
        int maxMPUp = 0;
        switch(originalStatus.status.maxMPValue)
        { 
            case StatusValue.S:
            maxMPUp = Random.Range(SMAXValue, SMINValue) * MPCorrection;
            break;

            case StatusValue.A:
            maxMPUp = Random.Range(AMAXValue, AMINValue) * MPCorrection;
            break;

            case StatusValue.B:
            maxMPUp = Random.Range(BMAXValue, BMINValue) * MPCorrection;
            break;

            case StatusValue.C:
            maxMPUp = Random.Range(CMAXValue, CMINValue) * MPCorrection;
            break;

            case StatusValue.D:
            maxMPUp = Random.Range(DMAXValue, DMINValue) * MPCorrection;
            break;

            case StatusValue.E:
            maxMPUp = Random.Range(EMAXValue, EMINValue) * MPCorrection;
            break;

            case StatusValue.F:
            maxMPUp = Random.Range(FMAXValue, FMINValue) * MPCorrection;
            break;

            default:
            Debug.LogWarning("MPValue가 이상합니다. 현재 MPValue의 값 = " + originalStatus.status.maxMPValue);
            break; 
        }
        IncreaseMaxMP(maxMPUp);
        IncreaseMP(maxMPUp);

        //ATK
        int ATKUp = 0;
        switch(originalStatus.status.ATKValue)
        { 
            case StatusValue.S:
            ATKUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            ATKUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            ATKUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            ATKUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            ATKUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            ATKUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            ATKUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("ATKValue가 이상합니다. 현재 ATKValue의 값 = " + originalStatus.status.ATKValue);
            break; 
        }
        IncreaseATK(ATKUp);

        //MAK
        int MAKUp = 0;
        switch(originalStatus.status.MAKValue)
        { 
            case StatusValue.S:
            MAKUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            MAKUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            MAKUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            MAKUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            MAKUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            MAKUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            MAKUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("MAKValue가 이상합니다. 현재 MAKValue의 값 = " + originalStatus.status.MAKValue);
            break; 
        }
        IncreaseMAK(MAKUp);

        //DEF
        int DEFUp = 0;
        switch(originalStatus.status.DEFValue)
        { 
            case StatusValue.S:
            DEFUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            DEFUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            DEFUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            DEFUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            DEFUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            DEFUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            DEFUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("DEFValue가 이상합니다. 현재 DEFValue의 값 = " + originalStatus.status.DEFValue);
            break; 
        }
        IncreaseDEF(DEFUp);

        //MDF
        int MDFUp = 0;
        switch(originalStatus.status.MDFValue)
        { 
            case StatusValue.S:
            MDFUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            MDFUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            MDFUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            MDFUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            MDFUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            MDFUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            MDFUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("MDFValue가 이상합니다. 현재 MDFValue의 값 = " + originalStatus.status.MDFValue);
            break; 
        }
        IncreaseMDF(MDFUp);

        //INT
        int INTUp = 0;
        switch(originalStatus.status.INTValue)
        { 
            case StatusValue.S:
            INTUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            INTUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            INTUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            INTUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            INTUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            INTUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            INTUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("INTValue가 이상합니다. 현재 INTValue의 값 = " + originalStatus.status.INTValue);
            break; 
        }
        IncreaseINT(INTUp);

        //STA
        int STAUp = 0;
        switch(originalStatus.status.STAValue)
        { 
            case StatusValue.S:
            STAUp = Random.Range(SMAXValue, SMINValue);
            break;

            case StatusValue.A:
            STAUp = Random.Range(AMAXValue, AMINValue);
            break;

            case StatusValue.B:
            STAUp = Random.Range(BMAXValue, BMINValue);
            break;

            case StatusValue.C:
            STAUp = Random.Range(CMAXValue, CMINValue);
            break;

            case StatusValue.D:
            STAUp = Random.Range(DMAXValue, DMINValue);
            break;

            case StatusValue.E:
            STAUp = Random.Range(EMAXValue, EMINValue);
            break;

            case StatusValue.F:
            STAUp = Random.Range(FMAXValue, FMINValue);
            break;

            default:
            Debug.LogWarning("STAValue가 이상합니다. 현재 STAValue의 값 = " + originalStatus.status.STAValue);
            break; 
        }
        IncreaseSTA(STAUp);
        #endregion
    }

    public bool CheckStatus(string statusName, int checkCount)
    {
        #region
        switch(statusName)
        { 
            case "MaxHP":
                if(originalStatus.status.maxHP > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }     
            case "HP":    
                if(originalStatus.HP > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }    
            case "MaxMP":    
                if(originalStatus.status.maxMP > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }         
            case "MP":    
                if(originalStatus.MP > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }                
            case "ATK":    
                if(originalStatus.status.ATK > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "MAK":    
                if(originalStatus.status.MAK > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "DEF":    
                if(originalStatus.status.DEF > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "MDF":    
                if(originalStatus.status.MDF > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "INT":    
                if(originalStatus.status.INT > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }    
            case "STA":    
                if(originalStatus.status.STA > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "Gold":    
                if(dungeonGold > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }   
            case "Level":    
                if(originalStatus.level > checkCount)
                {
                    return true;
                }
                else
                { 
                    return false;
                }                 
            default:
                Debug.LogError("CheckStatus에 잘못 된 값이 들어 왔습니다! 들어온 값은 " + statusName);
                return false;
        }
        #endregion
    }

    public void PlayVoice(string voiceType)
    {
        switch(voiceType)
        { 
            case "Select":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.Select);
                break;
            case "GetCharacter":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.GetCharacter);
                break;
            case "AttackSkill":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.AttackSkill);
                break;
            case "BuffSkill":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.BuffSkill);
                break;
            case "DeBuffSkill":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.DeBuffSkill);
                break;
            case "BattleWin":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.BattleWin);
                break;
            case "Die":
                VoiceDatabaseManager.instance.PlayVoice(playerCharacter.status.voiceType, VoiceType.Die);
                break;
            default:
                Debug.LogError("PlayVoice에 잘못 된 값이 들어 왔습니다! 들어온 값은 " + voiceType);
                break;
        }            
    }

    public BattlePhase GetBattlePhase()
    {
        return battleManager.GetBattlePhase();
    }

    //전투용
    public void BattleStart()
    {
        ChangeDungeonPhase(DungeonPhase.Battle);
        StartBattleBGM();
        if (GameManager.instance.selectDungeon.enemyGroup.Length < 1)
        {
            Debug.LogWarning("적이 없습니다!");
            BattleEnd();
        }
        int groupNum = Random.Range(0, GameManager.instance.selectDungeon.enemyGroup.Length);

        battleManager.BattleStart(GameManager.instance.selectDungeon.enemyGroup[groupNum]);
    }

    private void SpecialEnemyBattle(DunGeonSpecialEnemy specialEnemy)
    {
        ChangeDungeonPhase(DungeonPhase.Battle);
        if (specialEnemy.enemyBattleBGM != null)
        {
            GameManager.instance.ChangeBGM(specialEnemy.enemyBattleBGM);
        }

        if (specialEnemy.enemyGroup != null)
        {
            battleManager.BattleStart(specialEnemy.enemyGroup);
        }
        else
        {
            Debug.LogWarning("적이 없습니다!");
            BattleEnd();
        }
    }

    public void BattleEnd()
    {
        StartDungeonBGM();
        UpdateStatus();
        NextRouteStart();
    }
}

