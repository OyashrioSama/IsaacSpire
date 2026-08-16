using IsaacSpire.Characters;
using IsaacSpire.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Scaffolding.Content;

namespace IsaacSpire.Cards;

[RegisterCard(typeof(IsaacCardPool))]

public sealed class FinalJudgment : ModCardTemplate
{
	private const int BaseEnergyCost = 2;
	private const CardType CardKind = CardType.Attack;
	private const CardRarity CardRarityValue = CardRarity.Rare;
	private const TargetType CardTarget = TargetType.AnyEnemy;
	private const bool ShowInCardLibrary = true;
	protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
	[
		HoverTipFactory.FromPower<PietyPower>(),
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
	];
	private int _increasedDamage;
	private int _currentDamage = 13;
	[SavedProperty]
	public int CurrentDamage
	{
		get
		{
			return _currentDamage;
		}
		set
		{
			AssertMutable();
			_currentDamage = value;
			DynamicVars.Damage.BaseValue = _currentDamage;
		}
	}

	[SavedProperty]
	public int IncreasedDamage
	{
		get
		{
			return _increasedDamage;
		}
		set
		{
			AssertMutable();
			_increasedDamage = value;
		}
	}
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

	public override CardAssetProfile AssetProfile => new(
		PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new DamageVar(13m, ValueProp.Move),
		new IntVar("Increase", 2)
	];

	public FinalJudgment() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
		await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
			.FromCard(this, null)
			.Targeting(cardPlay.Target)
			.Execute(choiceContext);
		int IncreaseTimes = DynamicVars["Increase"].IntValue;
		var pietyPower = Owner.Creature.GetPower<PietyPower>();
		int pietyAmount = pietyPower?.Amount ?? 0;
		int increase = IncreaseTimes * pietyAmount;
		BuffFromPlay(increase);
		(DeckVersion as FinalJudgment)?.BuffFromPlay(increase);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Increase"].UpgradeValueBy(1);
	}

	protected override void AfterDowngraded()
	{
		UpdateDamage();
	}

	private void BuffFromPlay(int extraDamage)
	{
		IncreasedDamage += extraDamage;
		UpdateDamage();
	}

	private void UpdateDamage()
	{
		CurrentDamage = 13 + IncreasedDamage;
	}
}