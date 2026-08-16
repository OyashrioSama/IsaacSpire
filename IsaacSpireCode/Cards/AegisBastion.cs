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

public sealed class AegisBastion : ModCardTemplate
{
	private const int BaseEnergyCost = 1;
	private const CardType CardKind = CardType.Skill;
	private const CardRarity CardRarityValue = CardRarity.Rare;
	private const TargetType CardTarget = TargetType.Self;
	private const bool ShowInCardLibrary = true;
	protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
	[
		HoverTipFactory.FromPower<PietyPower>(),
		HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
	];
	private int _increasedBlock;
	private int _currentBlock = 1;

	[SavedProperty]
	public int CurrentBlock
	{
		get
		{
			return _currentBlock;
		}
		set
		{
			AssertMutable();
			_currentBlock = value;
			DynamicVars.Block.BaseValue = _currentBlock;
		}
	}

	[SavedProperty]
	public int IncreasedBlock
	{
		get
		{
			return _increasedBlock;
		}
		set
		{
			AssertMutable();
			_increasedBlock = value;
		}
	}
	public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

	public override CardAssetProfile AssetProfile => new(
		PortraitPath: $"{Entry.ResPath}/images/cards/{GetType().Name}.png");

	protected override IEnumerable<DynamicVar> CanonicalVars =>
	[
		new BlockVar(1m, ValueProp.Move),
		new IntVar("Increase", 3)
	];

	public AegisBastion() : base(BaseEnergyCost, CardKind, CardRarityValue, CardTarget, ShowInCardLibrary)
	{
	}

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

		int IncreaseTimes = DynamicVars["Increase"].IntValue;
		var pietyPower = Owner.Creature.GetPower<PietyPower>();
		int pietyAmount = pietyPower?.Amount ?? 0;
		int increase = IncreaseTimes * pietyAmount;
		BuffFromPlay(increase);
		(DeckVersion as AegisBastion)?.BuffFromPlay(increase);
	}

	protected override void OnUpgrade()
	{
		DynamicVars["Increase"].UpgradeValueBy(1);
	}

	protected override void AfterDowngraded()
	{
		UpdateBlock();
	}

	private void BuffFromPlay(int extraBlock)
	{
		IncreasedBlock += extraBlock;
		UpdateBlock();
	}

	private void UpdateBlock()
	{
		CurrentBlock = 1 + IncreasedBlock;
	}
}