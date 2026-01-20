using UnityEngine;

public class TutorialStep_Instruction : TutorialStepBase
{
    [Header("Key Settings")]
    [SerializeField] private KeyCode _continueKey = KeyCode.E;
    
    protected override void OnEnter() { }

    protected override void OnExit() { }

    protected override void CheckCompletion()
    {
        if (Input.GetKeyDown(_continueKey))
        {
            Complete();
        }
    }
}
