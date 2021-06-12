using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private static GameController _instance;

    public static GameController Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #region Public Variables

    public Text healthText;
    public RectTransform gameScreen;
    public RectTransform healthPanel;
    public Text winCondition;
    public GameObject[] restartPoints;
    public GameObject[] restartPointsEnemies;
    public GameObject[] enemiesRobots;
    public GameObject playerCharacter;
    public int healthRobots = 3;
    #endregion

    #region Private Variables
    [SerializeField]
    int health = 3;

    int countRobots;
    #endregion 

    private void Start()
    {
        Restart();
        countRobots = enemiesRobots.Length;
    }

    private void Update()
    {
        healthText.text = health.ToString();
    }
    public void Win()
    {
        if(health > 0 && countRobots <= 0)
        {
            healthPanel.gameObject.SetActive(false);
            gameScreen.gameObject.SetActive(true);
            winCondition.text = "Você venceu! =)";
        }
    }

    public void Lose()
    {
            healthPanel.gameObject.SetActive(false);
            gameScreen.gameObject.SetActive(true);
            winCondition.text = "Você perdeu! =(";
    }

    public void Health()
    {
        health--;
        if(health <= 0)
        {
            Lose();
        }
    }

    public void Restart()
    {
        playerCharacter.transform.position = restartPoints[Random.Range(0, 3)].transform.position;
        countRobots = enemiesRobots.Length;
        health = 3;
        healthRobots = 3;
        gameScreen.gameObject.SetActive(false);
        healthPanel.gameObject.SetActive(true);

        for (int i = 0; i < enemiesRobots.Length; i++)
        {
            enemiesRobots[i].gameObject.SetActive(true);
            enemiesRobots[i].transform.position = restartPointsEnemies[Random.Range(0, 4)].transform.position;
        }
    }

    public void RobotDeath()
    {
        countRobots--;
    }
}
