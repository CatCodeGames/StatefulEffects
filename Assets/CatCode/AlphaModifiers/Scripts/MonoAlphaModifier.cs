using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CatCode.AlphaModifiers
{
    [ExecuteInEditMode]
    public class MonoAlphaModifier : MonoBehaviour, IAlphaModifier
    {
        [SerializeField] private MonoAlphaModifier _parent;
        [SerializeField] private List<MonoAlphaModifier> _children = new();
        [Space]
        [SerializeField, Range(0, 1)] private float _alpha = 1f;
        [SerializeReference] private IAlphaModifierStrategy[] _alphaStrategies = new IAlphaModifierStrategy[0];


        public float Alpha
        {
            get => _alpha;
            set
            {
                _alpha = value;
                var totalAlpha = TotalAlpha;
                UpdateStrategiesAlpha(totalAlpha);
                for (int i = 0; i < _children.Count; i++)
                    _children[i].UpdateAlpha();
            }
        }

        public void SetParent(MonoAlphaModifier parent)
        {
            if (_parent != null && _parent != parent)
                _parent.RemoveChild(this);
            _parent = parent;
            if (_parent != null)
                _parent.AddChild(this);
        }

        public void RemoveParent()
            => SetParent(null);

        public void FindParent()
        {
            var parent = transform.parent;
            if (parent == null)
                return;
            SetParent(parent.GetComponentInParent<MonoAlphaModifier>(true));
        }

        public void FindChildren()
        {
            _children.Clear();
            var components = GetComponentsInChildren<MonoAlphaModifier>();
            if (components == null || components.Length == 0)
                return;
            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                if (component.gameObject == gameObject)
                    continue;
                _children.Add(component);
                component.SetParent(this);
            }
        }



        private float TotalAlpha => _alpha * (_parent != null ? _parent.TotalAlpha : 1f);

        private void UpdateAlpha()
        {
            var totalAlpha = TotalAlpha;
            UpdateStrategiesAlpha(totalAlpha);
            for (int i = 0; i < _children.Count; i++)
                _children[i].UpdateAlpha();
        }

        private void UpdateStrategiesAlpha(float alpha)
        {
            for (int i = 0; i < _alphaStrategies.Length; i++)
                _alphaStrategies[i].SetAlpha(alpha);
        }



        private void AddChild(MonoAlphaModifier alphaModifier)
        {
            if (_children.Contains(alphaModifier))
                return;
            _children.Add(alphaModifier);
        }

        private void RemoveChild(MonoAlphaModifier alphaModifier)
        {
            _children.Remove(alphaModifier);
        }



        private void OnDestroy()
        {
            RemoveParent();
            if (_children == null)
                return;

            var tempChildrenList = ListPool<MonoAlphaModifier>.Get();
            tempChildrenList.AddRange(_children);
            foreach (var child in tempChildrenList)
                child.SetParent(null);
            ListPool<MonoAlphaModifier>.Release(tempChildrenList);
        }

#if UNITY_EDITOR

        private void Awake()
        {
            if (Application.isPlaying)
                return;
            FindParent();
            FindChildren();
        }

        private void Update()
        {
            if (Application.isPlaying)
                UpdateAlpha();
        }


        [ContextMenu("Alpha for this Branch")]
        public void GetAllStrategies()
            => _alphaStrategies = AlphaModifierTools.GetStrategiesFromBranch(gameObject);

        [ContextMenu("Alpha for this Object")]
        public void GetCurrentStrategy()
            => _alphaStrategies = new[] { AlphaModifierTools.GetAlphaModifierStrategy(gameObject) };

#endif
    }
}