using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectDungeonController : MonoBehaviour
{
    private Dungeon dungeon;
    private MainManager mainManager;
    [SerializeField]
    private GameObject dungeonImage;
    [SerializeField]
    private GameObject dungeonNameText;
    [SerializeField]
    private GameObject dungeonDifficultyText;

    public void SetSelectDungeon(Dungeon newDungeon, MainManager newMainManager)
    { 
        if(newMainManager == null)
        { 
            Debug.LogWarning("메인 매니저가 없습니다!");
            Destroy(this.gameObject);
            return;
        }

        dungeon = newDungeon;
        mainManager = newMainManager;

        dungeonImage.GetComponent<Image>().sprite = dungeon.selectDungeonImage;
        dungeonNameText.GetComponent<Text>().text = dungeon.dungeonName;
        SetDungeonDifficultyText(dungeon.dungeonDifficulty);
    }

    public void SelectDungeon()
    { 
        if(mainManager == null)
        { 
            Debug.LogWarning("메인 매니저가 없습니다!");
            Destroy(this.gameObject);
            return;
        }        

        mainManager.SetSelectDungeon(dungeon);
    }

    private void SetDungeonDifficultyText(int dungeonDifficulty)
    {
        string difficultyText = "";
        int fullStarTextNum = dungeonDifficulty / 2;

        for (int i = 0; i < fullStarTextNum; i++)
        {
            difficultyText += "★";
        }

        if (dungeonDifficulty % 2 == 1)
        {
            difficultyText += "☆";
        }

        dungeonDifficultyText.GetComponent<Text>().text = "난이도 : " + difficultyText;
    }
}
