using System;
using Game.Scripts.UI.Core;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.MainMenu
{
    public class MainMenuHUDView: UIScreenView
    {
        [Header("Shop")]
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _shopCloseButton;
        
        [Header("LevelBar")]
        [SerializeField] private Slider _levelBarSlider;
        [SerializeField] private TextMeshProUGUI _levelBarText;
        
        [Header("Currency")]
        [SerializeField] private TextMeshProUGUI _coinsText;
        [SerializeField] private TextMeshProUGUI _gemsText;
        
        public IObservable<Unit> OnShopButtonClick => _shopButton.onClick.AsObservable();
        public IObservable<Unit> OnShopCloseButtonClick => _shopCloseButton.onClick.AsObservable();

        public void SetLevelBarProgress(float value)
        {
            _levelBarSlider.value = value;
        }
        
        public void SetupLevelBar(int newLevel)
        {
            _levelBarSlider.value = 0;
            _levelBarText.text = $"Level: {newLevel}";
        }

        public void SetCoins(int value)
        {
            _coinsText.text = ValueParser.Parse(value);
        }

        public void SetShopButtonState(bool activeShop)
        {
            _shopButton.gameObject.SetActive(!activeShop);
            _shopCloseButton.gameObject.SetActive(activeShop);
        }
    }
}