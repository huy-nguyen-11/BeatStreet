using UnityEngine;

public enum DevPopupType
{
    Landscape, LandscapeMini, Portrait, PortraitMini
}

public class MG_DevGUI_Menu : MonoBehaviour
{
    public DevPopupType PopupType;

    private MG_Interface MG;

    private bool checkHasMG;
    private bool checkDevMode;
    private bool checkPopupDev;

    private bool checkRewardLoaded;
    private bool checkLogViewer;

    private int width;
    private int height;

    private int fontSize = 24;

    public Vector2 btnDevPos;
    public Vector2 popupDevPos;

    public LevelControllerMain LevelControllerMain;

    // TODO: Rect For Landscape Screen
    private Rect btnDevRect = new Rect(0, 100, 80, 80);
    private Rect popupDevRect = new Rect(0, 0, 600, 400);
    private Rect btnExitRect = new Rect(240, 310, 120, 50);
    private Rect btnPopupRect_11 = new Rect(60, 50, 120, 50);
    private Rect btnPopupRect_12 = new Rect(240, 50, 120, 50);
    private Rect btnPopupRect_13 = new Rect(420, 50, 120, 50);
    private Rect btnPopupRect_21 = new Rect(60, 140, 120, 50);
    private Rect btnPopupRect_22 = new Rect(240, 140, 120, 50);
    private Rect btnPopupRect_23 = new Rect(420, 140, 120, 50);
    private Rect btnPopupRect_31 = new Rect(60, 230, 120, 50);
    private Rect btnPopupRect_32 = new Rect(240, 230, 120, 50);
    private Rect btnPopupRect_33 = new Rect(420, 230, 120, 50);

    // TODO: Rect For Landscape Screen Medium
    private Rect popupDevMediumRect = new Rect(0, 0, 400, 300);
    private Rect btnMediumExitRect = new Rect(140, 215, 120, 50);
    private Rect btnPopupMediumRect_11 = new Rect(60, 50, 120, 50);
    private Rect btnPopupMediumRect_12 = new Rect(240, 50, 120, 50);
    private Rect btnPopupMediumRect_21 = new Rect(60, 140, 120, 50);
    private Rect btnPopupMediumRect_22 = new Rect(240, 140, 120, 50);

    // TODO: Rect For Portrait Screen
    private Rect popupPortraitDevRect = new Rect(0, 0, 600, 400);
    private Rect btnPortraitExitRect = new Rect(240, 310, 120, 50);
    private Rect btnPopupPortraitRect_11 = new Rect(60, 50, 120, 50);
    private Rect btnPopupPortraitRect_12 = new Rect(240, 50, 120, 50);
    private Rect btnPopupPortraitRect_13 = new Rect(420, 50, 120, 50);
    private Rect btnPopupPortraitRect_21 = new Rect(60, 140, 120, 50);
    private Rect btnPopupPortraitRect_22 = new Rect(240, 140, 120, 50);
    private Rect btnPopupPortraitRect_23 = new Rect(420, 140, 120, 50);
    private Rect btnPopupPortraitRect_31 = new Rect(60, 230, 120, 50);
    private Rect btnPopupPortraitRect_32 = new Rect(240, 230, 120, 50);
    private Rect btnPopupPortraitRect_33 = new Rect(420, 230, 120, 50);

    private void Awake()
    {
        Debug.Log("AWkae dev gui");

        width = Screen.width;
        height = Screen.height;

        AutoScale();

        checkLogViewer = false;
        MG = FindObjectOfType<MG_Interface>();
        if (MG != null)
        {
            checkHasMG = true;
            checkDevMode = MG.devMode;
            checkRewardLoaded = MG.RewardLoaded;


#if UNITY_EDITOR
            checkLogViewer = false;
            MG.reporter.SetActive(false);
#else
            MG.reporter.SetActive(checkLogViewer);
#endif
        }
        else
        {
            checkHasMG = false;
            checkDevMode = false;
            checkRewardLoaded = false;
        }
    }

    private void Start()
    {
        checkPopupDev = false;
    }

    private void Update()
    {
        if (checkHasMG)
        {
            if (checkDevMode != MG.devMode)
            {
                HandleDevModeChange();
            }
        }
    }

    private void HandleDevModeChange()
    {
        checkDevMode = MG.devMode;
        if (checkDevMode)
        {
            checkPopupDev = false;

#if UNITY_EDITOR
            MG.reporter.SetActive(false);
#else
            MG.reporter.SetActive(true);
#endif

        }
    }

