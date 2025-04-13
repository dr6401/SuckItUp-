using System;
using UnityEngine;

public class DustParticleTutorialTracker : MonoBehaviour
{
        private TutorialManager _tutorialManager;

        void Start()
        {
                if (_tutorialManager == null)
                {
                        _tutorialManager = GameObject.FindAnyObjectByType<TutorialManager>();
                }
        }

        private void OnDestroy()
        {
                _tutorialManager.DustDestroyed(this.gameObject);
        }
}
