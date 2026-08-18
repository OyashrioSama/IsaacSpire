using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Powers;

[RegisterPower]
public sealed class StigmaAwakeningPower : ModPowerTemplate
{
    private int _strengthPerPiety;
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Test/images/powers/piety_power.png",
        BigIconPath: "res://Test/images/powers/piety_power.png"
    );

    public StigmaAwakeningPower()
    {
        _strengthPerPiety = 1;
    }

    public StigmaAwakeningPower(int strengthPerPiety = 1)
    {
        _strengthPerPiety = strengthPerPiety;
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

        _ = Task.Run(async () =>
        {
            await PowerCmd.Apply<StrengthPower>(
                new ThrowingPlayerChoiceContext(),
                Owner,
                _strengthPerPiety,
                giver,
                cardSource
            );
        });

        return 0m;
    }
}