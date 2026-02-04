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
    COIN_5,
    COIN_6,
    COIN_7,

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
    LimitedPack,
    Coin_0,    
    Coin_1,    
    Coin_2,
    Coin_3,
    Coin_4,
    Coin_5,
    Coin_6,
    Coin_7,
}

public class MG_RewardProduct
{
    public MG_ProductType type;
    public int amount;
    public int bonus;

    public int healBooster;
    public bool removeAds;

    public MG_RewardProduct(MG_ProductType type, int amount, int bonus = 0)
    {
        this.type = type;
        this.amount = amount;
        this.bonus = bonus;
    }

}
public static class MG_ProductData
{
    //package
    public static ProductIPAData NoAds_Pack = new ProductIPAData("Remove_Ads", "dab.removeads", "1.99", "USD");
    public static ProductIPAData Starter_Pack = new ProductIPAData("Starter_Pack", "dab.Starter_Pack", "1.99", "USD");
    public static ProductIPAData Limited_Pack = new ProductIPAData("Limited_Pack", "dab.Limited_Pack", "19.99", "USD");


    //shop  
    public static ProductIPAData Coin_0_Pack = new ProductIPAData("Coin_0", "dab.500c", "0.99", "USD");
    public static ProductIPAData Coin_1_Pack = new ProductIPAData("Coin_1", "dab.1k8c", "1.99", "USD");
    public static ProductIPAData Coin_2_Pack = new ProductIPAData("Coin_2", "dab.3k2c", "4.99", "USD");
    public static ProductIPAData Coin_3_Pack = new ProductIPAData("Coin_3", "dab.6k6c", "9.99", "USD");
    public static ProductIPAData Coin_4_Pack = new ProductIPAData("Coin_4", "dab.13k6c", "19.99", "USD");
    public static ProductIPAData Coin_5_Pack = new ProductIPAData("Coin_5", "dab.40kc", "49.99", "USD");
    public static ProductIPAData Coin_6_Pack = new ProductIPAData("Coin_6", "dab.35kc", "99.99", "USD");
    public static ProductIPAData Coin_7_Pack = new ProductIPAData("Coin_7", "dab.75kc", "49.99", "USD");


    //MG REward
    public static MG_RewardProduct NoAdsReward = new MG_RewardProduct(MG_ProductType.NoAds, 200, 0);
    public static MG_RewardProduct StarterPack = new MG_RewardProduct(MG_ProductType.StaterPack, 1000, 0);
    public static MG_RewardProduct LimitedPack = new MG_RewardProduct(MG_ProductType.LimitedPack, 10000, 0);
    public static MG_RewardProduct Coin_0 = new MG_RewardProduct(MG_ProductType.Coin_1, 500, 0);
    public static MG_RewardProduct Coin_1 = new MG_RewardProduct(MG_ProductType.Coin_1, 1200, 0);
    public static MG_RewardProduct Coin_2 = new MG_RewardProduct(MG_ProductType.Coin_2, 3000, 0);
    public static MG_RewardProduct Coin_3 = new MG_RewardProduct(MG_ProductType.Coin_3, 6000, 0);
    public static MG_RewardProduct Coin_4 = new MG_RewardProduct(MG_ProductType.Coin_4, 12500, 0);
    public static MG_RewardProduct Coin_5 = new MG_RewardProduct(MG_ProductType.Coin_5, 35000, 0);
    public static MG_RewardProduct Coin_6 = new MG_RewardProduct(MG_ProductType.Coin_6, 75000, 0);
    public static MG_RewardProduct Coin_7 = new MG_RewardProduct(MG_ProductType.Coin_7, 100000, 0);


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
            case IAPProductEnum.LIMITED_PACK:
                product = Limited_Pack;
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
            case IAPProductEnum.COIN_5:
                product = Coin_5_Pack;
                break;
                
            case IAPProductEnum.COIN_6:
                product = Coin_6_Pack;
                break;
                
            case IAPProductEnum.COIN_7:
                product = Coin_7_Pack;
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
        else if (productId == Limited_Pack.productId)
        {
            return LimitedPack;
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
        else if(productId == Coin_5_Pack.productId)
        {
            return Coin_5;
        }    
        else if(productId == Coin_6_Pack.productId)
        {
            return Coin_6;
        }    
        else if(productId == Coin_7_Pack.productId)
        {
            return Coin_7;
        }    
        return null;
    }
}
