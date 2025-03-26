using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TalkType
{ 
    Set,
    Talking,
    TalkingSkip,
    TalkingEnd,
    Choice,
    Event
}  

public class TalkManager : MonoBehaviour
{
    private string chooseRouteText = "어느 쪽 길로 갈까?";
    private string[] chooseRouteChoiceText = new string[3] {"왼쪽", "중앙", "오른쪽"};

    [SerializeField]
    private GameObject talkWindow;
    [SerializeField]
    private GameObject talkText;
    [SerializeField]
    private GameObject choiceWindow;
    private GameObject[] choiceButton;
    [HideInInspector]
    public DungeonManager dungeonManager;
    [SerializeField]//확인용
    private TalkType talkType;

    //대화용
   private List<Dictionary<string, object>> talkData;

    private int CSVNum = 0;
    private int choiceCount = 0;

    private bool keyCheck = false;
    private string choiceKey = "";
    private float typingSpeed = 0.05f;
    private char lineBreakForTalk = '#';
    private string[] selectChoiceKey;

    private void Awake()
    { 
        InitTalkManager();
    }

    private void Update()
    {
        ChooseRouteUpdate();
        TalkEventUpdate();
    }

    private void InitTalkManager()
    {   
        #region
        if(this.gameObject.GetComponent<DungeonManager>() == true)
        { 
            dungeonManager = this.gameObject.GetComponent<DungeonManager>();
        }
        else
        { 
            Debug.LogWarning("게임 오브젝트가 DungeonManager를 들고 있지 않습니다!");
        }

        choiceButton = new GameObject[choiceWindow.transform.childCount];
        for (int i = 0; i < choiceWindow.transform.childCount; i++)
        {
            choiceButton[i] = choiceWindow.transform.GetChild(i).gameObject;
            SetChoiceButton(i,false);
        }

        selectChoiceKey = new string[choiceButton.Length];
        for (int i = 0; i < selectChoiceKey.Length; i++)
        {
            selectChoiceKey[i] = "";
        }

        if (chooseRouteChoiceText.Length > choiceButton.Length)
        {
            Debug.LogWarning("chooseRouteChoiceText가 choiceButton보다 많습니다!");
        }

        choiceCount = 0;
        CSVNum = 0;
        keyCheck = false;
        ChangeTalkType(TalkType.Set);
        #endregion
    }

    private void ChooseRouteUpdate()
    { 
        if(dungeonManager.GetDungeonPhase() == DungeonPhase.DungeonSetting)
        {             
            if(talkType == TalkType.TalkingEnd)
            { 
                SetChooseRouteChoice();   
            }
        }
    }

    private void TalkEventUpdate()
    {
        if (GameManager.instance.KeyCheckAccept() == true) //|| (GameManager.instance.KeyCheckSkip() == true)) // 전투쪽하고 루틴꼬이는 버그있음 
        {
            if (talkType == TalkType.Talking)
            {
                ChangeTalkType(TalkType.TalkingSkip);
            }
            else if (dungeonManager.GetDungeonPhase() == DungeonPhase.Event)
            {
                if(CantTalkCheck() == false)
                { 
                    PrepareNextTalk();
                }
            }
            else if (dungeonManager.GetDungeonPhase() == DungeonPhase.Battle)
            {
                InformTalkEndToBattleManager(); //전투용
            }
        }
    }

