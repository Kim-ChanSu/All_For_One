using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerCharacter 
{
    [SerializeField]
    private int playerCharacterIndex; //게임매니저 캐릭터 리스트 인덱스
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
