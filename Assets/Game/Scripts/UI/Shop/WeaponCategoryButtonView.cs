using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI.Shop
{
    public class WeaponCategoryButtonView : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Image _activeSelector;
        
        public IObservable<bool> OnCategoryToggleClick => _toggle.onValueChanged.AsObservable();

        public void SetName(string name)
        {
            _nameText.text = name;
        }

        public void SetToggleGroup(ToggleGroup toggleGroup)
        {
            _toggle.group = toggleGroup;
        }

        public void SetCategoryActive(bool active)
        {
            _activeSelector.gameObject.SetActive(active);
        }
    }
}