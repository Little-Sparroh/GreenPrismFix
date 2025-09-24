using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using System;
using System.Reflection;

namespace MycopunkModTemplate;

[BepInPlugin("yourname.mycopunk.greenprismfix", "GreenPrismFix", "1.0.0")]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    private void Awake()
    {
        // Initialize Harmony
        var harmony = new Harmony("yourname.mycopunk.greenprismfix");
        harmony.PatchAll();

        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {harmony.Id} is loaded!");
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
            //Logger.LogError("Failed to cast IGear to MiniCannon");
            return true; // Run original method if cast fails
        }

        Rarity rarity = (Rarity)AccessTools.Field(typeof(UpgradeProperty_MiniCannon_Prism), "rarity").GetValue(__instance);

        // Replicate original logic
        int connectedPrismCountRecursive = (int)AccessTools.Method(
            typeof(UpgradeProperty_MiniCannon_Prism),
            "GetConnectedPrismCountRecursive",
            new[] { typeof(IGear), typeof(UpgradeInstance) }
        ).Invoke(__instance, new object[] { gear, upgrade });
        float num;
        try
        {
            // Try upgrade.GetValue first
            num = (float)AccessTools.Method(
                upgrade.GetType(),
                "GetValue",
                new[] { typeof(Pigeon.Math.Random).MakeByRefType() }
            ).Invoke(upgrade, new object[] { rand });
        }
        catch
        {
            // Fallback to value.GetValue
            object value = AccessTools.Field(typeof(UpgradeProperty_MiniCannon_Prism), "value").GetValue(__instance);
            num = (float)AccessTools.Method(
                value.GetType(),
                "GetValue",
                new[] { typeof(Pigeon.Math.Random).MakeByRefType() }
            ).Invoke(value, new object[] { rand });
        }

        // Log original values
        //Logger.LogInfo($"Before modification: damage = {miniCannon.GunData.damage}, maxDamageRange = {miniCannon.GunData.rangeData.maxDamageRange}, connectedPrismCountRecursive = {connectedPrismCountRecursive}, num = {num}");

        float multiplier = 1f + (float)connectedPrismCountRecursive * num;

        // Replicate original Apply method logic for all rarities
        switch (rarity)
        {
            case Rarity.Standard:
                miniCannon.GunData.rangeData.maxDamageRange *= multiplier; // Modified to range instead of damage
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

        // Log modified values
        //Logger.LogInfo($"After modification: damage = {miniCannon.GunData.damage}, maxDamageRange = {miniCannon.GunData.rangeData.maxDamageRange}, multiplier = {multiplier}");

        return false; // Skip original method to prevent damage modification
    }
}