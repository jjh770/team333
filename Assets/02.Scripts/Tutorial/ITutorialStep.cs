using System;

public interface ITutorialStep
{
    string StepId { get; }
    string InstructionText { get; }
    bool IsCompleted { get; }
    event Action OnCompleted;
    void Enter();
    void Exit();
    void OnUpdate();
}
