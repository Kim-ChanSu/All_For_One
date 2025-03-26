using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDatabaseManager : MonoBehaviour
{
    public static SkillDatabaseManager instance;
    [SerializeField]
    private Skill[] skill;

    private void Awake() 
    {
        InitSkillDatabaseManager();
    }

    private void InitSkillDatabaseManager()
    { 
        // 싱글턴
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 SkillDatabaseManager가 2개이상 존재합니다.");
            Destroy(this.gameObject);
        }
        #endregion
    }

    public Skill GetSkill(int num)
    {
        if((num < skill.Length) && (0 <= num))
        {
            return skill[num];
        }
        else
        {
            Debug.LogWarning("GetPlayerCharacter에 잘못된 값이 들어왔습니다! 들어온 값은 " + num);
            return skill[0];
        }
    }
}