    private void OnGUI()
    {
        if (!checkDevMode) return;
        GUI.skin.label.fontSize = GUI.skin.box.fontSize = GUI.skin.button.fontSize = fontSize;

        GUIStyle style = new GUIStyle();

        if (!checkPopupDev)
        {
            btnDevRect.x = btnDevPos.x;
            btnDevRect.y = btnDevPos.y;
            if (GUI.Button(btnDevRect, "DEV"))
            {
                checkPopupDev = !checkPopupDev;
            }
        }
        else
        {
            if (PopupType == DevPopupType.Landscape)
            {
                ShowPopupLandscape();
            }
            else if (PopupType == DevPopupType.LandscapeMini)
            {
                ShowPopupLandscapeMini();
            }
            else if (PopupType == DevPopupType.Portrait)
            {
                ShowPopupPortrait();
            }
            else if (PopupType == DevPopupType.PortraitMini)
            {
                ShowPopupPortraitMini();
            }


        }

    }


    #region Show Popup DEV

    private void ShowPopupLandscape()
    {
        GUI.BeginGroup(new Rect(width / 2 - popupDevRect.width / 2, height / 2 - popupDevRect.height / 2, popupDevRect.width, popupDevRect.height));
        GUI.Box(popupDevRect, "-----MYSTIC DEV MODE-----");

        // Turn On Off Reward Loader
        if (GUI.Button(btnPopupRect_11, "Reward: " + checkRewardLoaded))
        {
            Debug.Log("Reward: " + checkRewardLoaded);
            HandleToggleRewardLoaded();
        }


        if (GUI.Button(btnPopupRect_12, "Ads: "))
        {

        }

        // Turn On Off Log Viewer
        if (GUI.Button(btnPopupRect_13, "Logs: " + checkLogViewer))
        {
            HandleToggleLogViewer();
        }

        if (GUI.Button(btnPopupRect_21, "Add Coin"))
        {
            // TODO: Add coin to DB
        }

        if (GUI.Button(btnPopupRect_22, "Unlock"))
        {
            // TODO: Add Gem is Here
            SetUnlockAllLevel();
        }

        // TODO: ADD more Function for Dev mode
        // GUI.Button(btnPopupRect_23, "Add Gems");
        // GUI.Button(btnPopupRect_31, "Add Gems");
        // GUI.Button(btnPopupRect_32, "Add Coin");
        // GUI.Button(btnPopupRect_33, "Add Gems");

        if (GUI.Button(btnExitRect, "Exit"))
        {
            checkPopupDev = false;
        }

        GUI.EndGroup();
    }

    private void ShowPopupLandscapeMini()
    {
        GUI.BeginGroup(new Rect(width / 2 - popupDevMediumRect.width / 2, height / 2 - popupDevMediumRect.height / 2, popupDevMediumRect.width, popupDevMediumRect.height));
        GUI.Box(popupDevMediumRect, "-----MYSTIC DEV MODE-----");

        // Turn On Off Reward Loader
        if (GUI.Button(btnPopupMediumRect_11, "Reward: " + checkRewardLoaded))
        {
            Debug.Log("Reward: " + checkRewardLoaded);
            HandleToggleRewardLoaded();
        }


        // Turn On Off Log Viewer
        if (GUI.Button(btnPopupMediumRect_12, "Logs: " + checkLogViewer))
        {
            HandleToggleLogViewer();
        }

        if (GUI.Button(btnPopupMediumRect_21, "Add Coin"))
        {
            // TODO: Add coin to DB
        }

        if (GUI.Button(btnPopupMediumRect_22, "Add Gem"))
        {
            // TODO: Add Gem is Here
        }

        if (GUI.Button(btnMediumExitRect, "Exit"))
        {
            checkPopupDev = false;
        }

        GUI.EndGroup();
    }

