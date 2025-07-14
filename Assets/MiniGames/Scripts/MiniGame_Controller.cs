using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace I2.MiniGames
{
    //	[System.Serializable]
    //	public class WheelEvents
    //	{
    //		public UnityEvent _DisableAllObj = new UnityEvent ();
    //		public UnityEvent _OnUpdateFreeRounds = new UnityEvent();
    //		public UnityEvent _OnUpdateVideoRound 	= new UnityEvent();	
    //		public UnityEvent _OnVideoRoundEnd 	= new UnityEvent();	
    //		public UnityEvent _OnVideoRoundActive 	= new UnityEvent();
    //	}
    [System.Serializable] public class UnityEventCurrencyCallbk : UnityEvent<int, System.Action<bool>> { }

    // This Class implements the flow and monetization logic for all MiniGames
    // It provides some free rounds and then allow paying for extra chances and purchasing currency if there is not enough. 
    [AddComponentMenu("I2/MiniGames/Controller")]
    public class MiniGame_Controller : MonoBehaviour
    {
        #region Variables

        public MiniGame _Game;
        public Text CountTimeText;
        public bool _StartGameOnEnable = false;
        public float _TimeBeforeStartRound = 0;
        private int mNumFreeRounds;
        private int mNumPayedRounds;
        private bool mIsPlaying;

        //-- [Time] -------

        private float countdowntime;
        private int hour, minutes, seconds;
        private System.TimeSpan steptime;
        private int stateID;
        //-- [Time] -------


        //--[ Free Rounds ]---------------------
        [Space(10)]
        [Header("SETTING")]
        [SerializeField]
        public int _InitialFreeRounds;
        public int _InitialVideoRounds;
        public int _TimeToNextRound;
        public int _TimeToReset;
        private bool flagActiveVideo = true;
        public bool spining;

        [Space(10)]
        [Header("EVENTS")]
        public UnityEvent _DisableAllObj = new UnityEvent();
        public UnityEvent _OnUpdateFreeRounds = new UnityEvent();
        public UnityEvent _OnUpdateVideoRound = new UnityEvent();
        public UnityEvent _OnVideoRoundEnd = new UnityEvent();
        public UnityEvent _OnVideoRoundActive = new UnityEvent();

        #endregion

        #region Setup and State Management
        int DateToSecond(System.TimeSpan time)
        {
            return (int)time.TotalSeconds;
        }

        void OnApplicationFocus(bool focusStatus)
        {
            if (focusStatus)
                InitMinigame();
        }
        public void OnEnable()
        {
            if (_StartGameOnEnable) InitMinigame();
        }

        void InitMinigame()
        {
            if (!spining)
            {
                if (PlayerPrefs.GetString("nextspin") == "")
                    PlayerPrefs.SetString("nextspin", UnbiasedTime.Instance.Now().AddDays(-2).ToString());
                steptime = UnbiasedTime.Instance.Now() - System.DateTime.Parse(PlayerPrefs.GetString("nextspin"));

                if (steptime.TotalSeconds > _TimeToReset - _TimeToNextRound * 60)
                {
                    flagActiveVideo = true;
                    stateID = 0; // free time
                    ResetGame();
                }
                else if (steptime.TotalSeconds > 0)
                {
                    flagActiveVideo = false;
                    stateID = 1; // video time
                    videoroundactive();
                }
                else
                {
                    flagActiveVideo = false;
                    countdowntime = -(int)steptime.TotalSeconds;
                    stateID = -1; // waitting time
                    waittingactive();
                }
            }
        }
        // Initializes the game and starts the first round
        public void StartGame()
        {
            mNumFreeRounds = _InitialFreeRounds;
            mNumPayedRounds = 0;
            mIsPlaying = true;
            _Game.SetupGame();
            _DisableAllObj.Invoke();
            if (_TimeBeforeStartRound > 0)
                Invoke("OnReadyForNextRound", _TimeBeforeStartRound);
            else
            if (_TimeBeforeStartRound >= 0)
                OnReadyForNextRound();
        }
        void videoroundactive()
        {
            mNumFreeRounds = 0;
            mNumPayedRounds = 0;
            mIsPlaying = true;
            _Game.SetupGame();
            if (_TimeBeforeStartRound > 0)
                Invoke("OnReadyForNextRound", _TimeBeforeStartRound);
            else
                if (_TimeBeforeStartRound >= 0)
                OnReadyForNextRound();
        }
        void waittingactive()
        {
            mNumFreeRounds = 0;
            mNumPayedRounds = _InitialVideoRounds;
            mIsPlaying = true;
            _Game.SetupGame();
            if (_TimeBeforeStartRound > 0)
                Invoke("OnReadyForNextRound", _TimeBeforeStartRound);
            else
                if (_TimeBeforeStartRound >= 0)
                OnReadyForNextRound();
        }

        void Update()
        {
            if (countdowntime > 0)
            {
                countdowntime -= Time.deltaTime;
                minutes = (int)(countdowntime / 60) % 60;
                seconds = (int)countdowntime % 60;
                //				hour = (int)((countdowntime / 60) / 60) % 24;
                CountTimeText.text = "Next time: " + string.Format("{0:D2}:{1:D2}", minutes, seconds);
            }
            else
            {
                if (!flagActiveVideo)
                {
                    flagActiveVideo = true;
                    _OnVideoRoundActive.Invoke();
                }
            }
        }

        public virtual void StopGame()
        {
            mNumFreeRounds = mNumPayedRounds = 0;
            mIsPlaying = false;
            _DisableAllObj.Invoke();
        }

        public virtual void ResetGame()
        {
            StopGame();
            StartGame();
        }
        public void OnReadyForNextRound()
        {
            if (!mIsPlaying)
                return;

            if (!_Game.CanPlayAnotherRound())
            {
                StopGame();
            }
            else
            {
                if (mNumFreeRounds > 0)
                {

                    _OnUpdateFreeRounds.Invoke();
                }
                else
                {

                    EndVideoRound();
                }

            }
        }

        #endregion

        #region Validation

        public void ValidateRound()
        {
            if (!mIsPlaying)
            {
                DenyRound();
                return;
            }

            AllowRound();
        }
        public void ValidateRoundFree()
        {
            AllowRound();
        }
        public void EndVideoRound()
        {
            if (mNumPayedRounds < _InitialVideoRounds)
            {
                PlayerPrefs.SetString("nextspin", UnbiasedTime.Instance.Now().ToString());
                _OnUpdateVideoRound.Invoke();
            }
            else
            {
                if (stateID != -1)
                {
                    countdowntime = _TimeToNextRound * 60;
                    PlayerPrefs.SetString("nextspin", UnbiasedTime.Instance.Now().AddSeconds(_TimeToNextRound * 60).ToString());
                }
                flagActiveVideo = false;
                _OnVideoRoundEnd.Invoke();
            }

        }
        public void AllowRound()
        {
            if (mNumFreeRounds > 0)
                mNumFreeRounds--;
            else
                mNumPayedRounds++;

            _DisableAllObj.Invoke();
            _Game.StartRound();
            spining = true;
        }

        public void DenyRound()
        {
            _Game.CancelRound();
        }

        #endregion
    }
}