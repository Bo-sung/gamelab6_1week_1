using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public System.Action OnAPressed;
    public System.Action OnDPressed;
    public System.Action OnWPressed;
    public System.Action OnSPressed;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            OnAPressed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            OnDPressed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            OnWPressed?.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            OnSPressed?.Invoke();
        }
    }
}
