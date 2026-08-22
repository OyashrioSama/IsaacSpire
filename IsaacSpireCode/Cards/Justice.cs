using IsaacSpire.Characters;
using IsaacSpire.Powers;
using IsaacSpire.Scripts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]
public sealed class Justice : ModCardTemplate
{
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRarityValue = CardRarity.Token;
    private const TargetType CardTarget = TargetType.AllEnemies;
    private const bool ShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override HashSet<CardTag> CanonicalTags => new() { CardTags.Tarot };

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DynamicVar("Power", 1m)
    ];

    public Justice() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {

    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_flying_slash");
        int amount = DynamicVars["Power"].IntValue;
        foreach (Creature enemy in CombatState.HittableEnemies)
        {
            var WeakGain = await PowerCmd.Apply<WeakPower>(choiceContext, enemy, amount, Owner.Creature, this);
            var VulnerableGain = await PowerCmd.Apply<VulnerablePower>(choiceContext, enemy, amount, Owner.Creature, this);
            if (WeakGain != null || VulnerableGain != null)
            {
                await PowerCmd.Apply<PietyPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Power"].UpgradeValueBy(1m);
    }
}