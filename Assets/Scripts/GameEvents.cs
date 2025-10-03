using System;
using UnityEngine;


public static class GameEvents
{
        public static Action OnShoot;
        public static Action OnHit;
        public static Action OnPlayerDeath;
        public static Action OnSuckDust;
        public static Action<bool> OnHasSettingsUICoveredUpAugmentUI;
        public static Action OnDamageTaken;
        
        // SETTINGS
        public static Action<int> OnMouseInverted;
}