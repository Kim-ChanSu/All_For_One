using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterIP
{ 
    Ignore,
    All,
    ETC,
    RomanceOfTheThreeKingdoms,
    TheAgeOfBarbarians,
    AngelBeats,
    ACertainScientificRailgun,
    YuGiOh
}

public class IPLogoDatabaseManager : MonoBehaviour
{
    public static IPLogoDatabaseManager instance; 

    void Awake() 
    {
        // �̱���
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("���� IPLogoDatabaseManager 2���̻� �����մϴ�.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class CharacterIPLogo
    { 
        public CharacterIP characterIP;
        public Sprite Logo;
        public AudioClip BGMSE;
    }

    //[ArrayElementTitle("characterIP")]
    [SerializeField]
    private CharacterIPLogo[] characterIPLogo;

    public Sprite GetCharacterIPLogo(CharacterIP ip)
    {
        for(int i = 0; i < characterIPLogo.Length; i++)
        { 
            if(characterIPLogo[i].characterIP == ip)
            { 
                return  characterIPLogo[i].Logo;
            }
        }

        return  characterIPLogo[0].Logo;
    }

    public AudioClip GetCharacterIPSE(CharacterIP ip)
    { 
        for(int i = 0; i < characterIPLogo.Length; i++)
        { 
            if(characterIPLogo[i].characterIP == ip)
            { 
                return  characterIPLogo[i].BGMSE;
            }
        }

        return  characterIPLogo[0].BGMSE;        
    }

}
