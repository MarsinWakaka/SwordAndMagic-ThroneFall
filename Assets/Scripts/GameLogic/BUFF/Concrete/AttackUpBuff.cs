using GameLogic.BUFF.Concrete.Instance;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete
{
    /// <summary>
    /// 攻击强化BUFF
    /// 持有者的攻击力提升[a]点;
    /// </summary>
    [CreateAssetMenu(menuName = "Battle/BUFF/AttackUpBuff")]
    public class AttackUpBuff : Buff
    {
        public int increaseAmount;
        public override string Description { get; set; }
        public override BuffInstance CreateBuffInstance(CharacterUnitController owner)
        {
            return new AttackUpBuffIns(buffID, owner);
        }
    }
}