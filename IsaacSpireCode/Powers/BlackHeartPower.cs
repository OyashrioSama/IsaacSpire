using Godot;
using IsaacSpire.Resources;
using IsaacSpire.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Combat.SecondaryResources;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Powers;

/// <summary>
/// 黑心状态栏显示 - 显示当前黑心数量
/// </summary>
[RegisterPower]
public sealed class BlackHeartPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: $"{Entry.ResPath}/images/powers/black_heart_power.png",
        BigIconPath: $"{Entry.ResPath}/images/powers/black_heart_power.png"
    );

    /// <summary>
    /// 刷新显示（由 HeartResourceTracker 调用）
    /// </summary>
    public void RefreshDisplay()
    {
        var player = Owner?.Player;
        if (player == null) return;

        int amount = SecondaryResourceCmd.Get(player, IsaacResources.BlackHeartId);
        Amount = amount;
        LogHelper.Log($"【黑心 Power】刷新显示: {amount}");
    }
}