using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Powers;

[RegisterPower]
public sealed class ChariotPower : ModPowerTemplate
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override PowerAssetProfile AssetProfile => new(
        IconPath: "res://Test/images/powers/chariot_power.png",
        BigIconPath: "res://Test/images/powers/chariot_power.png"
    );

    public override decimal ModifyHpLostAfterOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && props.IsPoweredAttack() && dealer != null)
        {
            _ = Task.Run(async () =>
                    {
                        await CreatureCmd.Damage(
                            new ThrowingPlayerChoiceContext(),
                            target: dealer,
                            amount: amount,
                            props: ValueProp.Unpowered,
                            cardSource: null,
                            dealer: Owner,
                            cardPlay: null
                        );
                    }
                );
        }
        if (!CombatManager.Instance.IsInProgress) return amount;
        if (target != Owner) return amount;
        if (amount < 1m) return amount;
        return 0m;
    }

    public override Task AfterModifyingHpLostAfterOsty()
    {
        Flash();
        return Task.CompletedTask;
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        if (card.Owner != Owner.Player) return true;
        if (card.Pile?.Type != PileType.Hand) return true;
        if (card.Type == CardType.Attack)
        {
            if (autoPlayType != AutoPlayType.None) return true;
            return false;
        }
        return true;
    }

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (participants.Contains(Owner))
        {
            await PowerCmd.Decrement(this);
        }
    }
}