public class ProductIPAData
{
    public string productName;
    public string productId;
    public string priceString;
    public string currencyCode;


    public ProductIPAData() { }

    public ProductIPAData(string productName, string productId, string priceString,
        string currencyCode)
    {
        this.productName = productName;
        this.productId = productId;
        this.priceString = priceString;
        this.currencyCode = currencyCode;
    }
}

public static class MG_ProductData
{
    public static ProductIPAData Starter_Pack = new ProductIPAData("Starter_Pack", "ss.starterpack", "4.99", "USD");
    public static ProductIPAData NoAds_Pack = new ProductIPAData("Remove_Ads", "ss.removeads", "1.99", "USD");
    public static ProductIPAData VIP_Pack = new ProductIPAData("Vip", "ss.vip", "5.99", "USD");
    public static ProductIPAData Diamont_1_Pack = new ProductIPAData("Diamont_1", "ss.g160", "0.99", "USD");
    public static ProductIPAData Diamont_2_Pack = new ProductIPAData("Diamont_2", "ss.g440", "4.99", "USD");
    public static ProductIPAData Diamont_3_Pack = new ProductIPAData("Diamont_3", "ss.g960", "9.99", "USD");
    public static ProductIPAData Diamont_4_Pack = new ProductIPAData("Diamont_3", "ss.g2600", "19.99", "USD");
    public static ProductIPAData Diamont_5_Pack = new ProductIPAData("Diamont_3", "ss.g56000", "49.99", "USD");
    public static ProductIPAData Diamont_6_Pack = new ProductIPAData("Diamont_3", "ss.g12000", "99.99", "USD");

    public static ProductIPAData[] DiamontPacks = new[]
        { Diamont_1_Pack, Diamont_2_Pack, Diamont_3_Pack, Diamont_4_Pack, Diamont_5_Pack, Diamont_6_Pack };

    public static MG_RewardProduct StarterReward = new MG_RewardProduct(3, 5, 10);
    public static MG_RewardProduct NoAdsReward = new MG_RewardProduct(MG_ProductType.NoAds, 0, 20);
    public static MG_RewardProduct Diamont_1_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 2000);
    public static MG_RewardProduct Diamont_2_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 12000);
    public static MG_RewardProduct Diamont_3_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 30000);
    public static MG_RewardProduct Diamont_4_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 65000);
    public static MG_RewardProduct Diamont_5_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 160000);
    public static MG_RewardProduct Diamont_6_Reward = new MG_RewardProduct(MG_ProductType.Diamont, 350000);

    public static MG_RewardProduct[] DiamontRewards = new[]
        { Diamont_1_Reward, Diamont_2_Reward, Diamont_3_Reward, Diamont_4_Reward, Diamont_5_Reward, Diamont_6_Reward };


    public static MG_RewardProduct GetReward(string productId)
    {
        if (productId == Starter_Pack.productId)
        {
            return StarterReward;
        }
        else if (productId == NoAds_Pack.productId)
        {
            return NoAdsReward;
        }
        else if (productId == Diamont_1_Pack.productId)
        {
            return Diamont_1_Reward;
        }
        else if (productId == Diamont_2_Pack.productId)
        {
            return Diamont_2_Reward;
        }
        else if (productId == Diamont_3_Pack.productId)
        {
            return Diamont_3_Reward;
        }
        else if (productId == Diamont_4_Pack.productId)
        {
            return Diamont_4_Reward;
        }
        else if (productId == Diamont_5_Pack.productId)
        {
            return Diamont_5_Reward;
        }
        else if (productId == Diamont_6_Pack.productId)
        {
            return Diamont_6_Reward;
        }

        return null;
    }

}

public enum MG_ProductType
{
    NoAds,
    StaterPack,
    Diamont,
}

public class MG_RewardProduct
{
    public MG_ProductType type;
    public int amount;
    public int bonus;

    public int weaponId;
    public int outfitId;
    public int healBooster;
    public bool removeAds;

    public MG_RewardProduct(MG_ProductType type, int amount, int bonus = 0)
    {
        this.type = type;
        this.amount = amount;
        this.bonus = bonus;
    }

    public MG_RewardProduct(int outfitId, int weaponId, int healBooster)
    {
        this.type = MG_ProductType.StaterPack;
        this.outfitId = outfitId;
        this.weaponId = weaponId;
        this.healBooster = healBooster;
        removeAds = true;

    }
}