    private bool CantTalkCheck()
    {
        if ((talkType == TalkType.Set) || (talkType == TalkType.Choice) || (talkType == TalkType.Event) || (talkType == TalkType.TalkingSkip))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void SetDungeonChooseRoute()
    {
        SetChooseRouteText();
    }

    private void SetChooseRouteText()
    {         
        TalkStart(chooseRouteText);
        //talkWindow.SetActive(true);
        //talkText.GetComponent<Text>().text = chooseRouteText;
        //ChangeTalkType(TalkType.TalkingEnd);
    }

    private void SetChooseRouteChoice()
    {
        for(int i = 0; i < chooseRouteChoiceText.Length; i++)
        {
            if (i <= choiceButton.Length)
            {
                SetChoiceButtonText(i, chooseRouteChoiceText[i]);
            }
        }

        ChangeTalkType(TalkType.Choice);
        dungeonManager.ChangeDungeonPhase(DungeonPhase.ChooseRoute);
    }

    private void SetChoiceButtonText(int num, string text)
    {
        if((num < choiceButton.Length) && (0 <= num))
        { 
            choiceButton[num].transform.GetChild(0).GetComponent<Text>().text = text;
            SetChoiceButton(num,true);
        }
        else
        { 
            Debug.LogWarning("SetChoiceButtonText에 잘못된 값이 들어왔습니다! : " + num);    
        }
    }

    private void ClearChoiceButtonText()
    { 
        for(int i = 0; i < choiceButton.Length; i++)
        {
            choiceButton[i].transform.GetChild(0).GetComponent<Text>().text = "";
            SetChoiceButton(i,false);
        }        
    }

    public void SetChoiceButton(int num, bool mode)
    { 
        if((num < choiceButton.Length) && (0 <= num))
        { 
            choiceButton[num].SetActive(mode);
        }
        else
        { 
            Debug.LogWarning("SetChoiceButton 잘못된 값이 들어왔습니다! : " + num);    
        }
    }


    public void ChangeTalkType(TalkType newTalkType)
    { 
        talkType = newTalkType;
    }

    public TalkType GetTalkType()
    { 
        return talkType;
    }

    public void ChoiceButtonClick(int num)
    {
        if (dungeonManager.GetDungeonPhase() != DungeonPhase.Event)
        {
            for (int i = 0; i < choiceButton.Length; i++)
            {
                SetChoiceButton(i,false);
            }
            talkWindow.SetActive(false);
            dungeonManager.SelectRoute(num);
        }
        else
        {
            for (int i = 0; i < choiceButton.Length; i++)
            {
                SetChoiceButton(i,false);
            }
            choiceKey = selectChoiceKey[num];
            PrepareNextTalk();
        }
    }

    //대화용
    #region
    public void SetCSV(string CSVName)
    {
        talkData = CSVReader.Read(GameManager.instance.CSVFolder + CSVName, "");
        choiceCount = 0;
        CSVNum = 0;
        ResetTalkKey();
        Debug.Log("대본의 길이는 " + talkData.Count);
        NextTalk();
    }

    public void PrepareNextTalk()
    { 
        ChangeTalkType(TalkType.Set);
        IncreaseCSVNUM();
        NextTalk();     
    }

    private void IncreaseCSVNUM()
    { 
        CSVNum++;
    }

    private void NextTalk()
    {
        #region
        if(CSVNum < talkData.Count)
        {  
            if(talkData[CSVNum]["Event"].ToString() == "ResetKey")
            { 
                ResetKey();
            }
            else
            { 
                if(keyCheck == true)
                {
                    Debug.Log("KeyCheck조건이 걸려있습니다.");
                    if(talkData[CSVNum]["Branch"].ToString() == choiceKey)
                    {
                        Debug.Log("Branch = choiceKey임으로 내용을 출력합니다.");
                        NextTalkCheck();
                    }
                    else
                    {
                        Debug.Log("Branch != choiceKey임으로 내용을 스킵합니다.");
                        PrepareNextTalk();
                    }
                
                }
                else
                { 
                    NextTalkCheck();
                }
            }
        }
        else
        {
            dungeonManager.NextRouteStart();
        }
        #endregion
    }

    private void NextTalkCheck()
    {   
        #region
        if(talkData[CSVNum]["Event"].ToString() != "")
        { 
            if(EventCheck(CSVNum) == false)
            { 
                Talk(CSVNum);
            }
        }
        else
        { 
            Talk(CSVNum);
        }  
        #endregion
    }

    private bool EventCheck(int num)
    { 
        switch(talkData[num]["Event"].ToString())
        { 
            case "Choice":    
                Choice(num);
                return true;
            case "ChoiceEnd":    
                ChoiceEnd();
                return true;   
            case "IncreaseStatus":    
                IncreaseStatus(num);
                return true; 
            case "IncreaseStatusByDungeonDifficulty":    
                IncreaseStatusByDungeonDifficulty(num);
                return true;     
            case "CheckStatus":    
                CheckStatus(num);
                return true;
            case "CheckStatusByDungeonDifficulty":    
                CheckStatusByDungeonDifficulty(num);
                return true;   
            case "PlayVoice":
                PlayVoice(num);
                return true;   
            default:
                return false;
        }       
    }

    private void Talk(int num)
    { 
        #region
        talkText.GetComponent<Text>().text = "";
        //isTalk = true;
        TalkStart(talkData[num]["Content"].ToString());
        #endregion    
    }
    public void TalkStart(string text)
    {
        talkText.GetComponent<Text>().text = "";
        talkWindow.SetActive(true);
        ChangeTalkType(TalkType.Talking);
        StartCoroutine(Typing(text, typingSpeed));
    }

    IEnumerator Typing(string text, float talkSpeed)
    {
        #region
        if(talkType == TalkType.Talking)
        { 
            char[] word =  text.ToCharArray();
            for(int i = 0; i < word.Length; i++)
            {
                if(word[i] == lineBreakForTalk)
                { 
                    talkText.GetComponent<Text>().text += "\n";
                }
                else
                { 
                    talkText.GetComponent<Text>().text += word[i];
                }
                yield return new WaitForSeconds(talkSpeed);

                if(talkType == TalkType.TalkingSkip)
                {                    
                    talkText.GetComponent<Text>().text = "";

                    text = text.Replace(lineBreakForTalk, '\n');
                    talkText.GetComponent<Text>().text = text;
                    
                    Debug.Log("말하기 스킵");
                    break;                   
                }                
            }
            ChangeTalkType(TalkType.TalkingEnd);
        }
        #endregion
    }

    private void ResetKey()
    { 
        ChangeTalkType(TalkType.Event);
        ResetTalkKey();
        PrepareNextTalk();        
    }

    private void ResetTalkKey()
    { 
        #region
        keyCheck = false;
        choiceKey = "";
        for(int i = 0; i < selectChoiceKey.Length; i++)
        { 
            selectChoiceKey[i] = "";
        }       
        Debug.Log("키(선택지)설정이 초기화 되었습니다");      
        #endregion
    }

   private void Choice(int num)
   { 
        ChangeTalkType(TalkType.Event);
        if(choiceCount < choiceButton.Length)
        {
            Debug.Log(choiceCount + " 번 선택지 들어옴");                       
                
            choiceButton[choiceCount].transform.GetChild(0).GetComponent<Text>().text = talkData[num]["Content"].ToString();
            selectChoiceKey[choiceCount] = talkData[num]["Key"].ToString();
            choiceButton[choiceCount].SetActive(true);
            choiceCount++;
        }
        else
        { 
            Debug.Log("선택지가 버튼 갯수를 넘어갑니다.");               
        }            
        PrepareNextTalk();   
    }

    private void ChoiceEnd()
    {
        ChangeTalkType(TalkType.Choice);
        keyCheck = true;
        choiceCount = 0;

        var eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(choiceButton[0], new BaseEventData(eventSystem));
    }

    private void IncreaseStatus(int num)
    {
        #region
        ChangeTalkType(TalkType.Event);

        int increaseCount = 0;
        if (int.TryParse(talkData[num]["Content"].ToString(), out increaseCount) == false)
        {
            increaseCount = 0;
            Debug.LogError("csv파일 Content에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Content"].ToString());
        }

        switch(talkData[num]["Talker"].ToString())
        { 
            case "MaxHP":    
                dungeonManager.IncreaseMaxHP(increaseCount);
                break;       
            case "HP":    
                dungeonManager.IncreaseHP(increaseCount);
                break;     
            case "MaxMP":    
                dungeonManager.IncreaseMaxMP(increaseCount);
                break;           
            case "MP":    
                dungeonManager.IncreaseMP(increaseCount);
                break;                 
            case "ATK":    
                dungeonManager.IncreaseATK(increaseCount);
                break; 
            case "MAK":    
                dungeonManager.IncreaseMAK(increaseCount);
                break; 
            case "DEF":    
                dungeonManager.IncreaseDEF(increaseCount);
                break; 
            case "MDF":    
                dungeonManager.IncreaseMDF(increaseCount);
                break; 
            case "INT":    
                dungeonManager.IncreaseINT(increaseCount);
                break; 
            case "STA":    
                dungeonManager.IncreaseSTA(increaseCount);
                break; 
            case "Gold":    
                dungeonManager.IncreaseGold(increaseCount);
                break;
            case "EXP":    
                dungeonManager.IncreaseEXP(increaseCount);
                break;                 
            default:
                Debug.LogError("csv파일 Talker에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Talker"].ToString());
                break;
        }

