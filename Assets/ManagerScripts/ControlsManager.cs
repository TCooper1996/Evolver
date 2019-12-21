using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using BehaviorScripts;

using AbilityTuple = 
    System.Tuple<System.Action<BehaviorScripts.IMovementBehavior, Ability.ActionStage>, 
    System.Action<BehaviorScripts.CombatantBehavior, Ability.ActionStage>, 
    System.Action<BehaviorScripts.ShooterBehavior, Ability.ActionStage>>;

public class ControlsManager
{
    private PlayerScript _player;
    private PlayerControls _controls;
    public static Dictionary<string, Ability> AbilityMap;

    public ControlsManager(PlayerScript p)
    {
        _player = p;

        _controls = new PlayerControls();

        //All controls that aren't tied to abilities should be assigned here
        _controls.Gameplay.Move.performed += ctx => _player.SetMove(ctx.ReadValue<Vector2>());
        _controls.Gameplay.Move.canceled += ctx => _player.SetMove(Vector2.zero);

        _controls.Gameplay.Rotate.performed += ctx => _player.SetAim(ctx.ReadValue<Vector2>());
        _controls.Gameplay.Rotate.canceled += ctx => _player.SetAim(Vector2.zero);

        _controls.Gameplay.Pause.performed += ctx => DirectorScript.Pause();

        _controls.Gameplay.Quit.performed += ctx => DirectorScript.Quit();

        _controls.Gameplay.AbsorbPress.performed += ctx => PlayerScript.OnAbsorbInteraction(true);
        _controls.Gameplay.AbsorbRelease.performed += ctx => PlayerScript.OnAbsorbInteraction(false);

        _controls.Gameplay.Reload.performed += ctx => PlayerScript.ShooterComponent.TryReload();

        _controls.Gameplay.SelectNextEnemy.performed += ctx => CanvasScript.SelectNextEnemy();
        _controls.Gameplay.SelectPreviousEnemy.performed += ctx => CanvasScript.SelectPreviousEnemy();

    }

    public void EnableControls()
    {
        _controls?.Gameplay.Enable();
    }

    public void DisableControls()
    {
        _controls?.Gameplay.Disable();
    }

}