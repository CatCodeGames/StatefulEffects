using CatCode.StatefulEffects;
using System.Threading;
using UnityEngine;

public sealed class ShowHideTester : MonoBehaviour
{
    private CancellationTokenSource _source;
    [SerializeField] private MonoShowHide _showHide;


    private async void Start()
    {
        _source = new CancellationTokenSource();
        _showHide.Hide();
        await _showHide.WaitHiddenToAwaitable(_source.Token);
        Debug.Log("Completed");
    }

    private void OnDestroy()
    {
        _source.Cancel();
        _source.Dispose();
    }
}