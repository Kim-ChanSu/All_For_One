using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyCharacter")]
public class EnemyCharacter : ScriptableObject
{
    public CharacterStatus status;
    public AiSkill[] aiSkill;
    public int dropGold;
    public int dropEXP;
}

[System.Serializable]
public struct AiSkill
{
    public int skillNum;
    public int skillUseFrequency;
}
