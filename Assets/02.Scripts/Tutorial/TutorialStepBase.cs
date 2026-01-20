using System;
using UnityEngine;

public abstract class TutorialStepBase : MonoBehaviour, ITutorialStep
{
    [SerializeField] private string _stepId;
    [SerializeField] [TextArea] private string _instructionText;

    public string StepId => _stepId;
    public string InstructionText => _instructionText;
    public bool IsCompleted { get; private set; }

    public event Action OnCompleted;

    public void Enter()
    {
        IsCompleted = false;
        OnEnter();
    }

    public void Exit()
    {
        OnExit();
    }

    public void OnUpdate()
    {
        if (!IsCompleted)
        {
            CheckCompletion();
        }
    }

    protected abstract void OnEnter();
    protected abstract void OnExit();
    protected abstract void CheckCompletion();

    protected void Complete()
    {
        if (IsCompleted) return;

        IsCompleted = true;
        OnCompleted?.Invoke();
    }
}
