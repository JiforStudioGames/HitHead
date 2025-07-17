using Game.Scripts.Domain.Enums;
using UniRx;

namespace Game.Scripts.Services
{
    public interface ICurrencyService
    {
        IReadOnlyReactiveProperty<float> CoinsBalance { get; }
        IReadOnlyReactiveProperty<int> GemsBalance { get; }
        
        int GetBalanceByCurrencyType(ECurrencyType currencyType);
        bool CanPurchaseByCurrencyType(ECurrencyType currencyType, int price);
        void PurchaseByCurrencyType(ECurrencyType currencyType, int price);
        void ChangeBalanceCoins(float value);
        void ChangeBalanceGems(int value);

        bool CanPurchaseCoins(float price);
        bool CanPurchaseGems(int price);
    }
}