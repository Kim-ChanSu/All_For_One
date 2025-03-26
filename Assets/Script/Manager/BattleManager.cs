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
    SetBattle, //������ �� ó�� �����Ҷ�
    BattleAppearTalk, // ���� �����ߴٰ� �˸� ��
    BattleChooseCommandTalk, // � �ൿ�� �� ���� ���� ��� ���� ��(��ǻ� �÷��̾� �� ����)
    BattleChooseCommand, //Ŀ�ǵ� ���� �� ��
    PlayerSetTarget, // �÷��̾ Ÿ���� ������ �� 
    PlayerResultTalk, // �÷��̾��� �ൿ ����� ���� ���� ��
    EnemySet, // ���� ���� Ȯ���ϰ� �ൿ�� ������ ��
    EnemyResultTalk, // ���� �ൿ�� ����� ���� ���� ��
    BattleClear, // ������ ������ ��
    BattleEndTalk // ���� ���� �� �ؽ�Ʈ
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
    [SerializeField] // Ȯ�ο�
    private BattlePhase battlePhase;
    private int enemyActionCount = 0;
    private List<int> actionAbleEnemy = new List<int>();

    private int battleTarget = -1;
    [SerializeField] // Ȯ�ο�
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
            Debug.LogWarning("���ӿ�����Ʈ�� dungeonManager�� ��� ���� �ʽ��ϴ�!");
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
            Debug.LogWarning("SetEnemyStatusWindow�� �߸��� ���� ���Խ��ϴ�! ���� �� = " + num);
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
            Debug.LogWarning("���� ���� " + enemyStatusWindow.Length + "���� �����ϴ�! ���� ���� " + enemyCount);
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
            SetBattleChooseCommandTalk(); //�÷��̾������� ������
        }
        else
        {
            //���⿡ ai �ۼ�, �⺻���ݸ� �ϵ��� �ۼ���
            int turnCharacter = Random.Range(0, actionAbleEnemy.Count);
            UseSkill(SkillDatabaseManager.instance.GetSkill(0), enemyCharacter[actionAbleEnemy[turnCharacter]].status, dungeonManager.playerCharacter.status, true);
            actionAbleEnemy.RemoveAt(turnCharacter);
            enemyActionCount = enemyActionCount + 1;        
        }

    }

    //��ȭ��
    #region
    private void SetText(string text)
    {
        dungeonManager.talkManager.TalkStart(text);
    }

    public void TalkEnd() // TalkManager���� �ҷ����� ��
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
        SetText(enemyCharacter[0].status.name + "��(��) ��Ÿ����!");
    }

    private void SetBattleChooseCommandTalk()
    {
        ChangeBattlePhase(BattlePhase.BattleChooseCommandTalk);
        SetText(dungeonManager.playerCharacter.status.name + "��(��) ������ �ұ�?");
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
        SetText(dungeonManager.playerCharacter.status.name + "��(��) " + AllDropGold +"��ŭ�� ���� " + AllDropEXP + "��ŭ�� ����ġ�� �����.");
    }
    #endregion

    //��ų��
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
                SetCommandText(skillUser.name + "�� " + skill.skillName + "!! " + skillTarget.name + "��(��) " + skillDamage + "�� ���ظ� �Ծ���!", isEnemy);
                break;
            default:
                SetCommandText("�ش� ��ų�� �̱��� �����Դϴ�!", isEnemy);
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
                    Debug.Log("�������� " + skill.fixedValue);
                    return skill.fixedValue;                
                }
                else
                { 
                    Debug.Log("�������� " + defaultDamage);
                    return defaultDamage;
                }
            }
            else
            { 
                Debug.Log("�������� " + damage);
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
            Debug.LogWarning("�߸��� ���� ���Խ��ϴ�!");
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
            Debug.LogWarning("�߸��� ���� ���Խ��ϴ�!");
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

    //Ŀ��� ��ư��
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
        SetText("�������� ����ұ�?");        
        SetCommandWindow(false);
        #endregion
    }


    public void AttackButton()
    {
        SetUseSkill(dungeonManager.playerCharacter.status.characterDefaultAttackNum);
        SetTargetButton();
    }
}
