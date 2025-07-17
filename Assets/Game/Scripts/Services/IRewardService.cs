namespace Game.Scripts.Services
{
    public interface IRewardService
    {
        void GrantHitReward(int damage);
        void GrantKillReward();
    }
}