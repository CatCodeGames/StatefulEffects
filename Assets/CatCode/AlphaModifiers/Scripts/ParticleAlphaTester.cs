using UnityEngine;

public class ParticleAlphaTester : MonoBehaviour
{
    private ParticleSystem.ColorOverLifetimeModule _colorOverLifetimeModule;

    private GradientAlphaKey[] _defaultAlphaKeysMin;
    private GradientAlphaKey[] _defaultAlphaKeysMax;

    private GradientAlphaKey[] _alphaKeysMin;
    private GradientAlphaKey[] _alphaKeysMax;

    [SerializeField][Range(0, 1)] private float _alpha;
    [SerializeField] private ParticleSystem _particleSystem;


    private void Reset()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        var colorOverLifetimeModule = _particleSystem.colorOverLifetime;

        if (!colorOverLifetimeModule.enabled)
            colorOverLifetimeModule.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _colorOverLifetimeModule = _particleSystem.colorOverLifetime;

        if (_colorOverLifetimeModule.color.mode == ParticleSystemGradientMode.TwoGradients)
        {
            _defaultAlphaKeysMin = _colorOverLifetimeModule.color.gradientMin.alphaKeys;
            _alphaKeysMin = new GradientAlphaKey[_defaultAlphaKeysMin.Length];
        }

        _defaultAlphaKeysMax = _colorOverLifetimeModule.color.gradientMax.alphaKeys;
        _alphaKeysMax = new GradientAlphaKey[_defaultAlphaKeysMax.Length];
    }

    void Temp(Gradient gradient, GradientAlphaKey[] defaultAlphaKeys, in float alpha)
    {
        var alphaKeys = gradient.alphaKeys;
        var length = Mathf.Min(alphaKeys.Length, defaultAlphaKeys.Length);
        
        for (int i = 0; i < length; i++)
        {
            ref var alphaKey = ref alphaKeys[i];
            alphaKey.alpha = _defaultAlphaKeysMax[i].alpha * alpha;            
        }

        gradient.alphaKeys = alphaKeys;
    }

    // Update is called once per frame
    void Update()
    {
        var minMaxGradient = _colorOverLifetimeModule.color;
        switch (_colorOverLifetimeModule.color.mode)
        {
            case ParticleSystemGradientMode.Gradient:
                {
                    Temp(minMaxGradient.gradient, _defaultAlphaKeysMax, _alpha);                    
                }
                break;
            case ParticleSystemGradientMode.TwoGradients:
                {
                    Temp(minMaxGradient.gradientMin, _defaultAlphaKeysMin, _alpha);
                    
                    Temp(minMaxGradient.gradientMax, _defaultAlphaKeysMax, _alpha);                    
                }
                break;
        }
    }
}