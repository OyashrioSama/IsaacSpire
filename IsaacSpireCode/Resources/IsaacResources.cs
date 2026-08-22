using Godot;
using STS2RitsuLib;
using STS2RitsuLib.Combat.SecondaryResources;

namespace IsaacSpire.Resources;

public static class IsaacResources
{
    public static SecondaryResourceDefinition SoulHeart { get; private set; } = null!;
    public static SecondaryResourceDefinition BlackHeart { get; private set; } = null!;
    public static string SoulHeartId => SoulHeart.Id;
    public static string BlackHeartId => BlackHeart.Id;

    public static void Register()
    {
        try
        {
            var registry = RitsuLibFramework.GetSecondaryResourceRegistry(Entry.ModId);
            GD.Print("IsaacSpire: Got registry");

            // 魂心：跨战斗保留，每回合不自动恢复
            SoulHeart = registry.Register("soul_heart", new SecondaryResourceDefinition(
                defaultAmount: 0,
                baseMaxAmount: 999,
                turnStartPolicy: SecondaryResourceTurnStartPolicy.None,
                persistencePolicy: SecondaryResourcePersistencePolicy.Run,
                smallIconPath: $"{Entry.ResPath}/images/resources/soul_heart.png",
                largeIconPath: $"{Entry.ResPath}/images/resources/soul_heart.png"
            ));
            GD.Print($"IsaacSpire: Registered SoulHeart, Id={SoulHeart.Id}");

            // 黑心：跨战斗保留，每回合不自动恢复
            BlackHeart = registry.Register("black_heart", new SecondaryResourceDefinition(
                defaultAmount: 0,
                baseMaxAmount: 999,
                turnStartPolicy: SecondaryResourceTurnStartPolicy.None,
                persistencePolicy: SecondaryResourcePersistencePolicy.Run,
                smallIconPath: $"{Entry.ResPath}/images/resources/black_heart.png",
                largeIconPath: $"{Entry.ResPath}/images/resources/black_heart.png"
            ));
            GD.Print($"IsaacSpire: Registered BlackHeart, Id={BlackHeart.Id}");
        }
        catch (Exception ex)
        {
            GD.PrintErr($"IsaacSpire: Register failed: {ex.Message}");
        }
    }
}