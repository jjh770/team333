using System;
using UnityEngine;
using UnityEngine.AI;

public class TestInput : MonoBehaviour
{
    [SerializeField] private FloraMovement _movement;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _movement.Resume();
        }
    }
}
