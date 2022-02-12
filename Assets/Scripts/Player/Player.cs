using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SingletonMonoBehaviour<Player>
{
    [SerializeField] private PlayerData data;

    #region Movement
    private float xInput;
    private float yInput;
    private Vector2 inputVector;
    private Vector2 moveDirection;
    private float movmentSpeed;
    private Direction playerDirection;
    private bool _playerInputDisabled = false;

    public bool PlayerInputDisabled { get => _playerInputDisabled; set => _playerInputDisabled = value; }
    #endregion

    #region Components
    private Rigidbody2D rb;
    private Animator animator;
    #endregion

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();    
    }

    private void Update()
    {
        GetPlayerInput();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void GetPlayerInput()
    {
        if (!_playerInputDisabled)
        {
            xInput = Input.GetAxisRaw("Horizontal");
            yInput = Input.GetAxisRaw("Vertical");
            inputVector = new Vector2(xInput, yInput);

            if (!Mathf.Approximately(inputVector.x, 0.0f) || !Mathf.Approximately(inputVector.y, 0.0f))
            {
                moveDirection.Set(inputVector.x, inputVector.y);
                moveDirection.Normalize();
            }

            if(xInput != 0 || yInput != 0)
            {
                movmentSpeed = data.movementSpeed;

                if (xInput < 0)
                {
                    playerDirection = Direction.left;
                }
                else if (xInput > 0)
                {
                    playerDirection = Direction.right;
                }
                else if (yInput < 0)
                {
                    playerDirection = Direction.down;
                }
                else if (yInput > 0)
                {
                    playerDirection = Direction.up;
                }
            }
        }
    }

    private void PlayerMovement()
    {
        Vector2 move = new Vector2(xInput * movmentSpeed * Time.deltaTime, yInput * movmentSpeed * Time.deltaTime);

        rb.MovePosition(rb.position + move);
    
        animator.SetFloat("xInput", moveDirection.x);
        animator.SetFloat("yInput", moveDirection.y);
        animator.SetFloat("speed", move.normalized.magnitude);

    }
}
