using IsaacSpire.Characters;
using IsaacSpire.Scripts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]
public sealed class TheEmperor : ModCardTemplate
{
    private const int BaseEnergyCost = 0;
    private const CardType CardKind = CardType.Skill;
    private const CardRarity CardRarityValue = CardRarity.Token;
    private const TargetType CardTarget = TargetType.Self;
    private const bool ShowInCardLibrary = true;
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override CardAssetProfile AssetProfile => new(
        PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

    protected override HashSet<CardTag> CanonicalTags => new() { CardTags.Tarot };

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3)
    ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
    ];

    public TheEmperor() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.BaseValue, Owner);
        IEnumerable<CardModel> enumerable = PileType.Hand.GetPile(Owner).Cards
            .Where((CardModel c) => !c.EnergyCost.CostsX);
        foreach (CardModel item in enumerable)
        {
            if (item.EnergyCost.GetWithModifiers(CostModifiers.None) >= 0)
            {
                item.EnergyCost.SetThisTurnOrUntilPlayed(NextEnergyCost());
                NCard.FindOnTable(item)?.PlayRandomizeCostAnim();
            }
        }
    }

    private int NextEnergyCost()
    {
        // 使用当前战斗的随机数生成器
        var random = Owner.RunState.Rng.CombatCardGeneration;
        return random.NextInt(0, 4);  // 0, 1, 2, 3
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}