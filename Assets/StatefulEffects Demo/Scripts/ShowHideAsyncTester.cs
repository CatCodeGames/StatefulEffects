using CatCode.StatefulEffects;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ShowHideAsyncTester : MonoBehaviour
{
    [SerializeField] private MonoShowHideAsync _showHide;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            Show().Forget();
        if (Input.GetKeyDown(KeyCode.H))
            Hide().Forget();

    }

    private async UniTaskVoid Show()
    {
        var result = await _showHide.ShowAsync();

        Debug.Log($"Show Result = {result}");
    }

    private async UniTaskVoid Hide()
    {
        var result = await _showHide.HideAsync();
        Debug.Log($"Hide Result = {result}");
    }
}
