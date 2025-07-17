using UniRx;

namespace Game.Scripts.Domain.Models
{
    public class LevelProgressModel
    {
        public ReactiveProperty<float> Progress { get; } = new ReactiveProperty<float>(0f);
        public ReactiveProperty<int> CurrentLevel { get; } = new ReactiveProperty<int>(1);
        public ReactiveProperty<float> CurrentExp { get; } = new ReactiveProperty<float>(0f);
    }
}