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
        // �̱���
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� SkillDatabaseManager�� 2���̻� �����մϴ�.");
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
            Debug.LogWarning("GetPlayerCharacter�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + num);
            return skill[0];
        }
    }
}
