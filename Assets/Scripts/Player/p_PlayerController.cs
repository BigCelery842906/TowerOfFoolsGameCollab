using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class is listening for inputs, if u bind to an input any reason bind here but have ur code in a different class <3///
/// </summary>
public class p_PlayerController : MonoBehaviour
{
    #region Player Scripts
    private p_PlayerMovement m_playerMovement;
    private p_PlayerCombat m_playerCombat;
    private p_PlayerPickupManager m_playerPickupManager;
    private p_playerAnimControl m_playerAnim;
    #endregion

    [SerializeField] private PlayerInputManager m_playerInputManager;
    private PlayerInput m_playerInput;

    /// <summary>
    /// 0 = player 1 , 1 = player 2. Used for input  
    /// </summary>
    private int m_playerIndex;

    private void OnEnable()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerIndex = m_playerInput.playerIndex;

        m_playerInputManager = e_GameEvents.instance.gameObject.GetComponent<PlayerInputManager>();

        if (m_playerInputManager != null )
        {
            var gamepads = Gamepad.all;

            if (gamepads.Count > m_playerIndex)
            {
                // Use gamepads first
                m_playerInputManager.JoinPlayer(
                    m_playerIndex,
                    -1,
                    "Gamepad",
                    gamepads[m_playerIndex]
                );
            }

            else
            {
                // Keyboard fallback for 2 players
                string scheme = (p_PlayerData.ReturnPlayerIDFromTag(gameObject.tag) == 0) ? "WASD" : "Arrows";
                m_playerInput.SwitchCurrentControlScheme(scheme, Keyboard.current);

                m_playerInputManager.JoinPlayer(
                    m_playerIndex,
                    -1,
                    scheme,
                    Keyboard.current
                );
            }
        }

        m_playerMovement = GetComponent<p_PlayerMovement>();
        m_playerCombat = GetComponentInChildren<p_PlayerCombat>();
        m_playerPickupManager = GetComponent<p_PlayerPickupManager>();
        m_playerAnim = GetComponentInChildren<p_playerAnimControl>();
    }

    
    
    /// <summary>
    /// Tells the player what direction to move
    /// </summary>
    public void Handle_Move(InputAction.CallbackContext ctx) => m_playerMovement.SetMoveDirection(ctx.ReadValue<Vector2>());

    /// <summary>
    /// Tells the player to stop moveing by passing in an empty vector 2
    /// </summary>
    public void Handle_MoveCancelled(InputAction.CallbackContext ctx) => m_playerMovement.SetMoveDirection(ctx.ReadValue<Vector2>());

    public void Handle_Jump(InputAction.CallbackContext ctx) => m_playerMovement.Jump();
    public void Handle_JumpCancelled(InputAction.CallbackContext ctx) => m_playerMovement.JumpCancelled();

    public void Handle_Attack(InputAction.CallbackContext ctx)
    {
        if (m_playerPickupManager.GetPlayerInteractablePickupPPM())
        {
            //player has an interactable pickup use that rather than attack
            m_playerPickupManager.UseInteractablePickup();
        }
        else
        {
            m_playerCombat.Attack();
        }
    }

    public void Handle_Taunt(InputAction.CallbackContext ctx)
    {
        float tempF = Random.Range(0.0f, 1.0f);
        if (tempF >= 0.5)
        {
            tempF = 1;
        }
        else
        {
            tempF = 0;
        }

        m_playerAnim.SetTauntFloat(tempF);
    }

    public void Handle_Pause(InputAction.CallbackContext ctx)
    {
        e_GlobalData.instance.TogglePause();
    }
}
