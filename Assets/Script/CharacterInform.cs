using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterInform : MonoBehaviour
{    
    protected MainManager mainManager;

    public void SetMainManager(MainManager newMainManager)
    {
        mainManager = newMainManager;
    }

    public abstract void ClearInform();

    public abstract void SelectCharacter(int num);

    public abstract void UnselectCharacter(int num);

    public abstract void CharacterInformButton();

    public abstract void CancleCharacterInformButton();
}
