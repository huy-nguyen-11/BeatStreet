using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    [SerializeField] private List<ItemDaily> itemDailys;
    [SerializeField] private List<GameObject> listDays;
    [SerializeField] private GameObject buttonClaim , buttonClaimX2;
    //public GetReward getReward;
    //public Transform posHeart, posCoin;
    //public Sprite /*heartSprite, coinsSprite ,*/ titleSpriteDayCoins, titleSpriteDayHeart, titleSpriteToday;
    public int currentDay;
    private Tween scaleTween;
    //public MainMenu mainMenu; 

    public CollectItemUICtrl getReward;

    private int amountCoins = 0 , amountHearts = 0;


    private void OnEnable()
    {
        //can claim
        if (System.DateTime.Now.Day != PlayerPrefs.GetInt("Yesterday", 0) && PlayerPrefs.GetInt("is claimed yesterday", 0) == 0 && AudioBase.Instance.isCheckPlayed)
        {
            currentDay = PlayerPrefs.GetInt("currentday", 0);
            SetPack();
            buttonClaim.GetComponent<Button>().interactable = true;
            buttonClaimX2.GetComponent<Button>().interactable = true;
        }
        else // not claim
        {
            buttonClaim.GetComponent<Button>().interactable = false;
            buttonClaimX2.GetComponent<Button>().interactable = false;
            for (int i = 0; i < listDays.Count; i++)
            {
                if (PlayerPrefs.GetInt($"Button_{i}_Claimed", 0) == 1)
                {
                    //listDays[i].transform.GetChild(0).gameObject.SetActive(false); // glow
                    listDays[i].transform.GetChild(0).gameObject.SetActive(false); // day select
                    listDays[i].transform.GetChild(1).gameObject.SetActive(true); // day normal
                    listDays[i].transform.GetChild(3).gameObject.SetActive(true); // is claimed

                    //listDays[i].transform.Find("infor").gameObject.SetActive(false);
                    //listDays[i].transform.Find("imgChecked").gameObject.SetActive(true);
                    //listDays[i].transform.Find("imgToday").gameObject.SetActive(false);
                    //if(i==0 || i==2 || i == 4)
                    //{
                    //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayCoins;
                    //}
                    //else
                    //{
                    //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayHeart;
                    //}
                }
                else
                {
                    //listDays[i].transform.GetChild(0).gameObject.SetActive(false); // glow
                    listDays[i].transform.GetChild(0).gameObject.SetActive(false); // day select
                    listDays[i].transform.GetChild(1).gameObject.SetActive(true); // day normal
                    listDays[i].transform.GetChild(3).gameObject.SetActive(false); // is claimed
                    //listDays[i].transform.Find("infor").gameObject.SetActive(true);
                    //listDays[i].transform.Find("imgChecked").gameObject.SetActive(false);
                    //listDays[i].transform.Find("imgToday").gameObject.SetActive(false);
                    //if (i == 0 || i == 2 || i == 4)
                    //{
                    //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayCoins;
                    //}
                    //else
                    //{
                    //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayHeart;
                    //}
                }
            }
        }
    }

    void SetPack()
    {
        for (int i = 0; i < listDays.Count; i++)
        {
            if (i == currentDay && PlayerPrefs.GetInt($"Button_{i}_Claimed", 0) != 1) // have getting reward
            {
                //listDays[i].transform.Find("infor").gameObject.SetActive(true);
                //listDays[i].transform.Find("imgChecked").gameObject.SetActive(false);
                //listDays[i].transform.Find("imgToday").gameObject.SetActive(true);
                //listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteToday;
                //listDays[i].transform.GetChild(0).gameObject.SetActive(true); // glow
                listDays[i].transform.GetChild(0).gameObject.SetActive(true); // day select
                listDays[i].transform.GetChild(1).gameObject.SetActive(false); // day normal
                listDays[i].transform.GetChild(3).gameObject.SetActive(false); // is claimed

                scaleTween = listDays[i].transform.GetChild(1).transform.DOScale(1.06f, 0.5f).SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
                buttonClaim.GetComponent<Button>().interactable = true;
                buttonClaimX2.GetComponent<Button>().interactable = true;
            }
            else if (PlayerPrefs.GetInt($"Button_{i}_Claimed", 0) == 1) // is claimed
            {
                //listDays[i].transform.Find("infor").gameObject.SetActive(false);
                //listDays[i].transform.Find("imgChecked").gameObject.SetActive(true);
                //listDays[i].transform.Find("imgToday").gameObject.SetActive(false);
                //if (i == 0 || i == 2 || i == 4)
                //{
                //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayCoins;
                //}
                //else
                //{
                //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayHeart;
                //}
                //listDays[i].transform.GetChild(0).gameObject.SetActive(false); // glow
                listDays[i].transform.GetChild(0).gameObject.SetActive(false); // day select
                listDays[i].transform.GetChild(0).gameObject.SetActive(true); // day normal
                listDays[i].transform.GetChild(3).gameObject.SetActive(true); // is claimed
                scaleTween.Kill();
                buttonClaim.GetComponent<Button>().interactable = false;
                buttonClaimX2.GetComponent<Button>().interactable = false;
            }
            else // havent getting reward
            {
                //listDays[i].transform.Find("infor").gameObject.SetActive(true);
                //listDays[i].transform.Find("imgChecked").gameObject.SetActive(false);
                //listDays[i].transform.Find("imgToday").gameObject.SetActive(false);
                //if (i == 0 || i == 2 || i == 4)
                //{
                //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayCoins;
                //}
                //else
                //{
                //    listDays[i].transform.Find("title").gameObject.GetComponent<Image>().sprite = titleSpriteDayHeart;
                //}
                //listDays[i].transform.GetChild(0).gameObject.SetActive(false); // glow
                listDays[i].transform.GetChild(0).gameObject.SetActive(false); // day select
                listDays[i].transform.GetChild(1).gameObject.SetActive(true); // day normal
                listDays[i].transform.GetChild(3).gameObject.SetActive(false); // is claimed
                buttonClaim.GetComponent<Button>().interactable = false;
                buttonClaimX2.GetComponent<Button>().interactable = false;
            }

        }
    }

    public void Claimed()
    {
        //AudioManager.instance.PlaySFX("click");
        scaleTween.Kill();

        buttonClaim.GetComponent<Button>().interactable = false;
        buttonClaimX2.GetComponent<Button>().interactable = false;
        //save data
        PlayerPrefs.SetInt("is claimed yesterday", 1);
        PlayerPrefs.SetInt("Yesterday", System.DateTime.Now.Day);
        PlayerPrefs.Save();
        //StartCoroutine(CountCoin(10, 1));
        //StartCoroutine(CountHeart(5, 1));
        //get data
        //DataManager.Coins += itemDailys[currentDay].coins;
        //DataManager.Hearts += itemDailys[currentDay].heart;
        //if (itemDailys[currentDay].nameSkinBlue != "" || itemDailys[currentDay].nameSkinRed != "")
        //{
        //    DataManager.SetSkinBlueUnlock(itemDailys[currentDay].nameSkinBlue, 1);
        //    DataManager.SetSkinRedUnlock(itemDailys[currentDay].nameSkinRed, 1);
        //}
        //else if(itemDailys[currentDay].nameWeapon != "")
        //{
        //    DataManager.SetWeaponUnlock(itemDailys[currentDay].nameWeapon, 1);
        //}
        PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + itemDailys[currentDay].heart);
        PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + itemDailys[currentDay].coins);
        if(itemDailys[currentDay].heart > 0)
        {
            getReward.DoAddGemsEffect(listDays[currentDay].transform.position, PlayerPrefs.GetInt("Diamont") - itemDailys[currentDay].heart, PlayerPrefs.GetInt("Diamont"));
        }
        else
        {
            getReward.DoAddCoinEffect(listDays[currentDay].transform.position, PlayerPrefs.GetInt("Coin") - itemDailys[currentDay].coins, PlayerPrefs.GetInt("Coin"));
        }

        PlayerPrefs.SetInt($"Button_{currentDay}_Claimed", 1);
        SetPack();
        StartCoroutine(ClosePanel());
        //amountCoins = itemDailys[currentDay].coins;
        //amountHearts = itemDailys[currentDay].heart;
    }


    public void ClaimedX2()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //popUpInter.SetActive(true);
            //mainMenu.OpenPanel(8);
            Debug.Log("not internet connected");
        }
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            MG_Interface.Current.Reward_Show(HandleRewardResult);

            //Notification noti = FindObjectOfType<Notification>();
            //noti.CheckNotify();
        }
    }

    private void HandleRewardResult(bool result)
    {
        if (result)
        {
            scaleTween.Kill();

            //save data
            PlayerPrefs.SetInt("is claimed yesterday", 1);
            PlayerPrefs.SetInt("Yesterday", System.DateTime.Now.Day);
            PlayerPrefs.Save();


            PlayerPrefs.SetInt("Diamont", PlayerPrefs.GetInt("Diamont") + 2*itemDailys[currentDay].heart);
            PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + 2*itemDailys[currentDay].coins);
            if (itemDailys[currentDay].heart > 0)
            {
                getReward.DoAddGemsEffect(listDays[currentDay].transform.position, PlayerPrefs.GetInt("Diamont") - 2*itemDailys[currentDay].heart, PlayerPrefs.GetInt("Diamont"));
            }
            else
            {
                getReward.DoAddCoinEffect(listDays[currentDay].transform.position, PlayerPrefs.GetInt("Coin") - 2*itemDailys[currentDay].coins, PlayerPrefs.GetInt("Coin"));
            }
            //amountCoins = 2 * itemDailys[currentDay].coins;
            //amountHearts = 2 * itemDailys[currentDay].heart;

            PlayerPrefs.SetInt($"Button_{currentDay}_Claimed", 1);
            SetPack();

            buttonClaimX2.GetComponent<Button>().interactable = false;
            buttonClaim.GetComponent<Button>().interactable = false;
            StartCoroutine(ClosePanel());
        }
        else
        {
            Claimed();
        }
    }

    public void ButtonNothanks()
    {
        buttonClaimX2.SetActive(false);
    }

    IEnumerator CountCoin(int number, int num)
    {
        yield return new WaitForSecondsRealtime(0.63f);
        var timer = 0f;
        for (int i = 0; i < number; i++)
        {
            timer += 0.01f;

            yield return new WaitForSecondsRealtime(timer);
        }
    }

    IEnumerator CountHeart(int number, int num)
    {
        yield return new WaitForSecondsRealtime(0.63f);
        var timer = 0f;
        for (int i = 0; i < number; i++)
        {
            timer += 0.05f;

            yield return new WaitForSecondsRealtime(timer);
        }
    }

    IEnumerator ClosePanel()
    {
        //yield return new WaitForSeconds(0.5f);
        ////AudioManager.instance.isCheckPlayed = false;

        //Vector3 pos = listDays[currentDay].transform.position;

        if (PlayerPrefs.GetInt("is claimed yesterday", 0) == 1)
        {
            PlayerPrefs.SetInt("is claimed yesterday", 0);
            currentDay += 1;
            if (currentDay >= 7)
            {
                currentDay = 0;
                for (int i = 0; i < 7; i++)
                {
                    PlayerPrefs.SetInt($"Button_{i}_Claimed", 0);
                }
            }
            PlayerPrefs.SetInt("currentday", currentDay);
        }

        //if (amountCoins > 0 && amountHearts == 0)
        //{
        //    getReward.DoAddCoinEffect(pos, DataManager.Coins - amountCoins, DataManager.Coins);
        //}
        //else if(amountHearts > 0 && amountCoins == 0)
        //{
        //    getReward.DoAddGemsEffect(pos, DataManager.Hearts - amountHearts, DataManager.Hearts);
        //}
        //else
        //{
        //    getReward.DoAddGemsEffect(new Vector3(pos.x - 0.5f, pos.y - 0.5f , 0) , DataManager.Hearts - amountHearts, DataManager.Hearts);
        //    getReward.DoAddCoinEffect(new Vector3(pos.x + 0.5f, pos.y -0.5f, 0), DataManager.Coins - amountCoins, DataManager.Coins);
        //}

        yield return new WaitForSeconds(2f);
        AudioBase.Instance.isCheckPlayed = false;
        MainManager.Instance.OpenPanel(3);
        //mainMenu.ClosePanel();
    }

    public void NexDay()
    {
        //OnDailyReward();
        //PlayerPrefs.SetInt("is claimed yesterday", 0);
        // buttonNextday.transform.localScale = Vector3.zero;
    }

    [System.Serializable]
    public class ItemDaily
    {
        public int coins;
        public int heart;
        public string nameSkinBlue;
        public string nameSkinRed;
        public string nameWeapon;
    }
}
