using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System.IO;

namespace CatCode.StatefulEffects.EditorTools
{
    public static class UniTaskShowHideScriptGenerator
    {
        [MenuItem("Assets/Create/Scripting/ShowHide/MonoShowHide Callback")]
        private static void Create()
        {
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
                0,
                ScriptableObject.CreateInstance<CreateScriptAsset>(),
                "NewMonoShowHide.cs",
                null,
                null);
        }

        private class CreateScriptAsset : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                const string template = @"
using CatCode.Events;
using CatCode.StatefulEffects;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public sealed class #CLASSNAME# : MonoShowHide
{
    private IShowHide _showHide;

    [SerializeField] private ShowHideState _initialState;
    [SerializeField] private bool _ignoreState;

    private void Awake()
    {
        _showHide = new ShowHideCallbackStateMachine(
            _initialState,
            _ignoreState,
            OnShow,
            OnHide,
            OnSetShown,
            OnSetHidden,
            OnStop);
    }

    private void OnShow(Action onCompleted) { }
    private void OnSetShown() { }
    private void OnHide(Action onCompleted) { }    
    private void OnSetHidden() { }
    private void OnStop() { }

    #region IShowHide 

    public override IReadOnlyEventValue<ShowHideState> State => _showHide.State;

    public override void Show() => _showHide.Show();
    public override void SetShown() => _showHide.SetShown();
    public override void Hide() => _showHide.Hide();
    public override void SetHidden() => _showHide.SetHidden();
    public override void Stop() => _showHide.Stop();

    #endregion
}";

                string className = Path.GetFileNameWithoutExtension(pathName);

                string code = template.Replace("#CLASSNAME#", className);

                File.WriteAllText(pathName, code);

                AssetDatabase.Refresh();

                ProjectWindowUtil.ShowCreatedAsset(
                    AssetDatabase.LoadAssetAtPath<Object>(pathName));
            }
        }
    }
}