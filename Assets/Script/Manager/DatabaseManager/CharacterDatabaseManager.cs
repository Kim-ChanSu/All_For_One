using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterStatus
{ 
    #region
    public string name;
    public Sprite face;
    public CharacterRank characterRank;
    public int characterUpgrade;
    public int upgradeMaxEXP;
    public int upgradeEXP;

    public CharacterIP characterIP;
    public CharacterVoiceType voiceType;

    public int maxHP;
    public int maxMP;
    public int ATK;
    public int MAK;
    public int DEF;
    public int MDF;
    public int INT;
    public int STA;  

    public int characterDefaultAttackNum;

    public StatusValue maxHPValue;
    public StatusValue maxMPValue;
    public StatusValue ATKValue;
    public StatusValue MAKValue;
    public StatusValue DEFValue;
    public StatusValue MDFValue;
    public StatusValue INTValue;
    public StatusValue STAValue;  
    [HideInInspector]
    public int characterIndex;
    #endregion
}

public enum CharacterRank
{
    #region
    D,
    C,
    B,
    A
    #endregion
}

public enum StatusValue
{
    #region
    F,
    E,
    D,
    C,
    B,
    A,
    S
    #endregion
}

public class CharacterDatabaseManager : MonoBehaviour
{
    public static CharacterDatabaseManager instance;
    [SerializeField]
    private Character[] playerCharacter;
    [SerializeField]
    private EnemyCharacter[] enemyCharacter;

    private void Awake() 
    {
        InitCharacterDatabaseManager();
    }

    private void InitCharacterDatabaseManager()
    { 
        // 싱글턴
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 CharacterDatabaseManager가 2개이상 존재합니다.");
            Destroy(this.gameObject);
        }
        #endregion
        #region
        for(int i = 0; i < playerCharacter.Length; i++)
        { 
            playerCharacter[i].status.characterIndex = i;
        }

        for(int i = 0; i < enemyCharacter.Length; i++)
        { 
            enemyCharacter[i].status.characterIndex = i;
        }
        #endregion
    }

    public Character GetPlayerCharacter(int num)
    { 
        if((num < playerCharacter.Length) && (0 <= num))
        {
            return playerCharacter[num];
        }
        else
        {
            Debug.LogWarning("GetPlayerCharacter에 잘못된 값이 들어왔습니다! 들어온 값은 " + num);
            return playerCharacter[0];
        }
    }

    public int GetPlayerCharacterLength()
    { 
        return playerCharacter.Length;
    }

    public EnemyCharacter GetEnemyCharacter(int num)
    { 
        if((num < enemyCharacter.Length) && (0 <= num))
        {
            return enemyCharacter[num];
        }
        else
        {
            Debug.LogWarning("GetPlayerCharacter에 잘못된 값이 들어왔습니다! 들어온 값은 " + num);
            return enemyCharacter[0];
        }        
    }
}
