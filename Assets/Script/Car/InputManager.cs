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
        
        if (Input.GetKey(KeyCode.A))
        {
            OnAPressed?.Invoke();
        }
        if (Input.GetKey(KeyCode.D))
        {
            OnDPressed?.Invoke();
        }
        if (Input.GetKey(KeyCode.W))
        {
            OnWPressed?.Invoke();
        }
        if (Input.GetKey(KeyCode.S))
        {
            OnSPressed?.Invoke();
        }
    }
}
