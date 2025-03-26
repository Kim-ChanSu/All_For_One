using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonDatabaseManager : MonoBehaviour
{
    public static DungeonDatabaseManager instance;
    [SerializeField]
    private Dungeon[] dungeon;

    private void Awake() 
    {
        InitDungeonDatabaseManager();
    }

    private void InitDungeonDatabaseManager()
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
        ResetDungeonClear();
    }

    private void ResetDungeonClear()
    {
        for (int i = 0; i < dungeon.Length; i++)
        {
            dungeon[i].isDungeonClear = false;
        }
    }

    public Dungeon GetDungeon(int num)
    { 
        if((num < dungeon.Length) && (0 <= num))
        {
            return dungeon[num];
        }
        else
        {
            Debug.LogWarning("GetDungeon에 잘못된 값이 들어왔습니다! 들어온 값은 " + num);
            return dungeon[0];
        }
    }

    public int GetDungeonLength()
    { 
        return dungeon.Length;
    }
}
