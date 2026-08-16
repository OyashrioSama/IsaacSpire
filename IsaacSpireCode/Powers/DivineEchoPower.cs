using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Powers;

[RegisterPower]
public sealed class DivineEchoPower : ModPowerTemplate
{
    private int _extraPiety;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Test/images/powers/piety_power.png",
        BigIconPath: "res://Test/images/powers/piety_power.png"
    );

    public DivineEchoPower()
    {
        _extraPiety = 1;
    }
    public DivineEchoPower(int extraPiety = 1)
    {
        _extraPiety = extraPiety;
    }

    public override decimal ModifyPowerAmountGivenAdditive(PowerModel power, Creature giver, decimal amount, Creature? target, CardModel? cardSource)
    {
        if (!(power is PietyPower))
        {
            return 0m;
        }
        if (giver != Owner)
        {
            return 0m;
        }

        return _extraPiety;
    }
}