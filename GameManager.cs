using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject cameraObj;
    public GameObject cameraOffsetObj;
    public GameObject tileMap;
    public bool SelectionAllowed = true;
    public GameObject newCamera;
    public Transform EnemyDestination;
    public int RoundNumber = 0;
    public TextMeshProUGUI FinalTime;
    

    [Header("Attributes")]
    public int Round;
    public int Honour;
    public int Gold = 470;
    public TextMeshProUGUI GoldField;

    [Header("Rounds")]
    public GameObject Round1Parent;
    public GameObject Rount2Parent;
    public GameObject Round3Parent;
    public GameObject Round4Parent;
    public GameObject Round5Parent;
    public GameObject BruteNPC;
    public int AliveEnemies = 0;
    float timePassed;
    string timer;
    public TextMeshProUGUI Timer;
    public float BuildTimer = 60;
    string buildTimer;
    public TextMeshProUGUI buildTimerText;
    public TextMeshProUGUI HonourTMP;



    public List<GameObject> GameObjectsToReactivate;

    public enum GameStates
    {
        MainMenu,
        Building,
        Selecting,
        RunningAround,
    }

    public GameStates GameState = GameStates.Building;


    private void Awake()
    {
        GameManager.instance = this;
    }

    private void Start()
    {
        //BeginRound(Round1Parent);
        BuildingUIScript.instance.MainMenuGui();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; 
            SceneManager.LoadScene(0);
            
        }

        GoldField.text = Gold.ToString();


        if (GameState == GameStates.RunningAround || GameState == GameStates.Selecting)
        {
            CountingTime();
            WriteTime(Timer, timer);
        }

        if(GameState == GameStates.Building)
        {
            BuildTimeDown();
            WriteTime(buildTimerText, buildTimer);
            if(BuildTimer <= 0)
            {
                BuildTimeOver();
            }
            if(Input.GetAxis("Jump") != 0)
            {
                BuildTimeOver();
            }
            
        }


        UpdateHonourTMP();
        
    }

    public void RunningSudoku()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        newCamera.SetActive(true);
        
    }
    public void RunningStarter()
    {
        BuildingUIScript.instance.RunningAroundGui();
        GameState = GameStates.RunningAround;
        tileMap.SetActive(false);
        

        SelectionAllowed = false;
        FindObjectOfType<AudioManager>().StopPlay("Building");
        if(!FindObjectOfType<AudioManager>().IsPlaying("Theme"))
        {
            FindObjectOfType<AudioManager>().Play("Theme");
        }
        
    }

    public void SelectingSudoku()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        GameState = GameStates.Selecting;
        SelectionAllowed = true;
        tileMap.SetActive(false);
        BuildingUIScript.instance.SelectingGui();

        foreach (GameObject item in GameObject.FindGameObjectsWithTag("tooltep"))
        {
            Destroy(item);
        }
        FindObjectOfType<AudioManager>().StopPlay("Building");
        if (!FindObjectOfType<AudioManager>().IsPlaying("Theme"))
        {
            FindObjectOfType<AudioManager>().Play("Theme");
        }
    }

    public void BuildingSudoku()
    {
        FindObjectOfType<PlayerController1>()?.Sudoku();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        BuildingUIScript.instance.BuildingGui();
        tileMap.SetActive(true);
        GameState = GameStates.Building;
        SelectionAllowed = false;
        FindObjectOfType<AudioManager>().StopPlay("Theme");
        FindObjectOfType<AudioManager>().Play("Building");


    }


    public void ReactivateObjs()
    {
        foreach (GameObject item in GameObjectsToReactivate)
        {
            item.SetActive(true);
        }
    }

    public void BeginRound(GameObject parent)
    {

        Transform[] spawnpoints = parent.GetComponentsInChildren<Transform>();
        foreach (Transform item in spawnpoints)
        {
            Instantiate(BruteNPC, item.position, item.rotation);
            AliveEnemies++;
        }
    }

    public void CountingTime()
    {
        timePassed += Time.deltaTime;
        float minutes = Mathf.FloorToInt(timePassed / 60);
        float seconds = Mathf.FloorToInt(timePassed % 60);
        float milliSeconds = (timePassed % 1) * 1000;

        timer = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliSeconds);
    }

    public void WriteTime(TextMeshProUGUI tmp, string timestring)
    {
        tmp.text = timestring;
    }

    public void BuildTimeDown()
    {
        BuildTimer -= Time.deltaTime;
        //float minutes = Mathf.FloorToInt(BuildTimer / 60);
        float seconds = Mathf.FloorToInt(BuildTimer % 60);

        buildTimer = string.Format("{0:00}", seconds);
    }

    public void BuildTimeOver()
    {
        GameState = GameStates.Selecting;
        BuildTimer = 60;
        RoundSelector();
        SelectingSudoku();
        
    }

    public void HonourHandler(int value)
    {
        Honour -= value;
        if(Honour <= 0 )
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        BuildingUIScript.instance.GameOverGui();
        FindObjectOfType<AudioManager>().StopPlay("Theme");

        foreach (EnemyBruteScript item in FindObjectsOfType<EnemyBruteScript>())
        {
            Destroy(item.gameObject);
        }
        Destroy(FindObjectOfType<PlayerController1>()?.gameObject);
        cameraObj.SetActive(true);
        cameraObj.GetComponent<AudioListener>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void UpdateHonourTMP()
    {
        string sHonour = Honour.ToString();
        HonourTMP.text = sHonour;

    }


    public void RoundSelector()
    {

        switch(RoundNumber)
        {
            case 0:
                BeginRound(Round1Parent);
                return;
            case 1:
                BeginRound(Rount2Parent);
                return;
            case 2:
                BeginRound(Round3Parent);
                return;
            case 3:
                BeginRound(Round4Parent);
                return;
            case 4:
                BeginRound(Round5Parent);
                return;
            default:
                return;




        }



    }


    public void IDied()
    {
        AliveEnemies--;
        if(AliveEnemies == 0)
        {
            if (RoundNumber < 4)
            {
                RoundEnd();
            }
            else
            {
                GameEnd();
            }
        }

    }

    public void RoundEnd()
    {
        RoundNumber++;
        BuildingSudoku();
        ReactivateObjs();
    }

    public void GameEnd()
    {
        FinalTime.text = timer;
        BuildingUIScript.instance.VictoryGUi();

        FindObjectOfType<AudioManager>().StopPlay("Theme");

        foreach (EnemyBruteScript item in FindObjectsOfType<EnemyBruteScript>())
        {
            Destroy(item.gameObject);
        }
        Destroy(FindObjectOfType<PlayerController1>()?.gameObject);
        cameraObj.SetActive(true);
        cameraObj.GetComponent<AudioListener>().enabled = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }


    public void GoldHandler(int ammount)
    {
        Gold += ammount;
    }
}
