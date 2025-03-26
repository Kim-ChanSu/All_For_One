using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusWindowController : MonoBehaviour
{
    [SerializeField]
    private GameObject characterFaceImage;
    [SerializeField]
    private GameObject characterNameText;
    [SerializeField]
    private GameObject characterStatusText;
    [SerializeField]
    private GameObject characterHPImage;
    [SerializeField]
    private GameObject characterHPText;
    [SerializeField]
    private GameObject characterMPImage;
    [SerializeField]
    private GameObject characterMPText;

    public void SetEnemyStatusWindow(DungeonEnemyCharacter enemyCharacter)
    {
        characterFaceImage.GetComponent<Image>().sprite = enemyCharacter.status.face;
        characterNameText.GetComponent<Text>().text = "[ " + enemyCharacter.status.name + " ]";
        if (enemyCharacter.isDead == true)
        {
            characterStatusText.GetComponent<Text>().text = "상태 : 기절";
        }
        else
        {
            characterStatusText.GetComponent<Text>().text = "상태 : 정상";
        }     

        characterHPImage.GetComponent<Image>().fillAmount = (float)enemyCharacter.HP / (float)enemyCharacter.status.maxHP;
        characterHPText.GetComponent<Text>().text = enemyCharacter.HP + " / " + enemyCharacter.status.maxHP;
        characterMPImage.GetComponent<Image>().fillAmount = (float)enemyCharacter.MP / (float)enemyCharacter.status.maxMP;
        characterMPText.GetComponent<Text>().text = enemyCharacter.MP + " / " + enemyCharacter.status.maxMP;

    }
}
