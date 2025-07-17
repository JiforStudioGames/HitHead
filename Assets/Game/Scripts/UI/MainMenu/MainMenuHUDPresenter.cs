using Cysharp.Threading.Tasks;
using Game.Scripts.Services;
using Game.Scripts.UI.Core;
using Game.Scripts.UI.Shop;
using UniRx;
using Zenject;

namespace Game.Scripts.UI.MainMenu
{
    public class MainMenuHUDPresenter : UIScreenPresenter<MainMenuHUDView>
    {
        [Inject] private readonly UINavigator _uiNavigator;
        [Inject] private readonly ICurrencyService _currencyService;
        [Inject] private readonly ILevelProgressService _levelProgressService;
        
        protected override UniTask BeforeShow(CompositeDisposable disposables)
        {
            _levelProgressService.Progress
                .Subscribe(value => _view.SetLevelBarProgress(value))
                .AddTo(disposables);
            
            _levelProgressService.CurrentLevel
                .Subscribe(value => _view.SetupLevelBar(value))
                .AddTo(disposables);
            
            _currencyService.CoinsBalance
                .Subscribe(value => _view.SetCoins((int)value))   
                .AddTo(disposables);
            
            _view.SetShopButtonState(false);
            
            _view.OnShopButtonClick
                .Subscribe(_ =>
                {
                    _uiNavigator.Show<ShopPresenter>();
                    _view.SetShopButtonState(true);
                })
                .AddTo(disposables);
            
            _view.OnShopCloseButtonClick
                .Subscribe(_ =>
                {
                    _uiNavigator.Hide<ShopPresenter>();
                    _view.SetShopButtonState(false);
                })
                .AddTo(disposables);
            
            return UniTask.CompletedTask;
        }
    }
}