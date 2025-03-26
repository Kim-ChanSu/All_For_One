using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DunGeonSpecialEnemy
{
    public EnemyGroup enemyGroup;
    public AudioClip enemyBattleBGM;
    public int enemyMeetCount;
}

[CreateAssetMenu(menuName = "Dungeon")]
public class Dungeon : ScriptableObject
{
    public string dungeonName;
    [TextArea]
    public string dungeonExplain;
    public Sprite dungeonBackground;
    public int dungeonLength;
    public DunGeonSpecialEnemy[] dungeonSpecialEnemy;
    public EnemyGroup[] enemyGroup;
    public AudioClip dungeonBGM;
    public AudioClip dungeonBattleBGM;

    public int dungeonClearGold;
    public Sprite selectDungeonImage;
    public int dungeonDifficulty;

    public int dungeonNeedGameProgress;
    public bool isDungeonClear;
}
