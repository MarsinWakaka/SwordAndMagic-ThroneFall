using GameLogic.BUFF.Concrete.Instance;
using GameLogic.Unit.Controller;
using UnityEngine;

namespace GameLogic.BUFF.Concrete.Config
{
    [CreateAssetMenu(menuName = "Battle/BUFF/SupportAttackBuff")]
    public class SupportAttackBuff : BuffConfig
    {
        public override BuffInstance CreateBuffInstance(CharacterUnitController owner)
        {
            return new SupportAttackInstance(BuffID, owner);
        }
    }
}