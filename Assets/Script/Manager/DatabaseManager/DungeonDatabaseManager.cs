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
        // �̱���
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� CharacterDatabaseManager�� 2���̻� �����մϴ�.");
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
            Debug.LogWarning("GetDungeon�� �߸��� ���� ���Խ��ϴ�! ���� ���� " + num);
            return dungeon[0];
        }
    }

    public int GetDungeonLength()
    { 
        return dungeon.Length;
    }
}
