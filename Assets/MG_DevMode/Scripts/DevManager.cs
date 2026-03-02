using System;
using System.Collections;
using System.Collections.Generic;
//using IngameDebugConsole;
using UnityEngine;
using UnityEngine.UI;


namespace MysticDev
{
    public class DevManager : MonoBehaviour
    {
        public static DevManager Instance { get; private set; }
        
        [Header( "Properties" )]
        [SerializeField]
        [Tooltip( "If enabled, console window will persist between scenes (i.e. not be destroyed when scene changes)" )]
        private bool singleton = true;
        
        public float popupOpacity;
        
        internal RectTransform canvasTR;
        
        [Tooltip( "If enabled, on Android and iOS devices with notch screens, the console window's popup won't be obscured by the screen cutouts" )]
        internal bool popupAvoidsScreenCutout = false;
        
        // MG params
        private MG_Interface MG;

        private bool checkHasMG;
        private bool checkDevMode;

        private bool checkRewardLoaded;
        private bool checkLogViewer;
        
        // Reporter
        [SerializeField] private GameObject reporter;
        //private DebugLogManager debugLogManager;
        
        [Header( "Dev Popup" )]
        public DevPopup devPopup;

        private void Awake()
        {
            if( !Instance )
            {
                Instance = this;

                // If it is a singleton object, don't destroy it between scene changes
                if( singleton )
                    DontDestroyOnLoad( gameObject );
            }
            else if( Instance != this )
            {
                Destroy( gameObject );
                return;
            }
            
            canvasTR = (RectTransform) transform;
            
            // Find MG and set params
            checkLogViewer = false;
            MG = FindObjectOfType<MG_Interface>();
            if (MG != null)
            {
                checkHasMG = true;
                checkDevMode = MG.devMode;
                checkRewardLoaded = MG.RewardLoaded;
                reporter = MG.reporter;
            }
            else
            {
                checkHasMG = false;
                checkDevMode = false;
                checkRewardLoaded = false;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            if (checkDevMode)
            {
                devPopup.gameObject.SetActive( true );
                devPopup.UpdatePosition(true);
                InitDevParams();
                
            }
            else
            {
                devPopup.gameObject.SetActive( false );
                
            }
            
            MG.reporter.SetActive( checkDevMode );
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void LateUpdate()
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
                devPopup.gameObject.SetActive( true );
                devPopup.UpdatePosition( true );
                reporter.SetActive( true );
                InitDevParams();
            }
            else
            {
                devPopup.gameObject.SetActive(false);
                reporter.SetActive(false);
            }
        }

        // Kiem tra 1 so tham so dev, hien thi len dev content
        public void InitDevParams()
        {
            var checkReporter = MG.reporter.gameObject.activeSelf;
            var checkAds = MG.RewardLoaded;
            
            devPopup.debugLogText.text = checkReporter ? "ON" : "OFF";
            devPopup.adsText.text = checkAds ? "ON" : "OFF";
        }


        #region Dev functions

        public void ToggleDebugLog(Text resultTxt)
        {
            var newCheckReporter = !MG.reporter.gameObject.activeSelf;
            MG.reporter.gameObject.SetActive(newCheckReporter);
            if (newCheckReporter)
            {
                resultTxt.text = "ON";
            }
            else
            {
                resultTxt.text = "OFF";
            }
        }

        public void ToggleAds(Text resultTxt)
        {
            var newCheckAds = !MG.RewardLoaded;
            MG.RewardLoaded = newCheckAds;
            if (newCheckAds)
            {
                resultTxt.text = "ON";
            }
            else
            {
                resultTxt.text = "OFF";
            }
        }


        public void FullManageDevMode()
        {
            if(GamePlayManager.Instance == null) return;
            GamePlayManager.Instance.DemoFillMana();
        }

        #endregion
    }
}
