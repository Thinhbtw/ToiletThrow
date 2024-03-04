using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelSelector : Singleton<LevelSelector>
{
    [SerializeField] bool testLevel;
    public GameObject levelHolder;
    public GameObject levelIcon;
    public GameObject thisCanvas;
    public int numberOfLevels = 50;
    public Vector2 iconSpacing;
    private Rect panelDimensions;
    private Rect iconDimensions;
    private int amountPerPage;
    private int currentLevelCount;
    [SerializeField] LevelLoader levelLoader;
    [SerializeField] ButtonManager buttonManager;
    [SerializeField] PageSwiper pageSwiper;

    [SerializeField] Button btnBack, btnForward;
    [SerializeField] Sprite imgBtnUnlock, starOn;

    [SerializeField] List<GameObject> list_ButtonInPanel;
    public int getPagePlayerisOn;
    bool checkIfFirstOpen = false;

    private void OnEnable()
    {
        if (!checkIfFirstOpen) return;
        ReloadUIButton();
        pageSwiper.MoveToCurrentLevel(getPagePlayerisOn);
    }

    // Start is called before the first frame update
    void Start()
    {
        panelDimensions = levelHolder.GetComponent<RectTransform>().rect;
        iconDimensions = levelIcon.GetComponent<RectTransform>().rect;
        amountPerPage = 18;
        int totalPages = Mathf.CeilToInt((float)numberOfLevels / amountPerPage);
        LoadPanels(totalPages);
        checkIfFirstOpen = true;

    }
    public void LoadPanels(int numberOfPanels)
    {
        GameObject panelClone = Instantiate(levelHolder) as GameObject;
        PageSwiper swiper = levelHolder.AddComponent<PageSwiper>();
        pageSwiper = swiper;
        swiper.totalPages = numberOfPanels;

        btnBack.onClick.AddListener(() =>
        {
            swiper.MoveToLeftPanel();
        });
        btnForward.onClick.AddListener(() =>
        {
            swiper.MoveToRightPanel();
        });

        for (int i = 1; i <= numberOfPanels; i++)
        {
            GameObject panel = Instantiate(panelClone) as GameObject;
            panel.transform.SetParent(thisCanvas.transform, false);
            panel.transform.SetParent(levelHolder.transform);
            panel.name = "Page-" + i;

            panel.GetComponent<RectTransform>().localPosition = new Vector2(panelDimensions.width * (i - 1), 0);
            swiper.AddPositionToListPanel(new Vector2(panelDimensions.width * (i - 1), 0), panel);
            SetUpGrid(panel);
            int numberOfIcons = i == numberOfPanels ? numberOfLevels - currentLevelCount : amountPerPage;
            LoadIcons(numberOfIcons, panel, imgBtnUnlock);

        }

        Destroy(panelClone);
       
    }
    public void SetUpGrid(GameObject panel)
    {
        GridLayoutGroup grid = panel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(iconDimensions.width, iconDimensions.height);
        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = iconSpacing;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = 6;
    }
    public void LoadIcons(int numberOfIcons, GameObject parentObject, Sprite btnUnLock)
    {
        for (int i = 1; i <= numberOfIcons; i++)
        {
            int num = currentLevelCount;
            currentLevelCount++;
            GameObject icon = Instantiate(levelIcon) as GameObject;
            icon.transform.SetParent(thisCanvas.transform, false);
            icon.transform.SetParent(parentObject.transform);
            icon.name = "Level " + i;
            icon.GetComponentInChildren<Text>().text = "" + currentLevelCount;
            icon.GetComponent<Button>().interactable = false;

            if (currentLevelCount > levelLoader.CountTotalLevel()) continue;
            icon.GetComponentInChildren<Text>().text = "" + currentLevelCount;
            icon.GetComponent<Button>().interactable = true;
            icon.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                levelLoader.InstantiateLevel(num);
                buttonManager.FromSelectLevelToGameplayCanvas();
            });

            list_ButtonInPanel.Add(icon);

            if (currentLevelCount - 1 > DATA.GetCurrentLevelPlay())
            {
                icon.GetComponent<Button>().interactable = false;
            }
            else
            {
                icon.GetComponent<Image>().sprite = btnUnLock;
            }

            if (testLevel) continue;
            //set hinh anh sao cho tung` level
            for (int j = 0; j < 3; j++)
            {
                int t = j;
                if (DATA.GetStarAtLevel(currentLevelCount - 1) > t)
                {
                    icon.transform.GetChild(1).GetChild(t).GetComponent<Image>().sprite = starOn;
                }
            }
        }
    }

    public void ReloadUIButton()
    {

        for (int i = 0; i < list_ButtonInPanel.Count; i++)
        {
            int num = i;
            if(num > DATA.GetCurrentLevelPlay())
            {
                list_ButtonInPanel[i].GetComponent<Button>().interactable = false;
            }
            else
            {
                list_ButtonInPanel[i].GetComponent<Image>().sprite = imgBtnUnlock;
                list_ButtonInPanel[i].GetComponent<Button>().interactable = true;
                
            }

            if (testLevel) continue;
            for (int j = 0; j < 3; j++)
            {
                int t = j;
                if (DATA.GetStarAtLevel(num) > t)
                {
                    list_ButtonInPanel[i].transform.GetChild(1).GetChild(t).GetComponent<Image>().sprite = starOn;
                }
            }
        }

        getPagePlayerisOn = Mathf.CeilToInt((float)(levelLoader.GetCurrentLevel() + 1) / amountPerPage);
    }

    public void MoveToPanel()
    {
        getPagePlayerisOn = Mathf.CeilToInt((float)(DATA.GetCurrentLevelPlay() + 1) / amountPerPage);
        pageSwiper.MoveToCurrentLevel(getPagePlayerisOn);
    }
        
}