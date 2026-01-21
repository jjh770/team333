using UnityEngine;

public class TutorialStep_Dash : TutorialStepBase
{
    [Header("Key Settings")]
    [SerializeField] private KeyCode _continueKey = KeyCode.Space;

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
