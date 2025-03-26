using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterVoiceType
{
    None,
    SimYoung,
    ZhaoYun,
    ZhugeLiang,
    InubashiriMomiji,
    RemiliaScarlet,
    MisakaMikito,
    TachibanaKanade,
    Mordred,
    KaibaSeto,
    MutoYugi,
    Shana,
    Vivy,
    Corrin
}

public enum VoiceType
{
    Select,
    GetCharacter,
    AttackSkill,
    BuffSkill,
    DeBuffSkill,
    BattleWin,
    Die
}

public class VoiceDatabaseManager : MonoBehaviour
{
    public static VoiceDatabaseManager instance; 

    void Awake() 
    {
        // 싱글톤
        #region
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("씬에 VoiceDatabaseManager가 2개이상 존재합니다.");
            Destroy(gameObject);
        }
        #endregion
    }

    [System.Serializable]
    private class GameVoice
    { 
        public CharacterVoiceType characterVoiceType;
        public AudioClip[] select;     
        public AudioClip[] getCharacter;     
        public AudioClip[] attackSkill;     
        public AudioClip[] buffSkill;     
        public AudioClip[] deBuffSkill;     
        public AudioClip[] battleWin;     
        public AudioClip[] die;     
    }

    //[ArrayElementTitle("characterVoiceType")]
    [SerializeField]
    private GameVoice[] Voice;



    public void PlayVoice(CharacterVoiceType character, VoiceType voicetype) 
    {  
        if(character == CharacterVoiceType.None)
        { 
            return;    
        }

       PlayGameVoice(FindVoice(character),voicetype);
    } 

    private GameVoice FindVoice(CharacterVoiceType character)
    { 
        #region
        int check = 0;
        for(check = 0; check <= Voice.Length; check++)
        {
            if(check < Voice.Length)
            { 
                if(Voice[check].characterVoiceType  == character)
                {                 
                    break;
                }
            }
        }
        if(check < Voice.Length)
        {  
            return Voice[check];
        }
        else
        { 
            Debug.LogWarning("일치하는 캐릭터타입이 없습니다!");
            return Voice[0];
        }
        #endregion
    }

    private void PlayGameVoice(GameVoice voice, VoiceType voiceType)
    {
        #region
         switch(voiceType)
        {
            case VoiceType.Select:
                if(voice.select.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.select[Random.Range(0,voice.select.Length)]);
                break;
            case VoiceType.GetCharacter:
                if(voice.getCharacter.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.getCharacter[Random.Range(0,voice.getCharacter.Length)]);
                break;
            case VoiceType.AttackSkill:
                if(voice.attackSkill.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.attackSkill[Random.Range(0,voice.attackSkill.Length)]);
                break;
            case VoiceType.BuffSkill:
                if(voice.buffSkill.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.buffSkill[Random.Range(0,voice.buffSkill.Length)]);
                break;
            case VoiceType.DeBuffSkill:
                if(voice.deBuffSkill.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.deBuffSkill[Random.Range(0,voice.deBuffSkill.Length)]);
                break;
            case VoiceType.BattleWin:
                if(voice.battleWin.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.battleWin[Random.Range(0,voice.battleWin.Length)]);
                break;
            case VoiceType.Die:
                if(voice.die.Length < 1)
                { 
                    Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType + " / " + voiceType);
                    return;    
                }
                GameManager.instance.PlayVoice(voice.die[Random.Range(0,voice.die.Length)]);
                break;
        }       
        #endregion
    }

    public void PlayRandomVoice(CharacterVoiceType character)
    {
        if(character == CharacterVoiceType.None)
        { 
            return;    
        }

       PlayRandomGameVoice(FindVoice(character));
    }

    private void PlayRandomGameVoice(GameVoice voice)
    {
        #region
        List<AudioClip> randomVoice = new List<AudioClip>();

        for(int i = 0; i < voice.select.Length; i++)
        {
            randomVoice.Add(voice.select[i]);
        }
        for(int i = 0; i < voice.getCharacter.Length; i++)
        {
            randomVoice.Add(voice.getCharacter[i]);
        }
        for(int i = 0; i < voice.attackSkill.Length; i++)
        {
            randomVoice.Add(voice.attackSkill[i]);
        }
        for(int i = 0; i < voice.buffSkill.Length; i++)
        {
            randomVoice.Add(voice.buffSkill[i]);
        }
        for(int i = 0; i < voice.deBuffSkill.Length; i++)
        {
            randomVoice.Add(voice.deBuffSkill[i]);
        }
        for(int i = 0; i < voice.battleWin.Length; i++)
        {
            randomVoice.Add(voice.battleWin[i]);
        }
        for(int i = 0; i < voice.die.Length; i++)
        {
            randomVoice.Add(voice.die[i]);
        }

        if (randomVoice.Count > 0)
        {
            GameManager.instance.PlayVoice(randomVoice[Random.Range(0,randomVoice.Count)]);           
        }
        else
        {
            Debug.LogWarning("보이스가 설정 되어 있지 않습니다! " + voice.characterVoiceType);
        }
        #endregion
    }

}
