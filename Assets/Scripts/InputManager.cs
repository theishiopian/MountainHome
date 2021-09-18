using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputAction move;
    public InputAction jump;
    public InputAction click;
    public InputAction mousePos;

    public static InputManager instance;

    private void OnEnable()
    {
        instance = this;
        move.Enable();
        jump.Enable();
        click.Enable();
        mousePos.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        click.Disable();
        mousePos.Disable();
    }

    public Vector2 GetClickPos()
    {
        return Camera.main.ScreenToWorldPoint(instance.mousePos.ReadValue<Vector2>());
    }
}