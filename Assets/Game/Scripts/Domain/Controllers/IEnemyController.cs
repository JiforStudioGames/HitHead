using Game.Scripts.Domain.Enums;
using UnityEngine;

namespace Game.Scripts.Domain.Controllers
{
    public interface IEnemyController
    {
        void Hit(int damage, bool isCrit, Vector3 point, EForceType forceType, EEffectType3D effectType);
    }
}