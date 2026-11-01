using UnityEngine;

namespace CatCode.StatefulEffects
{
    [CreateAssetMenu(menuName = "CatCode/ShowHide/AnimatorShowHideConfig")]
    public sealed class AnimatorShowHideConfig : ScriptableObject
    {
        [Header("States")]
        [SerializeField] private string _shownState = "Shown";
        [SerializeField] private string _hiddenState = "Hidden";

        [Header("Triggers")]
        [SerializeField] private string _showTrigger = "show";
        [SerializeField] private string _hideTrigger = "hide";
        [SerializeField] private string _setShownTrigger = "set_shown";
        [SerializeField] private string _setHiddenTrigger = "set_hidden";

        [SerializeField, HideInInspector] private int _shownStateHash;
        [SerializeField, HideInInspector] private int _hiddenStateHash;
        [SerializeField, HideInInspector] private int _showTriggerHash;
        [SerializeField, HideInInspector] private int _hideTriggerHash;
        [SerializeField, HideInInspector] private int _setShownTriggerHash;
        [SerializeField, HideInInspector] private int _setHiddenTriggerHash;

        public int ShownStateHash => _shownStateHash;
        public int HiddenStateHash => _hiddenStateHash;

        public int ShowTriggerHash => _showTriggerHash;
        public int HideTriggerHash => _hideTriggerHash;
        public int SetShownTriggerHash => _setShownTriggerHash;
        public int SetHiddenTriggerHash => _setHiddenTriggerHash;


#if UNITY_EDITOR
        private void OnValidate()
        {
            _shownStateHash = Animator.StringToHash(_shownState);
            _hiddenStateHash = Animator.StringToHash(_hiddenState);
            _showTriggerHash = Animator.StringToHash(_showTrigger);
            _hideTriggerHash = Animator.StringToHash(_hideTrigger);
            _setShownTriggerHash = Animator.StringToHash(_setShownTrigger);
            _setHiddenTriggerHash = Animator.StringToHash(_setHiddenTrigger);
        }
#endif
    }
}
