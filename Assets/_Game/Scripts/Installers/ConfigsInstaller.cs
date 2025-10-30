using Game.Scripts.Domain.Configs;
using Game.Scripts.Services;
using Zenject;

namespace Game.Scripts.Installers
{
    public class ConfigsInstaller : Installer<ConfigsInstaller>
    {
        private readonly string SHOP_SCREEN_CONFIG_PATH = @"Configs/ShopConfig";
        private readonly string LEVEL_CONFIG = @"Configs/LevelsConfig";
        private readonly string EFFECTS_CONFIG = @"Configs/EffectsConfig";
        private readonly string REWARD_CONFIG = @"Configs/RewardConfig";
        
        public override void InstallBindings()
        {
            Container.Bind<ShopConfig>()
                .FromScriptableObjectResource(SHOP_SCREEN_CONFIG_PATH)
                .AsSingle();
            
            Container.Bind<EffectConfig>()
                .FromScriptableObjectResource(EFFECTS_CONFIG)
                .AsSingle();
            
            Container.Bind<LevelsConfig>()
                .FromScriptableObjectResource(LEVEL_CONFIG)
                .AsSingle();
            
            Container.Bind<RewardConfig>()
                .FromScriptableObjectResource(REWARD_CONFIG)
                .AsSingle();
        }
    }
}