using Game.Scripts.Domain.Configs;
using Zenject;

namespace Game.Scripts.Services
{
    public class RewardService : IRewardService
    {
        [Inject] private readonly ICurrencyService _currencyService;
        [Inject] private readonly ILevelProgressService _levelProgressService;
        [Inject] private readonly RewardConfig _rewardConfig;
        
        public void GrantHitReward(int damage)
        {
            float coins = (float)damage / _rewardConfig.AmountDamageForCoin;
            float xp = (float)damage / _rewardConfig.AmountDamageForExp;
            if (coins > 0) 
                _currencyService.ChangeBalanceCoins(coins);
            if (xp > 0)
                _levelProgressService.AddExp(xp);
        }

        public void GrantKillReward()
        {
            _currencyService.ChangeBalanceGems(_rewardConfig.GemsForKill);
        }
    }
}