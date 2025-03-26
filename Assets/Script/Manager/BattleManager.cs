using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct DungeonEnemyCharacter
{
    public CharacterStatus status;
    public int HP; 
    public int MP;
    public AiSkill[] aiSkill;
    public bool isDead;

    public int dropGold;
    public int dropEXP;
}

public enum BattlePhase
{ 
    SetBattle, //전투를 맨 처음 시작할때
    BattleAppearTalk, // 적이 등장했다고 알릴 때
    BattleChooseCommandTalk, // 어떤 행동을 할 지에 대한 대사 나올 때(사실상 플레이어 턴 시작)
    BattleChooseCommand, //커맨드 선택 할 때
    PlayerSetTarget, // 플레이어가 타겟을 설정할 때 
    PlayerResultTalk, // 플레이어의 행동 결과가 대사로 나올 때
    EnemySet, // 적의 턴을 확인하고 행동을 선택할 때
    EnemyResultTalk, // 적의 행동의 결과가 대사로 나올 때
    BattleClear, // 전투가 끝났을 때
    BattleEndTalk // 전투 종료 후 텍스트
}

public class BattleManager : MonoBehaviour
{
    [HideInInspector]
    public DungeonManager dungeonManager;
    [SerializeField]
    private GameObject enemyPanal;
    private GameObject[] enemyStatusWindow;
    [SerializeField]
    private GameObject commandWindow;
    private DungeonEnemyCharacter[] originalEnemyStatus;
    private DungeonEnemyCharacter[] enemyCharacter;
    [SerializeField] // 확인용
    private BattlePhase battlePhase;
    private int enemyActionCount = 0;
    private List<int> actionAbleEnemy = new List<int>();

    private int battleTarget = -1;
    [SerializeField] // 확인용
    private Skill useSkill = null;

    private bool isClear = false;

    void Start()
    {
        InitBattleManager();
    }

    private void InitBattleManager()
    {
        #region
        SetEnemyStatusWindow();

        if (this.gameObject.GetComponent<DungeonManager>() == true)
        {
            dungeonManager = this.gameObject.GetComponent<DungeonManager>();
        }
        else
        {
            Debug.LogWarning("게임오브젝트가 dungeonManager를 들고 있지 않습니다!");
        }     

        enemyActionCount = 0;
        #endregion
    }

    private void SetEnemyStatusWindow()
    {
        #region
        if (enemyPanal != null)
        {
            enemyStatusWindow = new GameObject[enemyPanal.transform.childCount];
            for (int i = 0; i < enemyPanal.transform.childCount; i++)
            {
                enemyStatusWindow[i] = enemyPanal.transform.GetChild(i).gameObject;
            }
            SetAllEnemyStatusWindowActiveFalse();
        }
        #endregion
    }

    private void SetAllEnemyStatusWindowActiveFalse()
    {
        #region
        for (int i = 0; i < enemyStatusWindow.Length; i++)
        {
            SetEnemyStatusWindow(i, false);
        }
        #endregion
    }

    private void SetEnemyStatusWindow(int num, bool mode)
    {
        #region
        if ((0 <= num) && (num < enemyStatusWindow.Length))
        {
            enemyStatusWindow[num].SetActive(mode);
        }
        else
        {
            Debug.LogWarning("SetEnemyStatusWindow에 잘못된 값이 들어왔습니다! 들어온 값 = " + num);
        }
        #endregion
    }

    private void SetCommandWindow(bool mode)
    {
        commandWindow.SetActive(mode);
    }

    public void BattleStart(EnemyGroup enemyGroup)
    {
        #region
        ChangeBattlePhase(BattlePhase.SetBattle);
        int enemyCount = enemyGroup.enemyNum.Length;
        enemyActionCount = 0;

        if (enemyCount > enemyStatusWindow.Length)
        {
            Debug.LogWarning("적의 수가 " + enemyStatusWindow.Length + "명보다 많습니다! 적의 수는 " + enemyCount);
            enemyCount = enemyStatusWindow.Length;
        }

        enemyCharacter = new DungeonEnemyCharacter[enemyCount];
        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            EnemyCharacter newEnemy = CharacterDatabaseManager.instance.GetEnemyCharacter(enemyGroup.enemyNum[i]);
            enemyCharacter[i].status = newEnemy.status;
            enemyCharacter[i].HP = newEnemy.status.maxHP;
            enemyCharacter[i].MP = newEnemy.status.maxMP;
            enemyCharacter[i].aiSkill = newEnemy.aiSkill;
            enemyCharacter[i].dropGold = newEnemy.dropGold;
            enemyCharacter[i].dropEXP = newEnemy.dropEXP;
        }

