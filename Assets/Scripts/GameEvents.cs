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
        public static Action OnLevelCompleted;
        public static Action OnEnteredMainMenu;
        public static Action OnLevelTimeRanOut;
        public static Action OnLowLevelTimer;
        public static Action OnResetHasPlayerDiedInPreviousScene;
        
        // SETTINGS
        public static Action<bool> OnMouseInverted;
        public static Action<bool> OnDifficultyChangedToHardcore;
        public static Action<float> OnSensitivityChanged;
        public static Action<float> OnFOVChanged;
        public static Action<bool> OnWeaponSwitchScrollChanged;
}