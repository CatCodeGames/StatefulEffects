using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class CallbackShowHideTests
    {
        private GameObject _gameObject;
        private FakeCallbackShowHide _fake;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(nameof(CallbackShowHideTests));
            _fake = _gameObject.AddComponent<FakeCallbackShowHide>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_gameObject != null)
                Object.Destroy(_gameObject);
        }

        [Test]
        public void State_BeforeCommands()
        {
            Assert.AreEqual(ShowHideState.None, _fake.State);
        }

        [Test]
        public void OnInitialize_RepeatedAccess()
        {
            _ = _fake.State;
            _fake.Show();
            _fake.Hide();

            Assert.AreEqual(1, _fake.InitializeCount);
        }

        [Test]
        public void Show_FromNone()
        {
            var states = new List<ShowHideState>();
            _fake.OnStateChanged += states.Add;

            _fake.Show();
            _fake.Completion();

            Assert.AreEqual(ShowHideState.Shown, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnShow, ShowHideState.None) }, _fake.Hooks);
            CollectionAssert.AreEqual(new[] { ShowHideState.Showing, ShowHideState.Shown }, states);
        }

        [Test]
        public void Show_FromShown_WithShowOnRepeatRestart()
        {
            _fake.SetPolicy(Policy.ShowOnRepeatRestart);
            _fake.SetShown();
            _fake.Hooks.Clear();

            _fake.Show();

            CollectionAssert.AreEqual(new[] { (Hook.OnSetHidden, ShowHideState.Shown), (Hook.OnShow, ShowHideState.Hidden) }, _fake.Hooks);
        }

        [Test]
        public void Disable_WhileShowing()
        {
            _fake.Show();
            _fake.Hooks.Clear();

            _gameObject.SetActive(false);

            Assert.AreEqual(ShowHideState.None, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnStop, ShowHideState.Showing) }, _fake.Hooks);
        }

        [Test]
        public void Disable_FromShown()
        {
            _fake.SetShown();
            _fake.Hooks.Clear();

            _gameObject.SetActive(false);

            Assert.AreEqual(ShowHideState.None, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnStop, ShowHideState.Shown) }, _fake.Hooks);
        }

        [UnityTest]
        public IEnumerator Destroy_WhileShowing()
        {
            _fake.Show();
            _fake.Hooks.Clear();

            Object.Destroy(_gameObject);
            yield return null;

            CollectionAssert.AreEqual(new[] { (Hook.OnStop, ShowHideState.Showing) }, _fake.Hooks);
        }
    }
}
