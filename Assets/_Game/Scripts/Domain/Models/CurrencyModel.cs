using UniRx;

namespace Game.Scripts.Domain.Models
{
    public class CurrencyModel
    {
        public ReactiveProperty<float> Coins { get; } = new ReactiveProperty<float>(0);
        public ReactiveProperty<int> Gems { get; } = new ReactiveProperty<int>(0);
    }
}