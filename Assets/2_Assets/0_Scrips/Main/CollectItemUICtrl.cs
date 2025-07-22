using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
// using ItchyOwl.ObjectPooling;
//using MoreMountains.Tools;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ViewType
{
	MainView, ShopView, DailyGiftView, InGameView
}

public class CollectItemUICtrl : /*Singleton<CollectItemUICtrl>*/MonoBehaviour
{
	public GameObject panelAddItemEffGO;

	public GameObject itemPrefabGO;
	public GameObject itemHeartPrefabGO;

	[Header("Item Pool")]
	public List<GameObject> itemBeanPool = new ();
	public List<GameObject> itemPool = new ();

	public TextMeshProUGUI tmpCoinEffect;
	public TextMeshProUGUI tmpBeanEffect;

	public Transform coinPos, gemPos;

	public int numOfItem = 9;

    public void DoAddCoinEffect(Vector3 posSpawn,int _currentCoins, int coin)
	{
        StartCoroutine(TimerPoolItemCoin(posSpawn,_currentCoins, coin));
    }
	
	public void DoAddGemsEffect(Vector3 posSpawn, int _currentGems, int bean)
	{
        StartCoroutine(TimerPoolItemGems(posSpawn,_currentGems, bean));
    }

	public void DoAddCoinAndBeanEffect(Vector3 coinPosSpawn, int _currentCoins, int coin, Vector3 beanPosSpawn, int _currentGems, int bean)
	{
		StartCoroutine(TimerPoolItemCoinAndBean(coinPosSpawn,_currentCoins, coin, beanPosSpawn,_currentGems, bean));
	}

	int counter;

	private IEnumerator TimerPoolItemGems(Vector3 posSpawn, int _currentGems, int newBean)
	{
		counter = 0;
		panelAddItemEffGO.SetActive(true);
		
		// TODO: Get Current Value for Coin and Bean
		var currentCoin = PlayerPrefs.GetInt("Coin");
		if (currentCoin < 1000)
		{
			tmpCoinEffect.SetText(currentCoin.ToString());
		}
		else
		{
			tmpCoinEffect.SetText(currentCoin.ToString("##,#"));
		}
		
		var currentGems = _currentGems;

        itemHeartPrefabGO.SetActive(true);
		if (currentGems < 1000)
		{
			tmpBeanEffect.SetText(currentGems.ToString());
		}
		else
		{
			tmpBeanEffect.SetText(currentGems.ToString("##,#"));
		}

        // TODO: DO collect bean effect

        yield return null;
		for (int i = 0; i < numOfItem; i++) // 
		{
			int a = i;
			GameObject clone = null;
			if (i >= itemBeanPool.Count)
			{
				clone = Instantiate(itemHeartPrefabGO, itemHeartPrefabGO.transform.position, Quaternion.identity, panelAddItemEffGO.transform);
				itemBeanPool.Add(clone);
			}
			else
			{
				clone = itemBeanPool[i];
			}
			
			clone.SetActive(true);
			
			Vector3 endValue = new Vector3(posSpawn.x + Random.Range(-0.8f, 0.8f), posSpawn.y + Random.Range(-0.4f, 0.4f));
			clone.transform.position = posSpawn;
			clone.transform.DOMove(endValue, 0.2f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				clone.transform.DOMoveY(-0.1f, 0.05f).SetDelay(0.1f + 0.05f * (float)a).SetEase(Ease.InOutSine)
					.SetRelative(isRelative: true)
					.OnComplete(delegate
					{
						clone.transform.DOMove(/*itemBeanPrefabGO.transform.position*/gemPos.position, 0.75f).SetEase(Ease.InOutSine).OnComplete(delegate
						{
							counter++;
							if (counter < 6 && counter %2 == 1)
                            RecyclePool(clone);
						});
					});
			});
		}

		yield return new WaitForSeconds(0.8f);

		var bean = currentGems;

        // TODO: play earn coin sound
        //AudioManager.instance.PlaySFX("getcoin");
        // Z_SoundBase.Instance.PlayEarnCoinSound();

