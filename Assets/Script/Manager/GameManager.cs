using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public const int RESETINDEX = -1;
    public const string TITLESCENENAME = "TitleScene";
    public const string MAINSCENENAME = "MainScene";
    public const string DUNGEONSCENENAME = "DungeonScene";

    [HideInInspector] 
    public DungeonManager dungeonManager;
    [HideInInspector] 
    public MainManager mainManager;
    [SerializeField]
    private AudioSource gameBGM;
    [SerializeField]
    private AudioSource gameSE;
    [SerializeField]
    private AudioSource gameVoice;

    private bool isBGMPlay;

    public List<PlayerCharacter> playerCharacter = new List<PlayerCharacter>();
   [HideInInspector] 
    public Dungeon selectDungeon;
    [HideInInspector] 
    public int selectCharacterNum;
    public int gameProgress = 0; //게임진행도
    public int gameGold = 0;

    [HideInInspector]
    public string CSVFolder;

    private void Awake() 
    {
        InitGameManager();
    }

    private void InitGameManager()
    { 
        // 싱글턴
        #region
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 게임매니저가 2개이상 존재합니다.");
            Destroy(this.gameObject);
        }
        #endregion

        playerCharacter = new List<PlayerCharacter>();
        isBGMPlay = true;
        gameProgress = 0;
        gameGold = 0;
        selectCharacterNum = RESETINDEX;
        CSVFolder = "CSV/";
    }

    //캐릭터 획득, 제거
    public void GetPlayerCharacter(Character character)
    { 
        PlayerCharacter newPlayerCharacter = new PlayerCharacter();
        newPlayerCharacter.SetCharacterStatus(character.status);
        newPlayerCharacter.SetPlayerCharacterIndex(playerCharacter.Count);
        playerCharacter.Add(newPlayerCharacter);
    }

    public void RemovePlayerCharacter(int num)
    { 
        for(int i = num; i < playerCharacter.Count; i++)
        { 
           playerCharacter[i].SetPlayerCharacterIndex(playerCharacter[i].GetPlayerCharacterIndex() - 1); 
        }
        playerCharacter.Remove(playerCharacter[num]);
    }
    //-----------------

    //게임시작 및 종료용
    public void StartGame()
    {
        SceneManager.LoadScene(MAINSCENENAME);
    }

    public void ReturnTitle()
    {
        Destroy(this.gameObject);
        SceneManager.LoadScene(TITLESCENENAME);
    }

    public void ExitGame()
    { 
        Application.Quit();
    }

    public void ChangeScene(string SceneName)
    { 
        SceneManager.LoadScene(SceneName);        
    }
    //--------

    //-- 사운드용
    #region
    public void ChangeBGM(AudioClip music)
    { 
        Debug.Log("바꾸는 브금이름은 " + music.ToString());
        gameBGM.time = 0;
        gameBGM.clip = music;
        if(isBGMPlay == true)
        {     
            gameBGM.Play();
        }
    }

    public void StartBGM()
    {
        isBGMPlay = true;
        gameBGM.Play();
    }    

    public void StopBGM()
    {
        isBGMPlay = false;
        gameBGM.Stop();
    }

    public void SetBGMMute(bool mode)
    {
        gameBGM.mute = mode;
    }

    public void PlaySE(AudioClip music)
    { 
        if(music != null)
        { 
            gameSE.clip = music;
            gameSE.Play();
        }
        else
        { 
            Debug.LogWarning("PlaySE에 null값이 들어왔습니다!");
        }
    } 

    public void StopSE()
    { 
        gameSE.Stop();
    } 

    public void PlayVoice(AudioClip music)
    { 
        if(music != null)
        { 
            gameVoice.clip = music;
            gameVoice.Play();
        }
        else
        { 
            Debug.LogWarning("PlayVoice에 null값이 들어왔습니다!");
        }
    }

    public void StopVoice()
    {
        gameVoice.Stop(); 
    }
    #endregion
    //--

    public bool SetSelectCharacterNum(int num)
    {
        if ((0 <= num) && (num < playerCharacter.Count))
        {
            selectCharacterNum = num;
            return true;
        }
        else
        {
            return false;
        }
    }

    //업그레이드용
    #region
    public void CharacterUpgradeEXPUp(int characterNum, int upEXP)
    {
        playerCharacter[characterNum].status.upgradeEXP = playerCharacter[characterNum].status.upgradeEXP + upEXP;
        CheckCharacterUpgrade(characterNum);
    }

    private void CheckCharacterUpgrade(int characterNum)
    {
        if (playerCharacter[characterNum].status.upgradeEXP >= playerCharacter[characterNum].status.upgradeMaxEXP)
        {
            playerCharacter[characterNum].status.upgradeEXP = playerCharacter[characterNum].status.upgradeEXP - playerCharacter[characterNum].status.upgradeMaxEXP;
            CharacterUpgrade(characterNum);
        }
    }

    public void CharacterUpgrade(int characterNum)
    {
        CharacterUpgradeMaxEXPUp(characterNum);
        playerCharacter[characterNum].status.characterUpgrade = playerCharacter[characterNum].status.characterUpgrade + 1;
        playerCharacter[characterNum].status.maxHP = (int)((float)playerCharacter[characterNum].status.maxHP * 1.1f);
        playerCharacter[characterNum].status.maxMP = (int)((float)playerCharacter[characterNum].status.maxMP * 1.1f);
        playerCharacter[characterNum].status.ATK = (int)((float)playerCharacter[characterNum].status.ATK * 1.1f);
        playerCharacter[characterNum].status.MAK = (int)((float)playerCharacter[characterNum].status.MAK * 1.1f);
        playerCharacter[characterNum].status.DEF = (int)((float)playerCharacter[characterNum].status.DEF * 1.1f);
        playerCharacter[characterNum].status.MDF = (int)((float)playerCharacter[characterNum].status.MDF * 1.1f);

        CheckCharacterUpgrade(characterNum);
    }

    private void CharacterUpgradeMaxEXPUp(int characterNum)
    {
        playerCharacter[characterNum].status.upgradeMaxEXP = UpgradeMaxEXPUp(playerCharacter[characterNum].status.upgradeMaxEXP);
    }

    public int UpgradeMaxEXPUp(int nowMaxEXP)
    {
        return (int)((float)nowMaxEXP * 1.5f);
    }

    public int GetUpgradeCost(int upEXP)
    {
        return (int)((float)upEXP * 0.5f);
    }

    public int GetUpgradeEXP(PlayerCharacter checkCharacter)
    {
        int upEXP = 50;
        upEXP = upEXP * (checkCharacter.status.characterUpgrade + 1) * ((int)checkCharacter.status.characterRank + 1);
        return upEXP;
    }
    #endregion
    //--
    // 랭크업용
    #region
    public int GetRankUpCost(CharacterRank rank)
    {
        int cost = 500;
        for (int i = 0; i < (int)rank; i++)
        {
            cost = cost * 2;
        }

        return cost;
    }

    public int GetRankUpNeedCharacter(CharacterRank rank)
    {
        return (int)rank + 1;
    }

    public void CharacterRankUp(int characterNum)
    {
        #region
        if ((int)(playerCharacter[characterNum].status.characterRank) < (System.Enum.GetValues(typeof(CharacterRank)).Length) - 1)
        {
            playerCharacter[characterNum].status.characterRank = playerCharacter[characterNum].status.characterRank + 1;

            if ((int)(playerCharacter[characterNum].status.maxHPValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.maxHPValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.maxMPValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.maxMPValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.ATKValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.ATKValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.MAKValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.MAKValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.DEFValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.DEFValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.MDFValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.MDFValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.INTValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.INTValue += 1;
            }

            if ((int)(playerCharacter[characterNum].status.STAValue) < (System.Enum.GetValues(typeof(StatusValue)).Length) - 1)
            {
                playerCharacter[characterNum].status.STAValue += 1;
            }

            playerCharacter[characterNum].status.INT = playerCharacter[characterNum].status.INT + 5;
            playerCharacter[characterNum].status.STA = playerCharacter[characterNum].status.STA + 5;
        }
        else
        {
            Debug.Log("이미 최고 랭크입니다");
        }
        #endregion
    }
    #endregion
    //판매용
    #region
    public int GetSellGold(PlayerCharacter checkCharacter)
    {
        int sellGold = 50;
        sellGold = sellGold * (checkCharacter.status.characterUpgrade + 1) * ((int)checkCharacter.status.characterRank + 1);
        return sellGold;
    }
    #endregion

    //키체크용
    public bool KeyCheckAccept()
    { 
        if(Input.GetKeyUp(KeyCode.Return) == true || Input.GetKeyUp(KeyCode.Mouse0) == true || Input.GetKeyUp(KeyCode.Space) == true )
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

    public bool KeyCheckSkip()
    { 
        if(Input.GetKey(KeyCode.LeftControl) == true)
        { 
            return true;    
        }    
        else
        { 
            return false;    
        }
    }

}