        originalEnemyStatus = new DungeonEnemyCharacter[enemyCharacter.Length];
        for (int i = 0; i < originalEnemyStatus.Length; i++)
        {
            originalEnemyStatus[i] = enemyCharacter[i];
        }
        UpdateEnemyStatus();
        for (int i = 0; i < originalEnemyStatus.Length; i++)
        {
            SetEnemyStatusWindow(i, true);
        }       

        actionAbleEnemy = new List<int>();
        actionAbleEnemy.Clear();
        useSkill = null;
        battleTarget = -1;
        SetEnemyStatusWindowButton(false);
        isClear = false;
        SetBattleAppearTalk();
        #endregion 
    }

    private void BattleClear()
    {
        isClear = true;
        dungeonManager.PlayVoice("BattleWin");
        ChangeBattlePhase(BattlePhase.BattleClear);
        SetBattleEndTalk();
    }

    private void BattleEnd()
    {
        #region
        SetAllEnemyStatusWindowActiveFalse();
        int AllDropGold = 0;
        int AllDropEXP = 0;

        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            if (enemyCharacter[i].isDead == true)
            {
                AllDropGold = AllDropGold + enemyCharacter[i].dropGold;
                AllDropEXP = AllDropEXP + enemyCharacter[i].dropEXP;
            }
        }
        battlePhase = BattlePhase.SetBattle;
        dungeonManager.IncreaseGold(AllDropGold);
        dungeonManager.IncreaseEXP(AllDropEXP);
        dungeonManager.BattleEnd();
        #endregion
    }

    private void UpdateEnemyStatus()
    {
        #region
        int deadCount = 0;
        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            enemyCharacter[i].status.maxHP = originalEnemyStatus[i].status.maxHP;
            enemyCharacter[i].HP = originalEnemyStatus[i].HP;
            enemyCharacter[i].status.maxMP = originalEnemyStatus[i].status.maxMP;
            enemyCharacter[i].MP = originalEnemyStatus[i].MP;
            enemyCharacter[i].status.ATK = originalEnemyStatus[i].status.ATK;
            enemyCharacter[i].status.MAK = originalEnemyStatus[i].status.MAK;
            enemyCharacter[i].status.DEF = originalEnemyStatus[i].status.DEF;
            enemyCharacter[i].status.MDF = originalEnemyStatus[i].status.MDF;
            enemyCharacter[i].status.INT = originalEnemyStatus[i].status.INT;
            enemyCharacter[i].status.STA = originalEnemyStatus[i].status.STA;

            if (originalEnemyStatus[i].HP <= 0)
            {
                originalEnemyStatus[i].isDead = true;
            }
            enemyCharacter[i].isDead = originalEnemyStatus[i].isDead;

            if (enemyCharacter[i].isDead == true)
            {
                deadCount++;
            }
        }

        if ((deadCount >= enemyCharacter.Length) && (isClear == false))
        {
            BattleClear();
        }

        UpdateEnemyStatusWindow();
        #endregion
    }

    private void UpdateEnemyStatusWindow()
    {
        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            enemyStatusWindow[i].GetComponent<EnemyStatusWindowController>().SetEnemyStatusWindow(enemyCharacter[i]);
        }
    }

    private void ChangeBattlePhase(BattlePhase newBattlePhase)
    {
        if (battlePhase != BattlePhase.BattleEndTalk)
        {
            battlePhase = newBattlePhase;
        }

        if (newBattlePhase != BattlePhase.SetBattle)
        {
            UpdateStatus();
        }
        
    }

    private void UpdateStatus()
    {
        dungeonManager.UpdateStatus();
        UpdateEnemyStatus();
    }

    public BattlePhase GetBattlePhase()
    {
        return battlePhase;
    }

    private void StartPlayerTurn()
    {
        SetCommandWindow(true);
        ChangeBattlePhase(BattlePhase.BattleChooseCommand);
    }

    private void StartEnemyTurn()
    {
        enemyActionCount = 0;
        actionAbleEnemy.Clear();

        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            if (enemyCharacter[i].isDead == false)
            {
                actionAbleEnemy.Add(i);
            }
            else
            {
                enemyActionCount = enemyActionCount + 1;
            }
        }

        CheckEnemyTurn();
    }

    private void CheckEnemyTurn()
    {
        ChangeBattlePhase(BattlePhase.EnemySet);
        if ((enemyActionCount >= enemyCharacter.Length) && (actionAbleEnemy.Count < 1))
        {
            SetBattleChooseCommandTalk(); //플레이어턴으로 돌리기
        }
        else
        {
            //여기에 ai 작성, 기본공격만 하도록 작성됨
            int turnCharacter = Random.Range(0, actionAbleEnemy.Count);
            UseSkill(SkillDatabaseManager.instance.GetSkill(0), enemyCharacter[actionAbleEnemy[turnCharacter]].status, dungeonManager.playerCharacter.status, true);
            actionAbleEnemy.RemoveAt(turnCharacter);
            enemyActionCount = enemyActionCount + 1;        
        }

    }

    //대화용
    #region
    private void SetText(string text)
    {
        dungeonManager.talkManager.TalkStart(text);
    }

    public void TalkEnd() // TalkManager에서 불러오는 거
    {
        switch (battlePhase)
        {
            case BattlePhase.BattleAppearTalk:
                SetBattleChooseCommandTalk();
                break;
            case BattlePhase.BattleChooseCommandTalk:
                StartPlayerTurn();
                break;
            case BattlePhase.PlayerResultTalk:
                StartEnemyTurn();
                break;
            case BattlePhase.PlayerSetTarget:
                break;
            case BattlePhase.EnemyResultTalk:
                CheckEnemyTurn();
                break;
            case BattlePhase.BattleEndTalk:
                BattleEnd();
                break;
            default:
                break;
        }
        UpdateStatus();
    }

    private void SetBattleAppearTalk()
    {
        ChangeBattlePhase(BattlePhase.BattleAppearTalk);
        SetText(enemyCharacter[0].status.name + "이(가) 나타났다!");
    }

    private void SetBattleChooseCommandTalk()
    {
        ChangeBattlePhase(BattlePhase.BattleChooseCommandTalk);
        SetText(dungeonManager.playerCharacter.status.name + "은(는) 무엇을 할까?");
    }

    private void SetCommandText(string text, bool isEnemy)
    {
        if (isClear == true)
        {
            return;
        }

        if (isEnemy == true)
        {
            ChangeBattlePhase(BattlePhase.EnemyResultTalk);
        }
        else
        {
            ChangeBattlePhase(BattlePhase.PlayerResultTalk);
        }

        SetText(text);
    }

    private void SetBattleEndTalk()
    {
        int AllDropGold = 0;
        int AllDropEXP = 0;

        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            if (enemyCharacter[i].isDead == true)
            {
                AllDropGold = AllDropGold + enemyCharacter[i].dropGold;
                AllDropEXP = AllDropEXP + enemyCharacter[i].dropEXP;
            }
        }

        ChangeBattlePhase(BattlePhase.BattleEndTalk);
        SetText(dungeonManager.playerCharacter.status.name + "은(는) " + AllDropGold +"만큼의 골드와 " + AllDropEXP + "만큼의 경험치를 얻었다.");
    }
    #endregion

    //스킬용
    #region
    public void UseSkill(Skill skill, CharacterStatus skillUser, CharacterStatus skillTarget, bool isEnemy)
    {
        switch (skill.skillType)
        {
            case SkillType.Attack:
                VoiceDatabaseManager.instance.PlayVoice(skillUser.voiceType, VoiceType.AttackSkill);
                int skillDamage = GetDamage(skill, skillUser, skillTarget) * -1;
                if(isEnemy == true)
                { 
                    dungeonManager.IncreaseHP(skillDamage);
                }
                else
                {
                    IncreaseEnemyHP(battleTarget, skillDamage);
                }
                SetCommandText(skillUser.name + "의 " + skill.skillName + "!! " + skillTarget.name + "은(는) " + skillDamage + "의 피해를 입었다!", isEnemy);
                break;
            default:
                SetCommandText("해당 스킬은 미구현 상태입니다!", isEnemy);
                break;
        }
        UpdateStatus();
        useSkill = null;
    }

    public int GetDamage(Skill skill, CharacterStatus attackerStatus, CharacterStatus targetStatus)
    {
        #region
        int defaultDamage = 0;

        if (skill != null)
        {
            float attackPower = skill.DPHATK * attackerStatus.ATK + skill.DPHMAK * attackerStatus.MAK + skill.DPHDEF * attackerStatus.DEF + skill.DPHMDF * attackerStatus.MDF + skill.DPHMaxHP * attackerStatus.maxHP + skill.DPHINT * attackerStatus.INT + skill.DPHMaxMP * attackerStatus.maxMP + skill.DPHSTA * attackerStatus.STA;
            float resistPower = skill.enemyATK * targetStatus.ATK + skill.enemyMAK * targetStatus.MAK + skill.enemyDEF * targetStatus.DEF + skill.enemyMDF * targetStatus.MDF + skill.enemyMaxHP * targetStatus.maxHP + skill.enemyINT * targetStatus.INT + skill.enemyMaxMP * targetStatus.maxMP + skill.enemySTA * targetStatus.STA;
            float varianceDamage = Random.Range(1.0f - skill.variance, 1.0f + skill.variance);
            float innocentDamage = attackPower - resistPower; 
        
            int damage = (int)(innocentDamage * varianceDamage) + skill.fixedValue;

            if(damage <= skill.fixedValue)
            {
                if(skill.fixedValue > 0)
                { 
                    Debug.Log("데미지는 " + skill.fixedValue);
                    return skill.fixedValue;                
                }
                else
                { 
                    Debug.Log("데미지는 " + defaultDamage);
                    return defaultDamage;
                }
            }
            else
            { 
                Debug.Log("데미지는 " + damage);
               return damage; 
            }     
        }
        else
        {
            return defaultDamage;
        }
        #endregion
    }

    public void IncreaseEnemyHP(int characternum, int increaseCount)
    {
        if ((characternum < 0) || (characternum >= originalEnemyStatus.Length))
        {
            Debug.LogWarning("잘못된 값이 들어왔습니다!");
            characternum = 0;
        }

        originalEnemyStatus[characternum].HP = originalEnemyStatus[characternum].HP + increaseCount;
        if (originalEnemyStatus[characternum].HP > originalEnemyStatus[characternum].status.maxHP)
        {
            originalEnemyStatus[characternum].HP = originalEnemyStatus[characternum].status.maxHP;
        }
        else if (originalEnemyStatus[characternum].HP < 0)
        {
           originalEnemyStatus[characternum].HP = 0;
        }
        UpdateStatus();
    }

    public void SetUseSkill(int skillNum)
    {
        useSkill = SkillDatabaseManager.instance.GetSkill(skillNum);
    }

    public void SetBattleTarget(int characternum)
    {
        if ((characternum < 0) || (characternum >= enemyCharacter.Length))
        {
            Debug.LogWarning("잘못된 값이 들어왔습니다!");
            characternum = 0;
        }
        battleTarget = characternum;

        SetEnemyStatusWindowButton(false);
        UseSkill(useSkill, dungeonManager.playerCharacter.status, enemyCharacter[battleTarget].status, false);
    }

    private void SetEnemyStatusWindowButton(bool mode)
    {
        for (int i = 0; i < enemyStatusWindow.Length; i++)
        {
            enemyStatusWindow[i].GetComponent<Button>().enabled = mode;
        }
    }
    #endregion

    //커멘드 버튼용
    private void SetTargetButton()
    {
        #region
        if (useSkill == null)
        {
            StartEnemyTurn(); 
            return;
        }
        SetEnemyStatusWindowButton(true);

        for (int i = 0; i < enemyCharacter.Length; i++)
        {
            if ((enemyCharacter[i].isDead == false) && (enemyStatusWindow[i].activeSelf == true))
            {
                enemyStatusWindow[i].GetComponent<Button>().interactable = true;
            }
            else
            {
                enemyStatusWindow[i].GetComponent<Button>().interactable = false;
            }
        }

        ChangeBattlePhase(BattlePhase.PlayerSetTarget);   
        SetText("누구에게 사용할까?");        
        SetCommandWindow(false);
        #endregion
    }


    public void AttackButton()
    {
        SetUseSkill(dungeonManager.playerCharacter.status.characterDefaultAttackNum);
        SetTargetButton();
    }
}
