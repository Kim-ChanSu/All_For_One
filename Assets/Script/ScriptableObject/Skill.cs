using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Attack,
    Buff,
    Debuff
}

public enum SkillTarget
{
    Enemy,
    EnemyAll,
    Self,
    All
}

[CreateAssetMenu(menuName = "Skill")]
public class Skill : ScriptableObject
{
    public string skillName;
    //public Sprite skillIcon;
    [TextArea]
    public string skillExplain;
    public SkillType skillType;
    public SkillTarget skillTarget;

    public int skillHPCost;
    public int skillMPCost;

    //스킬계수
    public float DPHMaxHP;
    public float DPHMaxMP;
    public float DPHATK;
    public float DPHMAK;
    public float DPHDEF;
    public float DPHMDF;
    public float DPHINT;
    public float DPHSTA;

    //반감(방어력 등)
    public float enemyMaxHP;
    public float enemyHP;
    public float enemyMaxMP;
    public float enemyMP;
    public float enemyATK;
    public float enemyMAK;
    public float enemyDEF;
    public float enemyMDF;
    public float enemyINT;
    public float enemySTA;

    [Range(0.0f, 1.0f)]
    public float variance;    
    public int fixedValue;

}