    private void ShowPopupPortrait()
    {
        GUI.BeginGroup(new Rect(width / 2 - popupPortraitDevRect.width / 2, height / 2 - popupPortraitDevRect.height / 2, popupPortraitDevRect.width, popupPortraitDevRect.height));
        GUI.Box(popupPortraitDevRect, "-----MYSTIC DEV MODE-----");


        // Turn On Off Reward Loader
        if (GUI.Button(btnPopupPortraitRect_11, "Reward: " + checkRewardLoaded))
        {
            Debug.Log("Reward: " + checkRewardLoaded);
            HandleToggleRewardLoaded();
        }


        if (GUI.Button(btnPopupPortraitRect_12, "Ads: "))
        {

        }

        // Turn On Off Log Viewer
        if (GUI.Button(btnPopupPortraitRect_13, "Logs: " + checkLogViewer))
        {
            HandleToggleLogViewer();
        }

        if (GUI.Button(btnPopupPortraitRect_21, "Add Coin"))
        {
            // TODO: Add coin to DB
        }

        if (GUI.Button(btnPopupPortraitRect_22, "Add Gem"))
        {
            // TODO: Add Gem is Here
        }

        // TODO: ADD more Function for Dev mode
        // GUI.Button(btnPopupPortraitRect_23, "Add Gems");
        // GUI.Button(btnPopupPortraitRect_31, "Add Gems");
        // GUI.Button(btnPopupPortraitRect_32, "Add Coin");
        // GUI.Button(btnPopupPortraitRect_33, "Add Gems");

        if (GUI.Button(btnPortraitExitRect, "Exit"))
        {
            checkPopupDev = false;
        }

        GUI.EndGroup();
    }

    private void ShowPopupPortraitMini()
    {
        GUI.BeginGroup(new Rect(width / 2 - popupDevMediumRect.width / 2, height / 2 - popupDevMediumRect.height / 2, popupDevMediumRect.width, popupDevMediumRect.height));
        GUI.Box(popupDevMediumRect, "-----MYSTIC DEV MODE-----");

        // Turn On Off Reward Loader
        if (GUI.Button(btnPopupMediumRect_11, "Reward: " + checkRewardLoaded))
        {
            Debug.Log("Reward: " + checkRewardLoaded);
            HandleToggleRewardLoaded();
        }


        // Turn On Off Log Viewer
        if (GUI.Button(btnPopupMediumRect_12, "Logs: " + checkLogViewer))
        {
            HandleToggleLogViewer();
        }

        if (GUI.Button(btnPopupMediumRect_21, "Add Coin"))
        {
            // TODO: Add coin to DB
        }

        if (GUI.Button(btnPopupMediumRect_22, "Add Gem"))
        {
            // TODO: Add Gem is Here
        }

        if (GUI.Button(btnMediumExitRect, "Exit"))
        {
            checkPopupDev = false;
        }

        GUI.EndGroup();
    }


    #endregion



    #region Dev Functions

    private void HandleToggleRewardLoaded()
    {
        MG.RewardLoaded = !MG.RewardLoaded;
        checkRewardLoaded = MG.RewardLoaded;
    }

    private void HandleToggleLogViewer()
    {
#if UNITY_EDITOR
        // checkLogViewer = false;
        checkLogViewer = !checkLogViewer;
        if (checkLogViewer)
        {
            MG.reporter.SetActive(true);
        }
        else
        {
            MG.reporter.SetActive(false);
        }
#else
                checkLogViewer = !checkLogViewer;
                if (checkLogViewer)
                {
                    MG.reporter.SetActive(true);
                }
                else
                {
                    MG.reporter.SetActive(false);
                }
#endif
    }

    private void SetUnlockAllLevel()
    {
        LevelControllerMain.UnlockAllLevelsForTest();
    }

    #endregion

    #region Auto Scale