        PrepareNextTalk();  
        #endregion
    }

    private void IncreaseStatusByDungeonDifficulty(int num)
    {
        #region
        ChangeTalkType(TalkType.Event);
        int increaseCount = 0;
        if (int.TryParse(talkData[num]["Content"].ToString(), out increaseCount) == false)
        {
            increaseCount = 0;
            Debug.LogError("csv파일 Content에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Content"].ToString());
        }
        increaseCount = increaseCount * GameManager.instance.selectDungeon.dungeonDifficulty;

        switch(talkData[num]["Talker"].ToString())
        { 
            case "MaxHP":    
                dungeonManager.IncreaseMaxHP(increaseCount);
                break;       
            case "HP":    
                dungeonManager.IncreaseHP(increaseCount);
                break;     
            case "MaxMP":    
                dungeonManager.IncreaseMaxMP(increaseCount);
                break;           
            case "MP":    
                dungeonManager.IncreaseMP(increaseCount);
                break;                 
            case "ATK":    
                dungeonManager.IncreaseATK(increaseCount);
                break; 
            case "MAK":    
                dungeonManager.IncreaseMAK(increaseCount);
                break; 
            case "DEF":    
                dungeonManager.IncreaseDEF(increaseCount);
                break; 
            case "MDF":    
                dungeonManager.IncreaseMDF(increaseCount);
                break; 
            case "INT":    
                dungeonManager.IncreaseINT(increaseCount);
                break; 
            case "STA":    
                dungeonManager.IncreaseSTA(increaseCount);
                break; 
            case "Gold":    
                dungeonManager.IncreaseGold(increaseCount);
                break;
            case "EXP":    
                dungeonManager.IncreaseEXP(increaseCount);
                break;                 
            default:
                Debug.LogError("csv파일 Talker에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Talker"].ToString());
                break;
        }
        PrepareNextTalk();  
        #endregion
    }

    private void CheckStatus(int num)
    {
        #region
        ChangeTalkType(TalkType.Event);
        string[] keys = talkData[num]["Key"].ToString().Split('/');
        keyCheck = true;
        if (keys.Length < 2)
        {
            Debug.LogError("csv파일 Key에 변경될 키 값이 하나만 들어왔습니다!");
            keys = new string[2] {talkData[num]["Key"].ToString(), "false"};
        }

        int checkCount = 0;
        if (int.TryParse(talkData[num]["Content"].ToString(), out checkCount) == false)
        {
            checkCount = 0;
            Debug.LogError("csv파일 Content에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Content"].ToString());
        }

        if (dungeonManager.CheckStatus(talkData[num]["Talker"].ToString(), checkCount) == true)
        {
            choiceKey = keys[0];
        }
        else
        {
            choiceKey = keys[1];
        }
        PrepareNextTalk();  
        #endregion
    }

    private void CheckStatusByDungeonDifficulty(int num)
    {
        #region
        ChangeTalkType(TalkType.Event);
        string[] keys = talkData[num]["Key"].ToString().Split('/');
        keyCheck = true;
        if (keys.Length < 2)
        {
            Debug.LogError("csv파일 Key에 변경될 키 값이 하나만 들어왔습니다!");
            keys = new string[2] {talkData[num]["Key"].ToString(), "false"};
        }

        int checkCount = 0;
        if (int.TryParse(talkData[num]["Content"].ToString(), out checkCount) == false)
        {
            checkCount = 0;
            Debug.LogError("csv파일 Content에 잘못 된 값이 들어 왔습니다! CSVNum = " + CSVNum + " 들어온 값은 " + talkData[num]["Content"].ToString());
        }

        checkCount = checkCount * GameManager.instance.selectDungeon.dungeonDifficulty;

        if (dungeonManager.CheckStatus(talkData[num]["Talker"].ToString(), checkCount) == true)
        {
            choiceKey = keys[0];
        }
        else
        {
            choiceKey = keys[1];
        }
        PrepareNextTalk(); 
        #endregion
    }

    private void PlayVoice(int num)
    {
        ChangeTalkType(TalkType.Event);
        dungeonManager.PlayVoice(talkData[num]["Content"].ToString());
        PrepareNextTalk(); 
    }
    #endregion

    private void InformTalkEndToBattleManager()
    {
        if (dungeonManager.GetDungeonPhase() == DungeonPhase.Battle)
        {
            dungeonManager.battleManager.TalkEnd();
        }
    }
}
