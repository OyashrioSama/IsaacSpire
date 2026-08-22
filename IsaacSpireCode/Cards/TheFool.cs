using IsaacSpire.Characters;
using IsaacSpire.Scripts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]
public sealed class TheFool : ModCardTemplate
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

    public TheFool() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 1. 选择一张手牌
        CardModel cardModel = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1),
            context: choiceContext,
            player: Owner,
            filter: null,
            source: this
        )).FirstOrDefault();

        if (cardModel == null) return;

        // 2. 从 IsaacCardPool 获取所有卡牌，筛选出塔罗牌
        var allCards = ModelDb.CardPool<IsaacCardPool>()
            .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
            .ToList();

        // 3. 筛选：排除自己 + 通过 Tag 判断是否为塔罗牌
        var tarotCards = allCards
            .Where(c => c.GetType() != typeof(TheFool))
            .Where(c => c.Tags.Any(t => t == CardTags.Tarot))
            .ToList();

        if (tarotCards.Count == 0) return;

        // 4. 随机选一张（这些是 Canonical Model，不能直接用）
        var rng = Owner.RunState.Rng.CombatCardGeneration;
        var selectedTemplate = tarotCards[rng.NextInt(tarotCards.Count)];

        // 5. 关键：通过 CardFactory 从模板创建可玩实例
        // CardFactory.GetForCombat 接受 IEnumerable<CardModel> 作为模板源
        var selectedCards = CardFactory.GetForCombat(
            Owner,
            new[] { selectedTemplate },  // 传入模板实例数组
            1,
            rng
        );
        var selectedCard = selectedCards.FirstOrDefault();

        if (selectedCard == null) return;

        // 6. 如果是升级版，升级选中的卡牌
        if (IsUpgraded)
        {
            CardCmd.Upgrade(selectedCard);
        }

        // 7. 将手牌转换为选中的塔罗牌
        await CardCmd.Transform(cardModel, selectedCard);
    }
}