    private void AutoScale()
    {
        width = Screen.width;
        height = Screen.height;
        var resSize = height;
        if (width < height)
        {
            resSize = width;
        }

        Debug.Log("Res Size: " + resSize);

        if (resSize >= 2400)
        {
            Debug.Log("Size 2560");
            fontSize = 32;
            btnDevRect = new Rect(0, 100, 140, 140);
            popupDevRect = new Rect(0, 0, 940, 625);
            btnExitRect = new Rect(940 / 2 - 120, 60 + 270 + 120, 240, 100);
            btnPopupRect_11 = new Rect(940 / 2 - 360 - 45, 80, 240, 100);
            btnPopupRect_12 = new Rect(940 / 2 - 120, 80, 240, 100);
            btnPopupRect_13 = new Rect(940 / 2 + 120 + 45, 80, 240, 100);
            btnPopupRect_21 = new Rect(940 / 2 - 360 - 45, 80 + 100 + 40, 240, 100);
            btnPopupRect_22 = new Rect(940 / 2 - 120, 80 + 100 + 40, 240, 100);
            btnPopupRect_23 = new Rect(940 / 2 + 120 + 45, 80 + 100 + 40, 240, 100);
            btnPopupRect_31 = new Rect(940 / 2 - 360 - 45, 80 + 200 + 80, 240, 100);
            btnPopupRect_32 = new Rect(940 / 2 - 120, 80 + 200 + 80, 240, 100);
            btnPopupRect_33 = new Rect(940 / 2 + 120 + 45, 80 + 200 + 80, 240, 100);

            // TODO: Rect For Landscape Screen Medium
            popupDevMediumRect = new Rect(0, 0, 625, 470);
            btnMediumExitRect = new Rect(625 / 2 - 90, 60 + 190 + 50, 220, 90);
            btnPopupMediumRect_11 = new Rect(625 / 2 - 15 - 220, 60, 220, 90);
            btnPopupMediumRect_12 = new Rect(625 / 2 + 15, 60, 220, 90);
            btnPopupMediumRect_21 = new Rect(625 / 2 - 15 - 220, 60 + 90 + 25, 220, 90);
            btnPopupMediumRect_22 = new Rect(625 / 2 + 15, 60 + 90 + 25, 220, 90);

            // TODO: Rect For Portrait Screen
            popupPortraitDevRect = new Rect(0, 0, 940, 625);
            btnPortraitExitRect = new Rect(940 / 2 - 120, 60 + 270 + 120, 240, 100);
            btnPopupPortraitRect_11 = new Rect(940 / 2 - 360 - 45, 80, 240, 100);
            btnPopupPortraitRect_12 = new Rect(940 / 2 - 120, 80, 240, 100);
            btnPopupPortraitRect_13 = new Rect(940 / 2 + 120 + 45, 80, 240, 100);
            btnPopupPortraitRect_21 = new Rect(940 / 2 - 360 - 45, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_22 = new Rect(940 / 2 - 120, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_23 = new Rect(940 / 2 + 120 + 45, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_31 = new Rect(940 / 2 - 360 - 45, 80 + 200 + 80, 240, 100);
            btnPopupPortraitRect_32 = new Rect(940 / 2 - 120, 80 + 200 + 80, 240, 100);
            btnPopupPortraitRect_33 = new Rect(940 / 2 + 120 + 45, 80 + 200 + 80, 240, 100);
        }
        else if (resSize >= 1400 && resSize < 2400)
        {
            Debug.Log("Size 1440");
            fontSize = 32;
            btnDevRect = new Rect(0, 100, 140, 140);
            popupDevRect = new Rect(0, 0, 940, 625);
            btnExitRect = new Rect(940 / 2 - 120, 60 + 270 + 120, 240, 100);
            btnPopupRect_11 = new Rect(940 / 2 - 360 - 45, 80, 240, 100);
            btnPopupRect_12 = new Rect(940 / 2 - 120, 80, 240, 100);
            btnPopupRect_13 = new Rect(940 / 2 + 120 + 45, 80, 240, 100);
            btnPopupRect_21 = new Rect(940 / 2 - 360 - 45, 80 + 100 + 40, 240, 100);
            btnPopupRect_22 = new Rect(940 / 2 - 120, 80 + 100 + 40, 240, 100);
            btnPopupRect_23 = new Rect(940 / 2 + 120 + 45, 80 + 100 + 40, 240, 100);
            btnPopupRect_31 = new Rect(940 / 2 - 360 - 45, 80 + 200 + 80, 240, 100);
            btnPopupRect_32 = new Rect(940 / 2 - 120, 80 + 200 + 80, 240, 100);
            btnPopupRect_33 = new Rect(940 / 2 + 120 + 45, 80 + 200 + 80, 240, 100);

            // TODO: Rect For Landscape Screen Medium
            popupDevMediumRect = new Rect(0, 0, 625, 470);
            btnMediumExitRect = new Rect(625 / 2 - 90, 60 + 190 + 50, 220, 90);
            btnPopupMediumRect_11 = new Rect(625 / 2 - 15 - 220, 60, 220, 90);
            btnPopupMediumRect_12 = new Rect(625 / 2 + 15, 60, 220, 90);
            btnPopupMediumRect_21 = new Rect(625 / 2 - 15 - 220, 60 + 90 + 25, 220, 90);
            btnPopupMediumRect_22 = new Rect(625 / 2 + 15, 60 + 90 + 25, 220, 90);

            // TODO: Rect For Portrait Screen
            popupPortraitDevRect = new Rect(0, 0, 940, 625);
            btnPortraitExitRect = new Rect(940 / 2 - 120, 60 + 270 + 120, 240, 100);
            btnPopupPortraitRect_11 = new Rect(940 / 2 - 360 - 45, 80, 240, 100);
            btnPopupPortraitRect_12 = new Rect(940 / 2 - 120, 80, 240, 100);
            btnPopupPortraitRect_13 = new Rect(940 / 2 + 120 + 45, 80, 240, 100);
            btnPopupPortraitRect_21 = new Rect(940 / 2 - 360 - 45, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_22 = new Rect(940 / 2 - 120, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_23 = new Rect(940 / 2 + 120 + 45, 80 + 100 + 40, 240, 100);
            btnPopupPortraitRect_31 = new Rect(940 / 2 - 360 - 45, 80 + 200 + 80, 240, 100);
            btnPopupPortraitRect_32 = new Rect(940 / 2 - 120, 80 + 200 + 80, 240, 100);
            btnPopupPortraitRect_33 = new Rect(940 / 2 + 120 + 45, 80 + 200 + 80, 240, 100);
        }
        else if (resSize >= 1000 && resSize < 1400)
        {
            fontSize = 24;

            btnDevRect = new Rect(0, 100, 100, 100);
            popupDevRect = new Rect(0, 0, 750, 500);
            btnExitRect = new Rect(750 / 2 - 90, 60 + 225 + 75, 180, 75);
            btnPopupRect_11 = new Rect(750 / 2 - 310, 60, 180, 75);
            btnPopupRect_12 = new Rect(750 / 2 - 90, 60, 180, 75);
            btnPopupRect_13 = new Rect(750 / 2 + 130, 60, 180, 75);
            btnPopupRect_21 = new Rect(750 / 2 - 310, 60 + 75 + 25, 180, 75);
            btnPopupRect_22 = new Rect(750 / 2 - 90, 60 + 75 + 25, 180, 75);
            btnPopupRect_23 = new Rect(750 / 2 + 130, 60 + 75 + 25, 180, 75);
            btnPopupRect_31 = new Rect(750 / 2 - 310, 60 + 150 + 50, 180, 75);
            btnPopupRect_32 = new Rect(750 / 2 - 90, 60 + 150 + 50, 180, 75);
            btnPopupRect_33 = new Rect(750 / 2 + 130, 60 + 150 + 50, 180, 75);

            // TODO: Rect For Landscape Screen Medium
            popupDevMediumRect = new Rect(0, 0, 500, 375);
            btnMediumExitRect = new Rect(500 / 2 - 90, 60 + 150 + 50, 180, 75);
            btnPopupMediumRect_11 = new Rect(500 / 2 - 15 - 180, 60, 180, 75);
            btnPopupMediumRect_12 = new Rect(500 / 2 + 15, 60, 180, 75);
            btnPopupMediumRect_21 = new Rect(500 / 2 - 15 - 180, 60 + 75 + 25, 180, 75);
            btnPopupMediumRect_22 = new Rect(500 / 2 + 15, 60 + 75 + 25, 180, 75);

            // TODO: Rect For Portrait Screen
            popupPortraitDevRect = new Rect(0, 0, 750, 500);
            btnPortraitExitRect = new Rect(750 / 2 - 90, 60 + 225 + 75, 180, 75);
            btnPopupPortraitRect_11 = new Rect(750 / 2 - 310, 60, 180, 75);
            btnPopupPortraitRect_12 = new Rect(750 / 2 - 90, 60, 180, 75);
            btnPopupPortraitRect_13 = new Rect(750 / 2 + 130, 60, 180, 75);
            btnPopupPortraitRect_21 = new Rect(750 / 2 - 310, 60 + 75 + 25, 180, 75);
            btnPopupPortraitRect_22 = new Rect(750 / 2 - 90, 60 + 75 + 25, 180, 75);
            btnPopupPortraitRect_23 = new Rect(750 / 2 + 130, 60 + 75 + 25, 180, 75);
            btnPopupPortraitRect_31 = new Rect(750 / 2 - 310, 60 + 150 + 50, 180, 75);
            btnPopupPortraitRect_32 = new Rect(750 / 2 - 90, 60 + 150 + 50, 180, 75);
            btnPopupPortraitRect_33 = new Rect(750 / 2 + 130, 60 + 150 + 50, 180, 75);
        }
    }



    #endregion
}
