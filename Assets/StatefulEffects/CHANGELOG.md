# Changelog

All notable changes to this package are documented in this file.
The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/).

## [3.0.1] - 2026-11-10

### Fixed
- `AnimatorShowHide` stopped reaching `Shown`/`Hidden` after the object was disabled and enabled again: the Animator recreates its `StateMachineBehaviour` instances on enable, so the component now gets `StateChangeBehaviour` and subscribes in `OnEnable` and unsubscribes in `OnDisable`.

## [3.0.0] - 2026-11-01

Breaking release: the class structure was reworked.

### Changed
- ShowHide is rebuilt around a state machine: `CallbackShowHide` hooks, policies, the `None` state.
- `IShowHide.State` is a plain `ShowHideState` with the `OnStateChanged` event.
- The project is a UPM package; DOTween and UniTask are optional integrations.

### Added
- `EffectAction`: one-shot effects (`Ready → Playing → Ready`).
- Async extensions in the UniTask integration.

### Removed
- EventPrimitives dependency, AlphaModifiers integration, script templates.
