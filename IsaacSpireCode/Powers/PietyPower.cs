using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Powers;

[RegisterPower]
public sealed class PietyPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Test/images/powers/piety_power.png",
        BigIconPath: "res://Test/images/powers/piety_power.png"
    );

    // 每次受到未被格挡的伤害，只受到90%的伤害
    public override decimal ModifyHpLostAfterOstyLate(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return amount;
        }
        return Math.Floor(0.9m * amount);
    }

    public override async Task AfterModifyingHpLostAfterOsty()
    {
        if (Amount > 0)
        {
            await PowerCmd.Decrement(this);
        }
    }
}