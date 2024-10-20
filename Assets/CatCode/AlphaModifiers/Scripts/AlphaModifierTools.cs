using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CatCode.AlphaModifiers
{
    public static class AlphaModifierTools
    {
        public static IAlphaModifierStrategy GetAlphaModifierStrategy(GameObject go)
        {
            if (go.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
                return new SpriteAlphaModifierStrategy(spriteRenderer);

            if(go.TryGetComponent<ParticleSystem>(out var particleSystem))
                return new ParticleAlphaModifierStrategy(particleSystem);

            if(go.TryGetComponent<TrailRenderer>(out var trailRenderer))
                return new TrailAlphaModifierStrategy(trailRenderer);
                        
            if (go.TryGetComponent<CanvasGroup>(out var canvasGroup))
                return new CanvasGroupAlphaModifierStrategy(canvasGroup);
            else
            {                
                var graphics = go.GetComponents<Graphic>();
                if (graphics != null && graphics.Length > 0)
                    return new GraphicAlphaModifierStrategy(graphics);
            }
            return null;
        }

        public static IAlphaModifierStrategy[] GetStrategiesFromBranch(GameObject go)
            => GetAllStrategies(go.transform).ToArray();

        private static IEnumerable<IAlphaModifierStrategy> GetAllStrategies(Transform parent)
        {
            var parentStrategy = GetAlphaModifierStrategy(parent.gameObject);
            if (parentStrategy != null)
                yield return parentStrategy;

            var childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (child.TryGetComponent<MonoAlphaModifier>(out var monoAlphaModifier))
                    continue;
                foreach (var childStrategy in GetAllStrategies(child))
                    if (childStrategy != null)
                        yield return childStrategy;
            }
        }
    }

}