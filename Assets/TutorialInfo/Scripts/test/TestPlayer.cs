using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayer : MonoBehaviour
{
    private Rigidbody _rb;
    private Keyboard _keyboard;

    private void Start()
    {
        if (GetComponent<Rigidbody>() is Rigidbody RB) _rb = RB;
        else Debug.LogError("TestPlayer:RigidBodyがアタッチされていません！");

        if (Keyboard.current is Keyboard kb) _keyboard = kb;
        else Debug.LogError("TestPlayer:keyboardが見つかりません！");
    }

    void Update()
    {
        if(_rb != null && _keyboard != null)
        {
            if (_keyboard.wKey.isPressed) _rb.linearVelocity = new Vector3(0f, 0f, 10f);
            if (_keyboard.aKey.isPressed) _rb.linearVelocity = new Vector3(-10f, 0f, 0f);
            if (_keyboard.sKey.isPressed) _rb.linearVelocity = new Vector3(0f, 0f, -10f);
            if (_keyboard.dKey.isPressed) _rb.linearVelocity = new Vector3(10f, 0f, 0f);
        }
    }
}
