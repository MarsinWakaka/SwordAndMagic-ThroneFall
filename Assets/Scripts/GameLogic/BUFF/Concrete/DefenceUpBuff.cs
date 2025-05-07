using GameLogic.BUFF.Concrete.Instance;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete
{
    /// <summary>
    /// 掩护BUFF
    /// 当友方受到攻击时，若攻击者处于BUFF持有者的攻击范围内，则BUFF会对攻击者造成[a]点伤害，并使攻击者此次攻击伤害减少[b]点。
    /// 生效一次，最多叠加[c]次。
    /// </summary>
    [CreateAssetMenu(menuName = "Battle/BUFF/CoverBuff")]
    public class DefenceUpBuff : Buff
    {
        public int increaseAmount;
        public override string Description { get; set; }
        
        public override BuffInstance CreateBuffInstance(CharacterUnitController owner)
        {
            return new DefenceUpBuffIns(buffID, owner);
        }
    }
}