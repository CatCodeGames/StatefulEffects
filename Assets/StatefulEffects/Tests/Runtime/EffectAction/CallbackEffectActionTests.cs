using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace CatCode.StatefulEffects.Tests
{
    public sealed class CallbackEffectActionTests
    {
        private GameObject _gameObject;
        private FakeCallbackEffectAction _fake;

        [SetUp]
        public void SetUp()
        {
            _gameObject = new GameObject(nameof(CallbackEffectActionTests));
            _fake = _gameObject.AddComponent<FakeCallbackEffectAction>();
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
            Assert.AreEqual(EffectActionState.None, _fake.State);
        }

        [Test]
        public void OnInitialize_RepeatedAccess()
        {
            _ = _fake.State;
            _fake.Play();
            _fake.Prepare();

            Assert.AreEqual(1, _fake.InitializeCount);
        }

        [Test]
        public void Play_FromNone()
        {
            var states = new List<EffectActionState>();
            _fake.OnStateChanged += states.Add;

            _fake.Play();
            _fake.Completion();

            Assert.AreEqual(EffectActionState.Ready, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnPlay, EffectActionState.None) }, _fake.Hooks);
            CollectionAssert.AreEqual(new[] { EffectActionState.Playing, EffectActionState.Ready }, states);
        }

        [Test]
        public void Play_WhilePlaying_WithOnRepeatIgnore()
        {
            _fake.SetPolicy(ActionPolicy.OnRepeatIgnore);
            _fake.Play();
            _fake.Hooks.Clear();

            _fake.Play();

            CollectionAssert.IsEmpty(_fake.Hooks);
        }

        [Test]
        public void Disable_WhilePlaying()
        {
            _fake.Play();
            _fake.Hooks.Clear();

            _gameObject.SetActive(false);

            Assert.AreEqual(EffectActionState.None, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnStop, EffectActionState.Playing) }, _fake.Hooks);
        }

        [Test]
        public void Disable_FromReady()
        {
            _fake.Prepare();
            _fake.Hooks.Clear();

            _gameObject.SetActive(false);

            Assert.AreEqual(EffectActionState.None, _fake.State);
            CollectionAssert.AreEqual(new[] { (Hook.OnStop, EffectActionState.Ready) }, _fake.Hooks);
        }

        [UnityTest]
        public IEnumerator Destroy_WhilePlaying()
        {
            _fake.Play();
            _fake.Hooks.Clear();

            Object.Destroy(_gameObject);
            yield return null;

            CollectionAssert.AreEqual(new[] { (Hook.OnStop, EffectActionState.Playing) }, _fake.Hooks);
        }
    }
}
