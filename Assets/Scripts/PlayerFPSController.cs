using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerFPSController : MonoBehaviour
{
    public CharacterController controller;
    public Transform cameraTransform;
    public Transform bodyTransform;

    public float moveSpeed = 4f;
    public float gravity = -9.81f;

    public float mouseSensitivity = 150f;
    public float minPitch = -80f;
    public float maxPitch = 80f;

    private Controles input;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float verticalVel;
    private float pitch;
    private bool cursorLocked;
    private bool enabledControls = true;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        input = new Controles();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        input.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        input.Player.Look.canceled += ctx => lookInput = Vector2.zero;
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        if (!enabledControls) return;

        ManejarCursor();
        Mover();
        Rotar();
    }

    public void SetControlsEnabled(bool state)
    {
        enabledControls = state;

        if (!state)
        {
            moveInput = Vector2.zero;
            lookInput = Vector2.zero;
            SetCursorLock(false);
        }
    }

    public void ForceUnlockCursor()
    {
        SetCursorLock(false);
    }

    void ManejarCursor()
    {
        var mouse = Mouse.current;
        var key = Keyboard.current;

        if (mouse != null && mouse.leftButton.wasPressedThisFrame)
            SetCursorLock(!cursorLocked);

        if (key != null && key.escapeKey.wasPressedThisFrame)
            SetCursorLock(false);
    }

    void Mover()
    {
        Vector3 move =
            bodyTransform.forward * moveInput.y +
            bodyTransform.right * moveInput.x;

        if (move.magnitude > 1f) move.Normalize();

        if (controller.isGrounded && verticalVel < 0)
            verticalVel = -2f;

        verticalVel += gravity * Time.deltaTime;

        Vector3 vel = move * moveSpeed;
        vel.y = verticalVel;

        controller.Move(vel * Time.deltaTime);
    }

    void Rotar()
    {
        if (!cursorLocked) return;

        float mx = lookInput.x * mouseSensitivity * Time.deltaTime;
        float my = lookInput.y * mouseSensitivity * Time.deltaTime;

        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.localRotation = Quaternion.Euler(pitch, 0, 0);
        bodyTransform.Rotate(Vector3.up * mx);
    }

    void SetCursorLock(bool locked)
    {
        cursorLocked = locked;

        Cursor.lockState = locked ?
            CursorLockMode.Locked :
            CursorLockMode.None;

        Cursor.visible = !locked;
    }
}
