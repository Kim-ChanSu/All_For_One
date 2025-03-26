using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCharacter 
{
    [SerializeField]
    private int playerCharacterIndex; //���ӸŴ��� ĳ���� ����Ʈ �ε���
    public CharacterStatus status;

    public void SetCharacterStatus(CharacterStatus newStatus)
    { 
        status = newStatus;
    }

    public void SetPlayerCharacterIndex(int num)
    { 
        playerCharacterIndex = num;
    }

    public int GetPlayerCharacterIndex()
    { 
        return  playerCharacterIndex;
    }
}
