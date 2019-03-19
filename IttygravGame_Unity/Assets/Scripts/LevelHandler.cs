using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelHandler : MonoBehaviour {

    public string LevelName;
    public string NextLevel;

    public GameObject LevelTitle;
    public GameObject WinnerSplash;
    public float TitleWait = 2.0f;
    public float TitleDuration = 5.0f;
    private float levelStartTime = 0;

    private bool LevelComplete = false;

    public LevelInfo[] Levels;
    public float ButtonStart = -15;
    public float ButtonSpacing = -40;
    public GameObject MenuPanel;
    public GameObject LevelButtonPrefab;
    private bool displayMenu = false;

    public bool ShowArrow = true;
    public bool ShowMiniMap = true;
    private GameObject miniMap;

    // called first
    void OnEnable()
    {
        //Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // called second
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        levelStartTime = Time.fixedTime;
        Text levelTitleText = null;
        if(LevelTitle ) levelTitleText = LevelTitle.transform.GetChild(0).GetComponent<Text>();
        if(levelTitleText) levelTitleText.text = LevelName;

        if(MenuPanel) MenuPanel.SetActive(false);

        ShowArrowToggle(ShowArrow);
        miniMap = null;
        ShowMiniMapToggle(ShowMiniMap);
    }

    private void Start()
    {
        LevelHandler lh = GameObject.FindGameObjectWithTag("LevelHandler").GetComponent<LevelHandler>();
        if(lh && lh.gameObject != gameObject)
        {
            Destroy(gameObject);
        }
        else
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }
        

        //levelStartTime = Time.fixedTime;
        //LevelTitle.transform.GetChild(0).GetComponent<Text>().text = LevelName;

        //MenuPanel.SetActive(false);

        for(int i = 0; i < Levels.Length; i+=1)
        {
            GameObject btnObject = GameObject.Instantiate(LevelButtonPrefab, MenuPanel.transform.position, Quaternion.identity);
            btnObject.transform.GetChild(0).GetComponent<Text>().text = Levels[i].LevelTitle;
            btnObject.transform.SetParent(MenuPanel.transform);
            float PosY = ButtonStart + ButtonSpacing * i + MenuPanel.GetComponent<RectTransform>().rect.height / 2;
            btnObject.GetComponent<RectTransform>().localPosition = new Vector3(0, PosY ,0);
            Levels[i].SetButton(btnObject.GetComponent<Button>());
            
        }
    }

    private void Update()
    {
        handleLevelTitle();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            displayMenu = !displayMenu;
            MenuPanel.SetActive(displayMenu);
        }
    }

    private void handleLevelTitle()
    {
        if (Time.fixedTime - levelStartTime > TitleWait + TitleDuration) {
            LevelTitle.SetActive(false);
        }
        else if(Time.fixedTime - levelStartTime > TitleWait)
        {
            LevelTitle.SetActive(true);
        }
    }

    public void LoadLevel()
    {
        LevelInfo li = getLevel(SceneManager.GetActiveScene().name);
        setupNextLevel(li);
        LevelName = getLevel(li.NextLevel).LevelTitle;
        SceneManager.LoadScene(NextLevel);
    }

    public void ReloadLoadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LevelButtonClick(Button button)
    {
        for(int i = 0; i < Levels.Length; i += 1)
        {
            if(Levels[i].GetButton() == button)
            {
                setupNextLevel(Levels[i]);   
            }
        }
    }

    private void setupNextLevel(LevelInfo level)
    {
        //NextLevel = getLevel(level.NextLevel).NextLevel;
        LevelName = level.LevelTitle;
        NextLevel = level.NextLevel;
        //LevelName = level.LevelTitle;
        SceneManager.LoadScene(level.SceneName);
    }

    private LevelInfo getLevel(string sceneName)
    {
        LevelInfo li = null;
        foreach(LevelInfo l in Levels)
        {
            if (l.SceneName == sceneName) li = l;
        }

        return li;
    }

    public void LeverTriggered(bool isTriggered)
    {
        LevelComplete = isTriggered;
        WinnerSplash.SetActive(LevelComplete);
    }

    public void ShowArrowToggle(bool state)
    {
        ShowArrow = state;
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().DisplayArrow = ShowArrow;
    }
    
    public void ShowMiniMapToggle(bool state)
    {
        ShowMiniMap = state;
        if(!miniMap) miniMap = GameObject.FindGameObjectWithTag("MiniMap");
        if (miniMap)
        {
            miniMap.SetActive(ShowMiniMap);
        }
        
    }
}

[System.Serializable]
public class LevelInfo
{
    public string SceneName;
    public string LevelTitle;
    public string NextLevel;
    private Button button;

    public void SetButton(Button button)
    {
        this.button = button;
    }

    public Button GetButton()
    {
        return button;
    }
}