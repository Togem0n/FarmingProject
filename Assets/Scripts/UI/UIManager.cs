using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    int UILayer;
    int SceneTransitionLayer;

    private bool pauseMenuOn = false;
    [SerializeField] private UIInventoryBar uiInventoryBar = null;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool PauseMenuOn { get => pauseMenuOn; set => pauseMenuOn = value; }

    protected override void Awake()
    {
        base.Awake();
        pauseMenu.SetActive(false);
    }

    private void Start()
    {
        UILayer = LayerMask.NameToLayer("UI");
        SceneTransitionLayer = LayerMask.NameToLayer("SceneTransition");
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenuOn)
            {
                DisablePauseMenu();
            }
            else
            {
                EnablePauseMenu();
            }
        }
    }

    public void DisablePauseMenu()
    {
        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItem();

        pauseMenuOn = false;
        Player.Instance.EnablePlayerInput();
        Time.timeScale = 1;
        pauseMenu.SetActive(false);
    }

    public void EnablePauseMenu()
    {
        uiInventoryBar.DestoryCurrentlyDraggedItem();

        uiInventoryBar.ClearCurrentlySelectedItems();


        pauseMenuOn = true;
        Player.Instance.DisablePlayerInput();
        Time.timeScale = 0;
        pauseMenu.SetActive(true);

        System.GC.Collect();

        HighLightButtonForSelectedTab();
    }

    private void Update()
    {
        PauseMenu();
    }

    private void HighLightButtonForSelectedTab()
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            }
            else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.disabledColor;
        button.colors = colors;
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = colors.pressedColor;
        button.colors = colors;
    }

    
    public void SwitchPauseMenuTab(int tabNum)
    {
        for(int i = 0; i < menuTabs.Length; i++)
        {
            if(i != tabNum)
            {
                menuTabs[i].SetActive(false);
            }
            else
            {
                menuTabs[i].SetActive(true);
            }
        }

        HighLightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Debug.Log("quit game;");
        Application.Quit();
    }

    #region hover mouse
    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    public bool IsPointerOverSceneTransitionElement()
    {
        return IsPointerOverSceneTransitionElement(GetEventSystemRaycastResults());
    }


    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    private bool IsPointerOverSceneTransitionElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == SceneTransitionLayer)
                return true;
        }
        return false;
    }


    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
    #endregion
}
