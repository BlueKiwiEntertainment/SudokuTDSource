using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BuildingUIScript : MonoBehaviour
{
    public static BuildingUIScript instance;

    public GameObject SelectedTile;
    public GameObject Dummy1;
    public GameObject Dummy2;


    public GameObject MainMenu;
    public GameObject BuildingUI;
    public DepthOfField DepthOfField;


    public GameObject SelectedObj;
    public int ObjPrice;
  

    public GameObject HonourAndTimer;
    public GameObject GameOverScreen;
    public GameObject VictoryScreen;

    public GameObject Tips;
    public GameObject Nonsense;


    private void Awake()
    {
        BuildingUIScript.instance = this;
    }

    public void PleaseBuild()
    {
        Build(SelectedObj);
    }
    public void Build(GameObject obj)
    {
        SelectedTile.GetComponent<GridTile>().Build(obj);
    }

   

   public void SelectDummy1()
    {
        
        ObjPrice = 50;
        if(ObjPrice > GameManager.instance.Gold)
        {
            SelectedObj = null;
            FindObjectOfType<AudioManager>().Play("Error");
        }
        else
        {
            SelectedObj = Dummy1;
            FindObjectOfType<AudioManager>().Play("Sold");
            GameManager.instance.GoldHandler(-ObjPrice);
        }


    }
    public void SelectDummy2()
    {
        ObjPrice = 100;
        if (ObjPrice > GameManager.instance.Gold)
        {
            SelectedObj = null;
            FindObjectOfType<AudioManager>().Play("Error");
        }
        else
        {
            SelectedObj = Dummy2;
            FindObjectOfType<AudioManager>().Play("Sold");
            GameManager.instance.GoldHandler(-ObjPrice);
        }
    }
    public void SelectAgain()
    {
        if(ObjPrice < GameManager.instance.Gold)
        {
            FindObjectOfType<AudioManager>().Play("Sold");
            GameManager.instance.GoldHandler(-ObjPrice);
        }
        else
        {
            SelectedObj = null;
            FindObjectOfType<AudioManager>().Play("Error");
        }
    }

    public void ClearSelection()
    {
        SelectedObj = null;
    }


    public void RunningAroundGui()
    {

    }

    public void BuildingGui()
    {
        HonourAndTimer.SetActive(true);
        MainMenu.SetActive(false);
        BuildingUI.SetActive(true);
        GameOverScreen.SetActive(false);
    }

    public void GameOverGui()
    {
        MainMenu.SetActive(false);
        BuildingUI.SetActive(false);
        HonourAndTimer.SetActive(false);
        GameOverScreen.SetActive(true);
    }

    public void SelectingGui()
    {
        MainMenu.SetActive(false);
        BuildingUI.SetActive(false);
    }

    public void MainMenuGui()
    {
        MainMenu.SetActive(true);
        BuildingUI.SetActive(false);
        HonourAndTimer.SetActive(false);
        //DepthOfField = GameObject.Find("GlobalVolume").GetComponent<Volume>().GetComponent<DepthOfField>();



        //DepthOfField.focusDistance.SetValue(0.1f);
        GameManager.instance.GameState = GameManager.GameStates.MainMenu;
    }


    public void OnPlayButton()
    {
        BuildingGui();
        GameManager.instance.BuildingSudoku();
        FindObjectOfType<AudioManager>().StopPlay("MainMenu");


    }

    public void VictoryGUi()
    {
        MainMenu.SetActive(false);
        BuildingUI.SetActive(false);
        HonourAndTimer.SetActive(false);
        VictoryScreen.SetActive(true);
    }


    public void OnTipsButton()
    {
        Tips.SetActive(true);
        Nonsense.SetActive(false);
    }
    
}
