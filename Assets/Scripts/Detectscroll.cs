using UnityEngine;
using UnityEngine.InputSystem;

public class DetectScroll : MonoBehaviour
{
    void Update()
    {
        if (Mouse.current != null)
        {
            var s = Mouse.current.scroll.ReadValue();
            if (s.y != 0)
                Debug.Log("Input System scroll: " + s.y);
        }

        if (Input.mouseScrollDelta.y != 0)
        {
            Debug.Log("Old Input scroll: " + Input.mouseScrollDelta.y);
        }
    }
}
