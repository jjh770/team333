using System;
using UnityEngine;

public class TutorialStep_Movement : TutorialStepBase
{
    private bool _wPressed;
    private bool _aPressed;
    private bool _sPressed;
    private bool _dPressed;

    public bool WPressed => _wPressed;
    public bool APressed => _aPressed;
    public bool SPressed => _sPressed;
    public bool DPressed => _dPressed;

    public event Action OnKeyStateChanged;

    protected override void OnEnter()
    {
        _wPressed = false;
        _aPressed = false;
        _sPressed = false;
        _dPressed = false;
    }

    protected override void OnExit()
    {
        // Nothing to clean up
    }

    protected override void CheckCompletion()
    {
        bool stateChanged = false;

        if (!_wPressed && Input.GetKeyDown(KeyCode.W))
        {
            _wPressed = true;
            stateChanged = true;
        }

        if (!_aPressed && Input.GetKeyDown(KeyCode.A))
        {
            _aPressed = true;
            stateChanged = true;
        }

        if (!_sPressed && Input.GetKeyDown(KeyCode.S))
        {
            _sPressed = true;
            stateChanged = true;
        }

        if (!_dPressed && Input.GetKeyDown(KeyCode.D))
        {
            _dPressed = true;
            stateChanged = true;
        }

        if (stateChanged)
        {
            OnKeyStateChanged?.Invoke();
        }

        if (_wPressed && _aPressed && _sPressed && _dPressed)
        {
            Complete();
        }
    }
}
