namespace Game.Scripts.Domain.Enums
{
    public enum EWeaponSlotState
    {
        LockedByLevel = 0,
        LockedByVIP = 1,
        AvailableForPurchase = 2,
        LockedByCurrency = 3,
        Selected = 4,
        Unselected = 5
    }
}