        DOTween.To(() => bean, x => bean = x, newBean, 0.5f)
			.OnUpdate(() =>
			{
                if (bean < 1000)
                    tmpBeanEffect.SetText(bean.ToString());
                else
                    tmpBeanEffect.SetText(bean.ToString("##,#"));
			})
			.OnComplete(() =>
			{
				if (bean < 1000)
                    tmpBeanEffect.SetText(newBean.ToString());
                else
                    tmpBeanEffect.SetText(newBean.ToString("##,#"));
				currentGems = newBean;
				
				//PlayerPrefs.SetInt("LAST_POINT", currentGems);
			});
		
		
		yield return new WaitForSeconds(1f);
		panelAddItemEffGO.SetActive(false);
		
	}
	
	private IEnumerator TimerPoolItemCoin(Vector3 posSpawn, int _currentCoins , int newCoin)
	{
		Debug.Log("show reward");
		counter = 0;
		panelAddItemEffGO.SetActive(true);
		
		var currentGems = PlayerPrefs.GetInt("Diamont");
		if (currentGems < 1000)
		{
			tmpBeanEffect.SetText(currentGems.ToString());
		}
		else
		{
			tmpBeanEffect.SetText(currentGems.ToString("##,#"));
		}
		

		var currentCoin = _currentCoins;
		itemPrefabGO.SetActive(true);
		if (currentCoin < 1000)
		{
			tmpCoinEffect.SetText(currentCoin.ToString());
		}
		else
		{
			tmpCoinEffect.SetText(currentCoin.ToString("##,#"));
		}
		
		yield return null;
		for (int i = 0; i < numOfItem; i++)
		{
			int a = i;
			GameObject clone = null;
			if (i >= itemPool.Count)
			{
				clone = Instantiate(itemPrefabGO, itemPrefabGO.transform.position, Quaternion.identity, panelAddItemEffGO.transform);
				itemPool.Add(clone);
			}
			else
			{
				clone = itemPool[i];
			}
			
			clone.SetActive(true);
			
			Vector3 endValue = new Vector3(posSpawn.x + Random.Range(-0.8f, 0.8f), posSpawn.y + Random.Range(-0.4f, 0.4f));
			clone.transform.position = posSpawn;
			clone.transform.DOMove(endValue, 0.2f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				clone.transform.DOMoveY(-0.1f, 0.05f).SetDelay(0.1f + 0.05f * (float)a).SetEase(Ease.InOutSine)
					.SetRelative(isRelative: true)
					.OnComplete(delegate
					{
						clone.transform.DOMove(/*itemPrefabGO.transform.position*/coinPos.position, 0.75f).SetEase(Ease.InOutSine).OnComplete(delegate
						{
							counter++;
							if (counter < 6 && counter %2 == 1)
								//Z_SoundBase.Instance.PlayEarnCoinSound();
							RecyclePool(clone);
						});
					});
			});
		}

		yield return new WaitForSeconds(0.8f);

		var coin = currentCoin;

		// TODO: play earn coin sound
		// Z_SoundBase.Instance.PlayEarnCoinSound();
		//AudioManager.instance.PlaySFX("getcoin");


        DOTween.To(() => coin, x => coin = x, newCoin, 0.5f)
			.OnUpdate(() =>
			{
				tmpCoinEffect.SetText(coin.ToString("##,#"));
			})
			.OnComplete(() =>
			{
				tmpCoinEffect.SetText(newCoin.ToString("##,#"));
				currentCoin = newCoin;

                //PlayerPrefs.SetInt("LAST_BALL", currentCoin);
                //Z_SoundBase.Instance.PlayReceiveCoinSound();
            });
		
		yield return new WaitForSeconds(1f);
		panelAddItemEffGO.SetActive(false);
		
	}
	
	private IEnumerator TimerPoolItemCoinAndBean(Vector3 coinPosSpawn, int _currentCoins, int newCoin, Vector3 beanPosSpawn,int _currentGems, int newBean)
	{
		counter = 0;
		panelAddItemEffGO.SetActive(true);
		
		var currentBean = _currentGems;
		if (currentBean < 1000)
		{
			tmpBeanEffect.SetText(currentBean.ToString());
		}
		else
		{
			tmpBeanEffect.SetText(currentBean.ToString("##,#"));
		}
		

		var currentCoin = _currentCoins;
		itemPrefabGO.SetActive(true);
		if (currentCoin < 1000)
		{
			tmpCoinEffect.SetText(currentCoin.ToString());
		}
		else
		{
			tmpCoinEffect.SetText(currentCoin.ToString("##,#"));
		}
		
		
		yield return null;
		for (int i = 0; i < numOfItem; i++)
		{
			int a = i;
			GameObject clone = null;
			GameObject beanClone = null;
			
			if (i >= itemPool.Count)
			{
				clone = Instantiate(itemPrefabGO, itemPrefabGO.transform.position, Quaternion.identity, panelAddItemEffGO.transform);
				itemPool.Add(clone);
			}
			else
			{
				clone = itemPool[i];
			}
			clone.SetActive(true);

			if (i >= itemBeanPool.Count)
			{
				beanClone = Instantiate(itemHeartPrefabGO, itemHeartPrefabGO.transform.position, Quaternion.identity,
					panelAddItemEffGO.transform);
				itemBeanPool.Add(beanClone);
			}
			else
			{
				beanClone = itemBeanPool[i];
			}
			
			beanClone.SetActive(true);
			
			Vector3 endValue = new Vector3(coinPosSpawn.x + Random.Range(-0.8f, 0.8f), coinPosSpawn.y + Random.Range(-0.4f, 0.4f));
			clone.transform.position = coinPosSpawn;
			clone.transform.DOMove(endValue, 0.2f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				clone.transform.DOMoveY(-0.1f, 0.05f).SetDelay(0.1f + 0.05f * (float)a).SetEase(Ease.InOutSine)
					.SetRelative(isRelative: true)
					.OnComplete(delegate
					{
						clone.transform.DOMove(coinPos.position, 0.75f).SetEase(Ease.InOutSine).OnComplete(delegate
						{
							counter++;
							if (counter < 6 && counter %2 == 1)
                            RecyclePool(clone);
						});
					});
			});
			
			Vector3 endBeanValue = new Vector3(beanPosSpawn.x + Random.Range(-0.8f, 0.8f), beanPosSpawn.y + Random.Range(-0.4f, 0.4f));
			beanClone.transform.position = beanPosSpawn;
			beanClone.transform.DOMove(endBeanValue, 0.2f).SetEase(Ease.InQuad).OnComplete(delegate
			{
				beanClone.transform.DOMoveY(-0.1f, 0.05f).SetDelay(0.1f + 0.05f * (float)a).SetEase(Ease.InOutSine)
					.SetRelative(isRelative: true)
					.OnComplete(delegate
					{
						beanClone.transform.DOMove(gemPos.position, 0.75f).SetEase(Ease.InOutSine).OnComplete(delegate
						{
							RecyclePool(beanClone);
						});
					});
			});
		}

		yield return new WaitForSeconds(0.8f);

		var coin = currentCoin;

        // TODO: play earn coin sound
        //AudioManager.instance.PlaySFX("getcoin");
        // Z_SoundBase.Instance.PlayEarnCoinSound();

        DOTween.To(() => coin, x => coin = x, newCoin, 0.5f)
			.OnUpdate(() =>
			{
				tmpCoinEffect.SetText(coin.ToString("##,#"));
			})
			.OnComplete(() =>
			{
				tmpCoinEffect.SetText(newCoin.ToString("##,#"));
				currentCoin = newCoin;

                //PlayerPrefs.SetInt("LAST_BALL", currentCoin);
            });

		var bean = currentBean;
		DOTween.To(() => bean, x => bean = x, newBean, 0.5f)
			.OnUpdate(() =>
			{
				tmpBeanEffect.SetText(bean.ToString("##,#"));
			})
			.OnComplete(() =>
			{
				tmpBeanEffect.SetText(newBean.ToString("##,#"));
				currentBean = newBean;
				
				//PlayerPrefs.SetInt("TOTAL_POINT", currentBean);
			});
		
		yield return new WaitForSeconds(1f);
		panelAddItemEffGO.SetActive(false);
		
	}

	private void RecyclePool(GameObject itemClone)
	{
		// itemPool.Recycle();
		itemClone.SetActive(false);
	}
}
