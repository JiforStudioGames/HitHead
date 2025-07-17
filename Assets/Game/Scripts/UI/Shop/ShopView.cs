using System.Collections.Generic;
using Game.Scripts.Domain.Configs;
using Game.Scripts.Domain.Enums;
using Game.Scripts.Domain.Models;
using Game.Scripts.UI.Core;
using Game.Scripts.UI.Shop.WeaponInfo;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
    public class ShopView : UIScreenView
    {
        [Header("Categories")]
        [SerializeField] private ToggleGroup _categoriesToggleGroup;
        [SerializeField] private Transform _categoriesContent;
        [SerializeField] private WeaponCategoryButtonView _categoryPrefab;
        
        [Header("Items")]
        [SerializeField] private ToggleGroup _slotsToggleGroup;
        [SerializeField] private Transform _slotsContent;
        [SerializeField] private WeaponSlotView _slotPrefab;
        
        [Header("WeaponInfo")]
        [SerializeField] private WeaponInfoView _weaponInfoView;
        
        private List<WeaponCategoryButtonView> _categories = new List<WeaponCategoryButtonView>();
        private List<WeaponSlotView> _slots = new List<WeaponSlotView>();

        public WeaponCategoryButtonView SpawnCategory(EShopCategory categoryType)
        {
            WeaponCategoryButtonView category = Instantiate(_categoryPrefab, _categoriesContent);
            category.SetName(categoryType.ToString());
            category.SetToggleGroup(_categoriesToggleGroup);
            _categories.Add(category);
            return category;
        }
        
        public WeaponSlotView SpawnWeaponSlot(WeaponConfig config)
        {
            WeaponSlotView slot = Instantiate(_slotPrefab, _slotsContent);
            slot.SetToggleGroup(_slotsToggleGroup);
            _slots.Add(slot);
            return slot;
        }
        
        public void ClearCategories()
        {
            foreach (var c in _categories)
            {
                Destroy(c.gameObject);
            }
            _categories.Clear();
        }

        public void ClearSlots()
        {
            foreach (var s in _slots)
            {
                Destroy(s.gameObject);
            }
            _slots.Clear();
        }

        public void SetupWeaponInfo(WeaponConfig config)
        {
            _weaponInfoView.SetupWeaponInfo(config);
        }
    }
}