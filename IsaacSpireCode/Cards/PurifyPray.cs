using IsaacSpire.Characters;
using IsaacSpire.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]
public sealed class PurifyPray : ModCardTemplate
{
    private const int BaseEnergyCost = 1;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRarityValue = CardRarity.Uncommon;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override HashSet<CardTag> CanonicalTags => new();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<PietyPower>(3m)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromPower<PietyPower>(),
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
    ];

    public PurifyPray() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PietyPower>(choiceContext, Owner.Creature, base.DynamicVars["PietyPower"].BaseValue, Owner.Creature, this);
        if (IsUpgraded)
        {
            CardModel cardModel = (await CardSelectCmd.FromHand(
                    prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
                    context: choiceContext,
                    player: Owner,
                    filter: null,
                    source: this
                )
            ).FirstOrDefault();
            if (cardModel != null)
            {
                await CardCmd.Exhaust(choiceContext, cardModel);
            }
            return;
        }
        CardPile pile = PileType.Hand.GetPile(Owner);
        CardModel cardModel2 = Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
        if (cardModel2 != null)
        {
            await CardCmd.Exhaust(choiceContext, cardModel2);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
