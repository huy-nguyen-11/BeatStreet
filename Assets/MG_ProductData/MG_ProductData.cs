using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ProductIPAData;
public enum IAPProductEnum
{
    NO_ADS,
    STARTER_PACKAGE,
    LIMITED_PACK,
    COIN_0,
    COIN_1,
    COIN_2,
    COIN_3,
    COIN_4,
    GEM_0,
    GEM_1,
    GEM_2,
    GEM_3,
    GEM_4,
}
public class ProductIPAData
{

    public string productName;
    public string productId;
    public string priceString;
    public string currencyCode;
    
    
    public ProductIPAData() {}

    public ProductIPAData(string productName, string productId, string priceString,
        string currencyCode)
    {
        this.productName = productName;
        this.productId = productId;
        this.priceString = priceString;
        this.currencyCode = currencyCode;
    }
}
public enum MG_ProductType
{
    NoAds,
    StaterPack,
    Coin_0,    
    Coin_1,    
    Coin_2,
    Coin_3,
    Coin_4,
    Gem_0,
    Gem_1,
    Gem_2,
    Gem_3,
    Gem_4,
}


public class MG_RewardProduct
{
    public MG_ProductType type;
    public int amount;
    public int bonusCoin;

    public int healBooster;
    public bool removeAds;

    public MG_RewardProduct(MG_ProductType type, int amount, int bonus = 0)
    {
        this.type = type;
        this.amount = amount;
        this.bonusCoin = bonus;
    }
}
public static class MG_ProductData
{
    //package
    public static ProductIPAData NoAds_Pack = new ProductIPAData("Remove Ads", "dab.removeads", "2.99", "$");
    public static ProductIPAData Starter_Pack = new ProductIPAData("Starter Pack", "dab.Starter_Pack", "2.99", "$");

    //shop  
    public static ProductIPAData Coin_0_Pack = new ProductIPAData("Coin 1", "dab.500c", "30", "");
    public static ProductIPAData Coin_1_Pack = new ProductIPAData("Coin 2", "dab.1k8c", "100", "");
    public static ProductIPAData Coin_2_Pack = new ProductIPAData("Coin 3", "dab.3k2c", "300", "");
    public static ProductIPAData Coin_3_Pack = new ProductIPAData("Coin 4", "dab.6k6c", "500", "");
    public static ProductIPAData Coin_4_Pack = new ProductIPAData("Coin 5", "dab.13k6c", "1000", "");
    public static ProductIPAData Gem_0_Pack = new ProductIPAData("Gem 1", "dab.1xx", "0.99", "$");
    public static ProductIPAData Gem_1_Pack = new ProductIPAData("Gem 2", "dab.2xx", "1.99", "$");
    public static ProductIPAData Gem_2_Pack = new ProductIPAData("Gem 3", "dab.3xx", "4.99", "$");
    public static ProductIPAData Gem_3_Pack = new ProductIPAData("Gem 4", "dab.4xx", "9.99", "$");
    public static ProductIPAData Gem_4_Pack = new ProductIPAData("Gem 5", "dab.5xx", "19.99", "$");

    //MG REward
    public static MG_RewardProduct NoAdsReward = new MG_RewardProduct(MG_ProductType.NoAds, 1000, 0);
    public static MG_RewardProduct StarterPack = new MG_RewardProduct(MG_ProductType.StaterPack, 100, 2000);
    public static MG_RewardProduct Coin_0 = new MG_RewardProduct(MG_ProductType.Coin_1, 330, 0);
    public static MG_RewardProduct Coin_1 = new MG_RewardProduct(MG_ProductType.Coin_1, 1100, 0);
    public static MG_RewardProduct Coin_2 = new MG_RewardProduct(MG_ProductType.Coin_2, 3600, 0);
    public static MG_RewardProduct Coin_3 = new MG_RewardProduct(MG_ProductType.Coin_3, 6000, 0);
    public static MG_RewardProduct Coin_4 = new MG_RewardProduct(MG_ProductType.Coin_4, 13200, 0);
    public static MG_RewardProduct Gem_0 = new MG_RewardProduct(MG_ProductType.Gem_0, 100, 0);
    public static MG_RewardProduct Gem_1 = new MG_RewardProduct(MG_ProductType.Gem_1, 220, 0);
    public static MG_RewardProduct Gem_2 = new MG_RewardProduct(MG_ProductType.Gem_2, 600, 0);
    public static MG_RewardProduct Gem_3 = new MG_RewardProduct(MG_ProductType.Gem_3, 1200, 0);
    public static MG_RewardProduct Gem_4 = new MG_RewardProduct(MG_ProductType.Gem_4, 2500, 0);

    //
    public static ProductIPAData GetProductData(IAPProductEnum iapProductEnum)
    {
        ProductIPAData product = NoAds_Pack;
        switch (iapProductEnum)
        {
            case IAPProductEnum.NO_ADS:
                product = NoAds_Pack;
                break;
            case IAPProductEnum.STARTER_PACKAGE:
                product = Starter_Pack;
                break;
            case IAPProductEnum.COIN_0:
                product = Coin_0_Pack;
                break;
            case IAPProductEnum.COIN_1:
                product = Coin_1_Pack;
                break;
            case IAPProductEnum.COIN_2:
                product = Coin_2_Pack;
                break;
            case IAPProductEnum.COIN_3:
                product = Coin_3_Pack;
                break;
            case IAPProductEnum.COIN_4:
                product = Coin_4_Pack;
                break;
            case IAPProductEnum.GEM_0:
                product = Gem_0_Pack;
                break;
            case IAPProductEnum.GEM_1:
                product = Gem_1_Pack;
                break;
            case IAPProductEnum.GEM_2:
                product = Gem_2_Pack;
                break;
            case IAPProductEnum.GEM_3:
                product = Gem_3_Pack;
                break;
            case IAPProductEnum.GEM_4:
                product = Gem_4_Pack;
                break;

        }

        return product;
    }

    public static MG_RewardProduct GetReward(string productId)
    {
        if (productId == NoAds_Pack.productId)
        {
            return NoAdsReward;
        }
        else if (productId == Starter_Pack.productId)
        {
            return StarterPack;
        }
        else if (productId == Coin_0_Pack.productId)
        {
            return Coin_0;
        }
        else if (productId == Coin_1_Pack.productId)
        {
            return Coin_1;
        }
        else if(productId == Coin_2_Pack.productId)
        {
            return Coin_2;
        }  
        else if(productId == Coin_3_Pack.productId)
        {
            return Coin_3;
        }  
        else if(productId == Coin_4_Pack.productId)
        {
            return Coin_4;
        }      
        else if(productId == Gem_0_Pack.productId)
        {
            return Gem_0;
        }  
        else if(productId == Gem_1_Pack.productId)
        {
            return Gem_1;
        }  
        else if(productId == Gem_2_Pack.productId)
        {
            return Gem_2;
        }  
        else if(productId == Gem_3_Pack.productId)
        {
            return Gem_3;
        }  
        else if(productId == Gem_4_Pack.productId)
        {
            return Gem_4;
        }
        return null;
    }
}
