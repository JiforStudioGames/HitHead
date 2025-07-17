using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;

namespace Game.Scripts.Domain.Systems
{
    public static class DamageSystem
    {
        public static (int, bool) CalculateDamage(WeaponConfig weapon)
        {
            int damage = (int)weapon.Stats[EStatsTypes.Damage].Value;
            int spread = 0;
            bool isCrit = false;
            
            bool spreadCheck = weapon.Stats.ContainsKey(EStatsTypes.DamageSpread);
            if (spreadCheck)
            {
                spread = (int)weapon.Stats[EStatsTypes.DamageSpread].Value;
            }
            
            int damageWithSpread = UnityEngine.Random.Range(damage-spread,damage+spread);
            
            bool critCheck = weapon.Stats.ContainsKey(EStatsTypes.Crit);
            if (critCheck)
            {
                int crit = (int)weapon.Stats[EStatsTypes.Crit].Value;
                float critRate = weapon.Stats[EStatsTypes.CritRate].Value;
            
                if (UnityEngine.Random.value <= critRate)
                {
                    isCrit = true;
                    damageWithSpread += damageWithSpread*crit;
                }
            }

            return (damageWithSpread, isCrit);
        }
    }
}