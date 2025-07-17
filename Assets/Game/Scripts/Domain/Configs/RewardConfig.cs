using UnityEngine;

namespace Game.Scripts.Domain.Configs
{
    [CreateAssetMenu(menuName = "Reward/RewardConfig", fileName = "RewardConfig")]
    public class RewardConfig : ScriptableObject
    {
        public int AmountDamageForCoin;
        public int AmountDamageForExp;

        public int GemsForKill;
    }
}