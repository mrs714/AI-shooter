using System;
using UnityEngine;

public class HumanoidLandController : MonoBehaviour 
{   
    Rigidbody _rigidbody = null;
    [SerializeField] HumanoidLandInput _input;
    
    Vector3 _playerMoveinput;

    [Header("Movement")]
    [SerializeField] float _movementMultiplier = 30f;

    private void Awake() 
    {
        print("HumanoidLandController awake");
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() 
    {
        _playerMoveinput = GetMoveInput();
        PlayerMove();

        _rigidbody.AddRelativeForce(_playerMoveinput, ForceMode.Force);
    }

    private Vector3 GetMoveInput()
    {
        //print("HumanoidLandController GetMoveInput:" + _input.MoveInput.x + "," + _input.MoveInput.y);
        return new Vector3(_input.MoveInput.x, 0.0f, _input.MoveInput.y);
    }

    private void PlayerMove()
    {
        _playerMoveinput = (new Vector3(_playerMoveinput.x * _movementMultiplier * _rigidbody.mass, 
                                        _playerMoveinput.y,
                                        _playerMoveinput.z * _movementMultiplier * _rigidbody.mass));
    }
}