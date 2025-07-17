using UniRx;

namespace Game.Scripts.Services
{
    public interface ILevelProgressService
    {
        IReadOnlyReactiveProperty<float> Progress { get; }
        IReadOnlyReactiveProperty<int> CurrentLevel { get; }
        IReadOnlyReactiveProperty<float> CurrentExp { get; }
        int MaxLevel { get; }
        float MaxExpLevel { get; }
        void AddExp(float value);
        void ResetProgress();
    }
}