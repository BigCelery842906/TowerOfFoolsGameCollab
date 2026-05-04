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

        if(m_playerInputManager != null )
        {
            var gamepads = Gamepad.all;

            Debug.Log("gamepads: " + gamepads.Count + "index: " + m_playerIndex);

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
                string scheme = (m_playerIndex == 1) ? "WASD" : "Arrows";
                m_playerInput.SwitchCurrentControlScheme(scheme, Keyboard.current);

                Debug.Log($"Player {m_playerIndex} scheme: {m_playerInput.currentControlScheme}");

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

    //private void OnEnable()
    //{
    //    e_GameEvents.instance.onPlayerDeathAdded += Handle_PlayerReset;

    //    m_playerMovement = GetComponent<p_PlayerMovement>();
    //    m_playerCombat = GetComponentInChildren<p_PlayerCombat>();
    //    m_playerPickupManager = GetComponent<p_PlayerPickupManager>();
    //    m_playerAnim = GetComponentInChildren<p_playerAnimControl>();

    //    m_playerInputs = new IA_Player();

    //    m_playerInputs.Enable();

    //    //I did try a couple ways to give the players different action maps but this keeps the inspector clean and its not performance heavy, its just kinda ugly xx
    //    if (gameObject.CompareTag("Player0"))
    //    {
    //        m_playerID = 0;

    //        m_playerInputs.AM_PlayerOne.Move.performed += Handle_Move;
    //        m_playerInputs.AM_PlayerOne.Move.canceled += Handle_MoveCancelled;

    //        m_playerInputs.AM_PlayerOne.Jump.performed += Handle_Jump;
    //        m_playerInputs.AM_PlayerOne.Jump.canceled += Handle_JumpCancelled;

    //        m_playerInputs.AM_PlayerOne.Attack.performed += Handle_Attack;

    //        m_playerInputs.AM_PlayerOne.Taunt.performed += Handle_Taunt;

    //        m_playerInputs.AM_PlayerOne.Pause.performed += Handle_Pause;
    //    }
    //    else
    //    {
    //        m_playerID = 1;

    //        m_playerInputs.AM_PlayerTwo.Move.performed += Handle_Move;
    //        m_playerInputs.AM_PlayerTwo.Move.canceled += Handle_MoveCancelled;

    //        m_playerInputs.AM_PlayerTwo.Jump.performed += Handle_Jump;
    //        m_playerInputs.AM_PlayerTwo.Jump.canceled += Handle_JumpCancelled;

    //        m_playerInputs.AM_PlayerTwo.Attack.performed += Handle_Attack;

    //        m_playerInputs.AM_PlayerTwo.Taunt.performed += Handle_Taunt;

    //        m_playerInputs.AM_PlayerTwo.Pause.performed += Handle_Pause;
    //    }
    //}

    //private void OnDisable()
    //{
    //    m_playerInputs.Disable();

    //    if (m_playerID == 0)
    //    {
    //        m_playerInputs.AM_PlayerOne.Move.performed -= Handle_Move;
    //        m_playerInputs.AM_PlayerOne.Move.canceled -= Handle_MoveCancelled;

    //        m_playerInputs.AM_PlayerOne.Jump.performed -= Handle_Jump;
    //        m_playerInputs.AM_PlayerOne.Jump.canceled -= Handle_JumpCancelled;

    //        m_playerInputs.AM_PlayerOne.Attack.performed -= Handle_Attack;

    //        m_playerInputs.AM_PlayerOne.Taunt.performed -= Handle_Taunt;

    //        m_playerInputs.AM_PlayerOne.Pause.performed -= Handle_Pause;
    //    }
    //    else
    //    {
    //        m_playerInputs.AM_PlayerTwo.Move.performed -= Handle_Move;
    //        m_playerInputs.AM_PlayerTwo.Move.canceled -= Handle_MoveCancelled;

    //        m_playerInputs.AM_PlayerTwo.Jump.performed -= Handle_Jump;
    //        m_playerInputs.AM_PlayerTwo.Jump.canceled -= Handle_JumpCancelled;

    //        m_playerInputs.AM_PlayerTwo.Attack.performed -= Handle_Attack;

    //        m_playerInputs.AM_PlayerTwo.Taunt.performed -= Handle_Taunt;

    //        m_playerInputs.AM_PlayerTwo.Pause.performed -= Handle_Pause;
    //    }
    //}

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
