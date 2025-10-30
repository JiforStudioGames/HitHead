using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using UniRx;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Services
{
    public class CurrencyService : ICurrencyService, IInitializable
    {
        private const string COINS_KEY = "OMG COINS";
        private const string GEMS_KEY = "WTF GEMS";
        
        [Inject] private readonly CurrencyModel _currencyModel;
        
        public IReadOnlyReactiveProperty<float> CoinsBalance => _currencyModel.Coins;
        public IReadOnlyReactiveProperty<int> GemsBalance => _currencyModel.Gems;

        public bool CanPurchaseCoins(float price) => CoinsBalance.Value >= price;
        public bool CanPurchaseGems(int price) => GemsBalance.Value >= price;
        
        public void Initialize()
        {
            var savedCoins = PlayerPrefs.GetFloat(COINS_KEY, 0);
            _currencyModel.Coins.Value = savedCoins;
            
            var savedGems = PlayerPrefs.GetInt(GEMS_KEY, 0);
            _currencyModel.Gems.Value = savedGems;
        }

        public int GetBalanceByCurrencyType(ECurrencyType currencyType)
        {
            switch (currencyType)
            {
                case ECurrencyType.Coins: return (int)_currencyModel.Coins.Value;
                case ECurrencyType.Gems: return _currencyModel.Gems.Value;
            }
            
            return 0;
        }

        public bool CanPurchaseByCurrencyType(ECurrencyType currencyType, int price)
        {
            switch (currencyType)
            {
                case ECurrencyType.Coins: return _currencyModel.Coins.Value >= price;
                case ECurrencyType.Gems: return _currencyModel.Gems.Value >= price;
            }
            
            return true;
        }

        public void PurchaseByCurrencyType(ECurrencyType currencyType, int price)
        {
            switch (currencyType)
            {
                case ECurrencyType.Coins: 
                    ChangeBalanceCoins(-price);
                    break;
                case ECurrencyType.Gems:
                    ChangeBalanceGems(-price);
                    break;
            }
        }

        public void ChangeBalanceCoins(float value)
        {
            _currencyModel.Coins.Value = Mathf.Clamp(_currencyModel.Coins.Value+value, 0, float.MaxValue);
            PlayerPrefs.SetFloat(COINS_KEY, _currencyModel.Coins.Value);
        }
        
        public void ChangeBalanceGems(int value)
        {
            _currencyModel.Gems.Value = Mathf.Clamp(_currencyModel.Gems.Value+value, 0, int.MaxValue);
            PlayerPrefs.SetInt(GEMS_KEY, _currencyModel.Gems.Value);
        }
    }
}