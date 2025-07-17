using TMPro;
using UnityEngine;

namespace Game.Scripts.UI.Shop.WeaponInfo
{
    public class WeaponStatView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameStat;
        [SerializeField] private TextMeshProUGUI _valueStat;

        public void SetupStat(string statName, string statValue)
        {
            _nameStat.text = statName;
            _valueStat.text = statValue;
        }
    }
}