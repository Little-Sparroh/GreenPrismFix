using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace MycopunkModTemplate;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin
{
    public const string PluginGUID = "sparroh.greenprismfix";
    public const string PluginName = "GreenPrismFix";
    public const string PluginVersion = "1.0.1";

    internal new static ManualLogSource Logger;

    private void Awake()
    {
        var harmony = new Harmony(PluginGUID);
        harmony.PatchAll();

        Logger = base.Logger;
        Logger.LogInfo($"{PluginName} loaded successfully.");
    }
}

[HarmonyPatch]
public class MiniCannonPrismPatch
{
    static MethodInfo TargetMethod()
    {
        return AccessTools.Method(
            typeof(UpgradeProperty_MiniCannon_Prism),
            "Apply",
            new[] { typeof(IGear), typeof(UpgradeInstance), typeof(Pigeon.Math.Random).MakeByRefType() }
        );
    }

    static bool Prefix(UpgradeProperty_MiniCannon_Prism __instance, IGear gear, UpgradeInstance upgrade, ref Pigeon.Math.Random rand)
    {
        MiniCannon miniCannon = gear as MiniCannon;
        if (miniCannon == null)
        {
            return true;
        }

        Rarity rarity = (Rarity)AccessTools.Field(typeof(UpgradeProperty_MiniCannon_Prism), "rarity").GetValue(__instance);

        int connectedPrismCountRecursive = (int)AccessTools.Method(
            typeof(UpgradeProperty_MiniCannon_Prism),
            "GetConnectedPrismCountRecursive",
            new[] { typeof(IGear), typeof(UpgradeInstance) }
        ).Invoke(__instance, new object[] { gear, upgrade });
        float num;
        try
        {
            num = (float)AccessTools.Method(
                upgrade.GetType(),
                "GetValue",
                new[] { typeof(Pigeon.Math.Random).MakeByRefType() }
            ).Invoke(upgrade, new object[] { rand });
        }
        catch
        {
            object value = AccessTools.Field(typeof(UpgradeProperty_MiniCannon_Prism), "value").GetValue(__instance);
            num = (float)AccessTools.Method(
                value.GetType(),
                "GetValue",
                new[] { typeof(Pigeon.Math.Random).MakeByRefType() }
            ).Invoke(value, new object[] { rand });
        }

        float multiplier = 1f + (float)connectedPrismCountRecursive * num;

        switch (rarity)
        {
            case Rarity.Standard:
                miniCannon.GunData.rangeData.maxDamageRange *= multiplier;
                break;
            case Rarity.Rare:
                miniCannon.GunData.spreadData.spreadSize *= multiplier;
                break;
            case Rarity.Epic:
                miniCannon.GunData.fireInterval *= 1f / multiplier;
                break;
            case Rarity.Exotic:
                miniCannon.GunData.damage *= multiplier;
                break;
        }

        return false;
    }
}