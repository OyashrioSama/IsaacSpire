using IsaacSpire.Characters;
using IsaacSpire.Powers;
using IsaacSpire.Scripts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]
public sealed class WheelOfFortune : ModCardTemplate
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

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(3m, ValueProp.Move),
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("Draw", 1m),
        new DynamicVar("Energy", 1m),
        new DynamicVar("Piety", 1m)
    ];

    public WheelOfFortune() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获取所有效果的数值
        var damage = DynamicVars.Damage.BaseValue;
        var block = DynamicVars.Block;
        int draw = DynamicVars["Draw"].IntValue;
        int energy = DynamicVars["Energy"].IntValue;
        int piety = DynamicVars["Piety"].IntValue;

        // 随机选择 0-4
        int roll = Owner.RunState.Rng.CombatCardGeneration.NextInt(5);

        switch (roll)
        {
            case 0: // 群体伤害
                foreach (var enemy in CombatState.HittableEnemies)
                {
                    await DamageCmd.Attack(damage)
                        .FromCard(this, null)
                        .Targeting(enemy)
                        .Execute(choiceContext);
                }
                break;

            case 1: // 格挡
                await CreatureCmd.GainBlock(Owner.Creature, block, cardPlay);
                break;

            case 2: // 抽牌
                await CardPileCmd.Draw(choiceContext, draw, Owner);
                break;

            case 3: // 获得能量
                await PlayerCmd.GainEnergy(energy, Owner);
                break;

            case 4: // 获得虔诚
                await PowerCmd.Apply<PietyPower>(choiceContext, Owner.Creature, piety, Owner.Creature, this);
                break;
        }
    }

    protected override void OnUpgrade()
    {
        // 升级：所有效果数值 +1
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars.Block.UpgradeValueBy(1m);
        DynamicVars["Draw"].UpgradeValueBy(1m);
        DynamicVars["Energy"].UpgradeValueBy(1m);
        DynamicVars["Piety"].UpgradeValueBy(1m);
    }
}