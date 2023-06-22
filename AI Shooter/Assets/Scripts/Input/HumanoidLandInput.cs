using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HumanoidLandInput : MonoBehaviour
{

    public Vector2 MoveInput {get; private set;} = Vector2.zero;
    public Vector2 LookInput {get; private set;} = Vector2.zero;

    InputActions _input;


    private void onEnable()
    {
        _input = new InputActions();
        _input.HumanoidLand.Enable();

        _input.HumanoidLand.Move.performed += SetMove;
        _input.HumanoidLand.Move.canceled += SetMove;

        _input.HumanoidLand.Look.performed += SetLook;
        _input.HumanoidLand.Look.canceled += SetLook;
        print("HumanoidLandInput enabled");

    }

    private void onDisable()
    {
        _input.HumanoidLand.Move.performed -= SetMove;
        _input.HumanoidLand.Move.canceled -= SetMove;

        _input.HumanoidLand.Look.performed -= SetLook;
        _input.HumanoidLand.Look.canceled -= SetLook;

        _input.HumanoidLand.Disable();
        print("HumanoidLandInput disabled");
    }

    void SetMove(InputAction.CallbackContext ctx)
    {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    void SetLook(InputAction.CallbackContext ctx)
    {
        LookInput = ctx.ReadValue<Vector2>();
    }

}
