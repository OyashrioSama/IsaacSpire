using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.CardTags;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace IsaacSpire.Scripts;

[RegisterOwnedCardTag(nameof(Tarot))]
public class CardTags
{
    public static readonly CardTag Tarot = ModContentRegistry.GetQualifiedCardTagId(Entry.ModId, nameof(Tarot)).GetModCardTag();
}