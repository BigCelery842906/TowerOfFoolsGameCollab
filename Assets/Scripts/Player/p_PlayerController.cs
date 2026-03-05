using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class is listening for inputs, if u bind to an input any reason bind here but have ur code in a different class <3///
/// </summary>
public class p_PlayerController : MonoBehaviour
{        
    private IA_Player m_playerInputs;

    #region Player Scripts
    private p_PlayerMovement m_playerMovement;
    #endregion

    private void Awake()
    {
        m_playerMovement = GetComponent<p_PlayerMovement>();
    }

    private void OnEnable()
    {
        m_playerInputs = new IA_Player();

        m_playerInputs.Enable();

        m_playerInputs.AM_Player.Move.performed += Handle_Move;
        m_playerInputs.AM_Player.Move.canceled += Handle_MoveCancelled;

        m_playerInputs.AM_Player.Jump.performed += Handle_Jump;
    }

    private void OnDisable()
    {
        m_playerInputs.Disable();

        m_playerInputs.AM_Player.Move.performed -= Handle_Move;
        m_playerInputs.AM_Player.Move.canceled -= Handle_MoveCancelled;

        m_playerInputs.AM_Player.Jump.performed -= Handle_Jump;
    }

    /// <summary>
    /// Tells the player what direction to move
    /// </summary>
    private void Handle_Move(InputAction.CallbackContext ctx) => m_playerMovement.SetMoveDirection(ctx.ReadValue<Vector2>());

    /// <summary>
    /// Tells the player to stop moveing by passing in an empty vector 2
    /// </summary>
    private void Handle_MoveCancelled(InputAction.CallbackContext ctx) => m_playerMovement.SetMoveDirection(ctx.ReadValue<Vector2>());

    private void Handle_Jump(InputAction.CallbackContext ctx) => m_playerMovement.Jump();
}
