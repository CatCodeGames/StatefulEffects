using System;
using UnityEngine;
using UnityEngine.UI;

namespace CatCode.AlphaModifiers
{
    [Serializable]
    public class GraphicAlphaModifierStrategy : IAlphaModifierStrategy
    {
        [Serializable]
        public struct AlphaGraphicData
        {
            public float alpha;
            public Graphic graphic;


            public AlphaGraphicData(float alpha, Graphic graphic)
            {
                this.alpha = alpha;
                this.graphic = graphic;
            }
        }


        [SerializeField] private AlphaGraphicData[] _graphics;

        public GraphicAlphaModifierStrategy(Graphic graphic) : this(new Graphic[] { graphic })
        { }

        public GraphicAlphaModifierStrategy(Graphic[] graphics)
        {
            _graphics = new AlphaGraphicData[graphics.Length];
            for (int i = 0; i < _graphics.Length; i++)
                _graphics[i] = new AlphaGraphicData(graphics[i].color.a, graphics[i]);
        }

        public void Initialize()
        {
        }

        public void SetAlpha(float value)
        {
            for (int i = 0; i < _graphics.Length; i++)
            {
                var graphicData = _graphics[i];
                var alpha = Mathf.Clamp(graphicData.alpha * value, 0f, 1f);

                var color = graphicData.graphic.color;
                color.a = alpha;
                graphicData.graphic.color = color;
            }
        }
    }
}