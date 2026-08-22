using IsaacSpire.Powers;
using IsaacSpire.Resources;
using IsaacSpire.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Patching.Models;

namespace IsaacSpire.Patches;

public class CreatureDamagePatch : IPatchMethod
{
    public static string PatchId => "isaac_spire_damage_intercept";
    public static string Description => "黑心→魂心→红心扣除顺序";
    public static bool IsCritical => false;

    public static ModPatchTarget[] GetTargets()
    {
        return
        [
            new(typeof(Creature), nameof(Creature.LoseHpInternal))
        ];
    }

    public static bool Prefix(
        Creature __instance,
        ref decimal amount,
        ref int __result)
    {
        try
        {
            var player = __instance.Player;
            if (player == null)
                return true;

            int damage = (int)amount;
            if (damage <= 0)
                return true;

            int soulHearts = SecondaryResourceCmd.Get(player, IsaacResources.SoulHeartId);
            int blackHearts = SecondaryResourceCmd.Get(player, IsaacResources.BlackHeartId);
            int totalExtra = soulHearts + blackHearts;

            LogHelper.Log($"【伤害拦截】玩家受到 {damage} 点伤害, 黑心={blackHearts}, 魂心={soulHearts}");

            if (totalExtra <= 0)
                return true;

            int remaining = damage;

            // ====== 第一步：扣除黑心 ======
            int blackToLose = Math.Min(blackHearts, remaining);
            if (blackToLose > 0)
            {
                remaining -= blackToLose;
                _ = LoseBlackHeartAsync(player, blackToLose);
                LogHelper.Log($"【伤害拦截】扣除 {blackToLose} 点黑心");
            }

            // ====== 第二步：扣除魂心 ======
            if (remaining > 0)
            {
                int soulToLose = Math.Min(soulHearts, remaining);
                if (soulToLose > 0)
                {
                    remaining -= soulToLose;
                    _ = LoseSoulHeartAsync(player, soulToLose);
                    LogHelper.Log($"【伤害拦截】扣除 {soulToLose} 点魂心");
                }
            }

            // ====== 第三步：剩余伤害继续扣红心 ======
            if (remaining > 0)
            {
                amount = remaining;
                LogHelper.Log($"【伤害拦截】剩余 {remaining} 点伤害扣红心");
                return true;
            }
            else
            {
                amount = 0;
                __result = 0;
                LogHelper.Log($"【伤害拦截】伤害被完全吸收");
                return false;
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"【伤害拦截】失败", ex);
            return true;
        }
    }

    private static async Task LoseBlackHeartAsync(MegaCrit.Sts2.Core.Entities.Players.Player player, int amount)
    {
        try
        {
            await SecondaryResourceCmd.Lose(player, IsaacResources.BlackHeartId, amount);
            LogHelper.Log($"【黑心】异步扣除完成: {amount}");
            UpdateHeartPowers(player);
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"【黑心】异步扣除失败", ex);
        }
    }

    private static async Task LoseSoulHeartAsync(MegaCrit.Sts2.Core.Entities.Players.Player player, int amount)
    {
        try
        {
            await SecondaryResourceCmd.Lose(player, IsaacResources.SoulHeartId, amount);
            LogHelper.Log($"【魂心】异步扣除完成: {amount}");
            UpdateHeartPowers(player);
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"【魂心】异步扣除失败", ex);
        }
    }

    private static void UpdateHeartPowers(MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        try
        {
            var creature = player.Creature;
            if (creature == null) return;

            int soulHearts = SecondaryResourceCmd.Get(player, IsaacResources.SoulHeartId);
            int blackHearts = SecondaryResourceCmd.Get(player, IsaacResources.BlackHeartId);

            // 魂心 Power
            var soulPower = creature.GetPower<SoulHeartPower>();
            if (soulHearts > 0)
            {
                if (soulPower == null)
                {
                    var newPower = new SoulHeartPower();
                    // 修复：ApplyInternal(Creature owner, decimal amount, bool silent = false)
                    newPower.ApplyInternal(creature, soulHearts);
                    LogHelper.Log($"【魂心 Power】创建，数量={soulHearts}");
                    soulPower = creature.GetPower<SoulHeartPower>();
                }
                if (soulPower != null)
                {
                    soulPower.SetAmount(soulHearts);
                }
            }
            else
            {
                if (soulPower != null)
                {
                    soulPower.RemoveInternal();
                    LogHelper.Log($"【魂心 Power】移除");
                }
            }

            // 黑心 Power
            var blackPower = creature.GetPower<BlackHeartPower>();
            if (blackHearts > 0)
            {
                if (blackPower == null)
                {
                    var newPower = new BlackHeartPower();
                    // 修复：ApplyInternal(Creature owner, decimal amount, bool silent = false)
                    newPower.ApplyInternal(creature, blackHearts);
                    LogHelper.Log($"【黑心 Power】创建，数量={blackHearts}");
                    blackPower = creature.GetPower<BlackHeartPower>();
                }
                if (blackPower != null)
                {
                    blackPower.SetAmount(blackHearts);
                }
            }
            else
            {
                if (blackPower != null)
                {
                    blackPower.RemoveInternal();
                    LogHelper.Log($"【黑心 Power】移除");
                }
            }

            LogHelper.Log($"【Power更新】魂心={soulHearts}, 黑心={blackHearts}");
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"【Power更新】失败", ex);
        }
    }
}