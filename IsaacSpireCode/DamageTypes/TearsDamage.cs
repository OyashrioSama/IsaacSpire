using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace IsaacSpire.DamageTypes
{
    /// <summary>
    /// 泪弹伤害类型，用于标识所有由"泪弹射击"及其特效造成的伤害。
    /// 这样遗物（如暗物质、圣光）可以通过检查 Damage 的类型来触发联动效果。
    /// </summary>
    public class TearsDamage : DamageVar
    {
        // 构造函数1：基础伤害
        public TearsDamage(decimal amount) : base(amount, ValueProp.Move)
        {
        }

        // 构造函数2：带数值变化方式（Add/Multiply/Set等）
        public TearsDamage(decimal amount, ValueProp valueProp) : base(amount, valueProp)
        {
        }

        // 可选：提供一个“是否为泪弹伤害”的判断属性，方便其他代码检查
        public bool IsTearsDamage => true;
    }
}