using System;
using UnityEngine;

namespace CatCode.StatefulEffects
{
    [CreateAssetMenu(menuName = "CatCode/ShowHide/AnimatorConfig")]    
    public class AnimatorShowHideConfig : ScriptableObject
    {
        [Serializable]
        public struct AnimatorStateEntry
        {
            public int Hash;
            public ShowHideState State;
        }

        [Header("State Names")]
        [SerializeField] private string _showState = "Show";
        [SerializeField] private string _shownState = "Shown";
        [SerializeField] private string _hideState = "Hide";
        [SerializeField] private string _hiddenState = "Hidden";

        [Header("Trigger Names")]
        [SerializeField] private string _showTrigger = "show";
        [SerializeField] private string _setShownTrigger = "set_shown";
        [SerializeField] private string _hideTrigger = "hide";
        [SerializeField] private string _setHiddenTrigger = "set_hidden";

        [Header("Generated Mapping")]
        [SerializeField, HideInInspector] private AnimatorStateEntry[] _stateEntries;
        [SerializeField, HideInInspector] private int[] _triggerHashes;

        public ReadOnlySpan<AnimatorStateEntry> StateEntries => _stateEntries;
        public ReadOnlySpan<int> TriggerHashes => _triggerHashes;

        public ShowHideState GetStateByHash(int hash)
        {
            foreach (var e in _stateEntries)
                if (e.Hash == hash)
                    return e.State;

            return ShowHideState.Hidden;
        }

        public int GetTriggerForState(ShowHideState state)
        {
            return state switch
            {
                ShowHideState.Show => _triggerHashes[0],
                ShowHideState.Shown => _triggerHashes[1],
                ShowHideState.Hide => _triggerHashes[2],
                ShowHideState.Hidden => _triggerHashes[3],
                _ => 0
            };
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _stateEntries = new[]
            {
                new AnimatorStateEntry { Hash = Animator.StringToHash(_showState),   State = ShowHideState.Show },
                new AnimatorStateEntry { Hash = Animator.StringToHash(_shownState),  State = ShowHideState.Shown },
                new AnimatorStateEntry { Hash = Animator.StringToHash(_hideState),   State = ShowHideState.Hide },
                new AnimatorStateEntry { Hash = Animator.StringToHash(_hiddenState), State = ShowHideState.Hidden }
            };

            _triggerHashes = new[]
            {
                Animator.StringToHash(_showTrigger),
                Animator.StringToHash(_setShownTrigger),
                Animator.StringToHash(_hideTrigger),
                Animator.StringToHash(_setHiddenTrigger)
            };
        }
#endif
    }
}