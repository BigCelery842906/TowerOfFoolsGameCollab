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
    #endregion

    private IA_Player m_playerInputs;

    /// <summary>
    /// 0 = player 1 , 1 = player 2. Used for input maps and pickups
    /// </summary>
    private int m_playerID;

    private void Awake()
    {
        m_playerMovement = GetComponent<p_PlayerMovement>();
        m_playerCombat = GetComponentInChildren<p_PlayerCombat>();
        m_playerPickupManager = GetComponent<p_PlayerPickupManager>();
    }

    private void OnEnable()
    {
        m_playerInputs = new IA_Player();

        m_playerInputs.Enable();

        //I did try a couple ways to give the players different action maps but this keeps the inspector clean and its not performance heavy, its just kinda ugly xx
        if (gameObject.CompareTag("Player0"))
        {
            m_playerID = 0;

            m_playerInputs.AM_PlayerOne.Move.performed += Handle_Move;
            m_playerInputs.AM_PlayerOne.Move.canceled += Handle_MoveCancelled;
                                    
            m_playerInputs.AM_PlayerOne.Jump.performed += Handle_Jump;
            m_playerInputs.AM_PlayerOne.Jump.canceled += Handle_JumpCancelled;

            m_playerInputs.AM_PlayerOne.Attack.performed += Handle_Attack;
        }
        else
        {
            m_playerID = 1;

            m_playerInputs.AM_PlayerTwo.Move.performed += Handle_Move;
            m_playerInputs.AM_PlayerTwo.Move.canceled += Handle_MoveCancelled;

            m_playerInputs.AM_PlayerTwo.Jump.performed += Handle_Jump;
            m_playerInputs.AM_PlayerTwo.Jump.canceled += Handle_JumpCancelled;

            m_playerInputs.AM_PlayerTwo.Attack.performed += Handle_Attack;
        }

        // bind the other non-player specific actions
        m_playerInputs.Menus.Pause.performed += Handle_Pause;
    }

    private void OnDisable()
    {
        m_playerInputs.Disable();

        if (m_playerID == 0)
        {
            m_playerInputs.AM_PlayerOne.Move.performed -= Handle_Move;
            m_playerInputs.AM_PlayerOne.Move.canceled -= Handle_MoveCancelled;

            m_playerInputs.AM_PlayerOne.Jump.performed -= Handle_Jump;
            m_playerInputs.AM_PlayerOne.Jump.canceled -= Handle_JumpCancelled;
        }
        else
        {
            m_playerInputs.AM_PlayerTwo.Move.performed -= Handle_Move;
            m_playerInputs.AM_PlayerTwo.Move.canceled -= Handle_MoveCancelled;

            m_playerInputs.AM_PlayerTwo.Jump.performed -= Handle_Jump;
            m_playerInputs.AM_PlayerTwo.Jump.canceled -= Handle_JumpCancelled;
        }
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
    private void Handle_JumpCancelled(InputAction.CallbackContext ctx) => m_playerMovement.JumpCancelled();

    private void Handle_Attack(InputAction.CallbackContext ctx)
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

    private void Handle_Pause(InputAction.CallbackContext ctx)
    {
        // tells game events pause was fired
        
        // game events handles pause, inverts it, pauses game (timescale),
        // game events sends off a message that pause menu listens for to toggle pause state
    }
}
