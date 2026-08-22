using System.Reflection;
using IsaacSpire.Patches;
using IsaacSpire.Resources;
using IsaacSpire.Utils;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using STS2RitsuLib;
using STS2RitsuLib.Interop;
using STS2RitsuLib.Patching.Core;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace IsaacSpire;

[ModInitializer(nameof(Initialize))]
public partial class Entry
{
    public const string ModId = "IsaacSpire";
    public const string ResPath = $"res://{ModId}";

    public static Logger Logger { get; } = new(ModId, LogType.Generic);

    public static void Initialize()
    {
        var assembly = Assembly.GetExecutingAssembly();

        RitsuLibFramework.EnsureGodotScriptsRegistered(assembly, Logger);
        ModTypeDiscoveryHub.RegisterModAssembly(ModId, assembly);

        LogHelper.Log("========== IsaacSpire 初始化开始 ==========");

        try
        {
            IsaacResources.Register();
            LogHelper.Log($"IsaacResources 注册完成");
        }
        catch (Exception ex)
        {
            LogHelper.LogError("注册失败", ex);
        }

        // ============ 注册伤害拦截补丁 ============
        try
        {
            var patcher = RitsuLibFramework.CreatePatcher(ModId, "damage-patches");
            patcher.RegisterPatch<CreatureDamagePatch>();

            if (!patcher.PatchAll())
            {
                LogHelper.LogError("【伤害拦截】补丁注册失败");
            }
            else
            {
                LogHelper.Log("【伤害拦截】补丁注册成功");
            }
        }
        catch (Exception ex)
        {
            LogHelper.LogError("【伤害拦截】补丁注册失败", ex);
        }

        LogHelper.Log("========== IsaacSpire 初始化完成 ==========");
        Logger.Info("IsaacSpire initialized.");
    }
}