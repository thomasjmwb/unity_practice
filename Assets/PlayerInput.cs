using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public bool JumpPressed { get; private set; }
    public bool JumpHeld { get; private set; }
    public bool GlideHeld { get; private set; }
    public bool LeftPressed { get; private set; }
    public bool RightPressed { get; private set; }
    
    void Update()
    {
        ReadInput();
    }
    
    private void ReadInput()
    {
        HorizontalInput = 0f;
        if (Input.GetKey(KeyCode.A)) HorizontalInput = -1f;
        else if (Input.GetKey(KeyCode.D)) HorizontalInput = 1f;
        
        JumpPressed = Input.GetKeyDown(KeyCode.W);
        JumpHeld = Input.GetKey(KeyCode.W);
        GlideHeld = Input.GetKey(KeyCode.Space);
        
        LeftPressed = Input.GetKeyDown(KeyCode.A);
        RightPressed = Input.GetKeyDown(KeyCode.D);
    }
    
    public int GetInputSign()
    {
        if (Input.GetKey(KeyCode.A)) return -1;
        if (Input.GetKey(KeyCode.D)) return 1;
        return 0;
    }
}