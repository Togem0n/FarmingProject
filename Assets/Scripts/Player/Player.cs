using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class Player : SingletonMonoBehaviour<Player>, ISaveable
{

    #region Variables
    [SerializeField] public PlayerData playerData;
    [SerializeField] public GridCursor gridCursor;
    [SerializeField] public UIInventoryBar uiInventoryBar;

    // StateMachine
    public PlayerStateMachine Statemachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerHoeingState HoeingState { get; private set; }
    public PlayerWateringState WateringState { get; private set; }
    public PlayerCarrySeedState CarryingSeedState { get; private set; }
    public PlayerHarvestingState HarvestingState { get; private set; }
    public PlayerChoppingState ChoppingState { get; private set; }
    public PlayerBreakingState BreakingState { get; private set; }

    // Movement
    public int xInput;
    public int yInput;
    public Vector2 inputVector;
    public Vector2 moveDirection;
    public float movementSpeed;
    public Direction playerDirection;
    public bool _playerInputDisabled = false;

    public bool PlayerInputDisabled { get => _playerInputDisabled; set => _playerInputDisabled = value; }

    // Components
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [SerializeField] public Animator animator_hair;
    [SerializeField] public Animator animator_cloth;
    [HideInInspector] public Camera mainCamera;

    // Use Tool Variables
    public Vector3 useToolDirection;
    public float useToolDirectionForAnimator;
    public Vector3Int useToolGridDirection;
    public Vector3Int useToolGridPosition;

    // Dialogue system
    [SerializeField] private DialogueUI dialogueUI;
    public DialogueUI DialogueUI => dialogueUI;
    public IInteractable Interactable { get; set; }

    // Others
    public Vector2 CurrentVelocity { get; private set; }
    public int FacingDirection { get; private set; }

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get => _iSaveableUniqueID; set => _iSaveableUniqueID = value; }

    private GameObjectSave _gameObjectSave;
    public GameObjectSave GameObjectSave { get => _gameObjectSave; set => _gameObjectSave = value; }
    
    public Vector2 workspace;

    #endregion

    #region Life Cycle

    private void OnEnable()
    {
        ISaveableRegister();
    }

    private void OnDisable()
    {
        ISaveableDeregister();
    }

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        Statemachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, Statemachine, playerData, "idle");
        WalkState = new PlayerWalkState(this, Statemachine, playerData, "walk");
        HoeingState = new PlayerHoeingState(this, Statemachine, playerData, "hoeing");
        WateringState = new PlayerWateringState(this, Statemachine, playerData, "watering");
        CarryingSeedState = new PlayerCarrySeedState(this, Statemachine, playerData, "planting");
        HarvestingState = new PlayerHarvestingState(this, Statemachine, playerData, "harvesting");
        ChoppingState = new PlayerChoppingState(this, Statemachine, playerData, "chopping");
        BreakingState = new PlayerBreakingState(this, Statemachine, playerData, "breaking");

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;
        GameObjectSave = new GameObjectSave();
    }

    private void Start()
    {
        EventHandler.AfterSceneLoadFadeInEvent += EnablePlayerInput;

        FacingDirection = 1;
        Statemachine.Initialize(IdleState);
    }

    private void Update()
    {

        if (!PlayerInputDisabled)
        {
            GetPlayerInput();

            PlayerTestInput();

            DialogueInput();
        }

        Statemachine.CurrentState.LogicUpdate();
    }

    private void FixedUpdate()
    {
        Statemachine.CurrentState.PhysicsUpdate();
    }

    #endregion

    private void GetPlayerInput()
    {
        if (!_playerInputDisabled)
        {
            xInput = (int)Input.GetAxisRaw("Horizontal");
            yInput = (int)Input.GetAxisRaw("Vertical");
            inputVector = new Vector2(xInput, yInput);

            if (!Mathf.Approximately(inputVector.x, 0.0f) || !Mathf.Approximately(inputVector.y, 0.0f))
            {
                moveDirection.Set(inputVector.x, inputVector.y);
                moveDirection.Normalize();
            }

            if (xInput != 0 || yInput != 0)
            {
                movementSpeed = playerData.movementSpeed;

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

    private void DialogueInput()
    {
        if(Interactable != null)
        {
            if(!Interactable.needClick() || (Interactable.needClick() && Input.GetKeyDown(KeyCode.E))){
                Interactable.Interact(this);
                Debug.Log("interact");
            }
        } 
    }

    public Vector3 GetPlyerViewportPosition()
    {
        return mainCamera.WorldToViewportPoint(transform.position);
    }

    public void EnablePlayerInput()
    {
        PlayerInputDisabled = false;
    }

    public void DisablePlayerInput()
    {
        ResetMovement();
        PlayerInputDisabled = true;
    }

    public void DisablePlayerInputAndResetMovement()
    {
        DisablePlayerInput();

        ResetMovement();
    }

    private void ResetMovement()
    {
        xInput = 0;
        yInput = 0;
        animator.SetFloat("xInput", moveDirection.x);
        animator.SetFloat("yInput", moveDirection.y);
        animator.SetFloat("speed", 0);
    }

    private void PlayerTestInput()
    {
        if (Input.GetKey(KeyCode.T))
        {
            TimeManager.Instance.TestAdvanceGameMinute();
        }

        if (Input.GetKeyUp(KeyCode.G))
        {
            TimeManager.Instance.TestAdvanceGameDay();
        }

        if (Input.GetKey(KeyCode.L))
        {
            TimeManager.Instance.GoToNextDay();
        }
    }

    public void SetUseToolDirection(float mousePosX, float mousePosY)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosX, mousePosY, -mainCamera.transform.position.z));
        Vector3 tmp = mouseWorldPosition - transform.position;

        Vector3Int GridOfMouse = GridDetailsManager.Instance.grid.WorldToCell(mouseWorldPosition);
        Vector3Int GridOfPlayer = GridDetailsManager.Instance.grid.WorldToCell(transform.position);

        int itemUseGridRadius = InventoryManager.Instance.GetSelectedItemDetails().itemUseGridRadius;

        if (Mathf.Abs(GridOfMouse.x - GridOfPlayer.x) <= itemUseGridRadius && Mathf.Abs(GridOfMouse.y - GridOfPlayer.y) <= itemUseGridRadius)
        {

            useToolGridPosition = GridOfMouse;

            useToolGridDirection = GridOfMouse - GridOfPlayer;

            if (GridOfMouse == GridOfPlayer)
            {
                useToolGridDirection.x = (int)moveDirection.x;
                useToolGridDirection.y = (int)moveDirection.y;

                useToolGridPosition = GridOfPlayer + useToolGridDirection;
            }


            if (useToolGridDirection.y != 0)
            {
                useToolGridDirection.x = 0;
            }
            moveDirection.x = useToolGridDirection.x;
            moveDirection.y = useToolGridDirection.y;

        }
        else
        {
            useToolGridDirection.x = (int)moveDirection.x;
            useToolGridDirection.y = (int)moveDirection.y;

            useToolGridPosition = GridOfPlayer + useToolGridDirection;
        }

    }

    public void SetPlantDirection(float mousePosX, float mousePosY)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosX, mousePosY, -mainCamera.transform.position.z));
        Vector3 tmp = mouseWorldPosition - transform.position;

        Vector3Int GridOfMouse = GridDetailsManager.Instance.grid.WorldToCell(mouseWorldPosition);
        Vector3Int GridOfPlayer = GridDetailsManager.Instance.grid.WorldToCell(transform.position);

        int itemUseGridRadius = InventoryManager.Instance.GetSelectedItemDetails().itemUseGridRadius;

        if (Mathf.Abs(GridOfMouse.x - GridOfPlayer.x) <= itemUseGridRadius && Mathf.Abs(GridOfMouse.y - GridOfPlayer.y) <= itemUseGridRadius)
        {

            useToolGridPosition = GridOfMouse;

            useToolGridDirection = GridOfMouse - GridOfPlayer;

            if (GridOfMouse == GridOfPlayer)
            {
                useToolGridDirection.x = (int)moveDirection.x;
                useToolGridDirection.y = (int)moveDirection.y;

                useToolGridPosition = GridOfPlayer;
                return;
            }


            if (useToolGridDirection.y != 0)
            {
                useToolGridDirection.x = 0;
            }
            moveDirection.x = useToolGridDirection.x;
            moveDirection.y = useToolGridDirection.y;

        }
        else
        {
            useToolGridDirection.x = (int)moveDirection.x;
            useToolGridDirection.y = (int)moveDirection.y;

            useToolGridPosition = GridOfPlayer + useToolGridDirection;
        }
    }

    private void AnimationTrigger() => Statemachine.CurrentState.AnimationTrigger();

    private void AniamtionFinishTrigger() => Statemachine.CurrentState.AnimationFinishTrigger();

    #region ISaveable

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        GameObjectSave.sceneData.Remove(Settings.PersistentScene);

        SceneSave sceneSave = new SceneSave();

        sceneSave.vector3Dictionary = new Dictionary<string, Vector3Serializable>();

        sceneSave.stringDictionary = new Dictionary<string, string>();

        Vector3Serializable vector3Serializable = 
            new Vector3Serializable(transform.position.x, transform.position.y, transform.position.z);

        sceneSave.vector3Dictionary.Add("playerPosition", vector3Serializable);

        sceneSave.stringDictionary.Add("currentScene", SceneManager.GetActiveScene().name);

        sceneSave.stringDictionary.Add("playerDirection", playerDirection.ToString());

        GameObjectSave.sceneData.Add(Settings.PersistentScene, sceneSave);

        return GameObjectSave;

    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            if(gameObjectSave.sceneData.TryGetValue(Settings.PersistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.vector3Dictionary != null &&
                    sceneSave.vector3Dictionary.TryGetValue("playerPosition", out Vector3Serializable playerPosition))
                {
                    transform.position = new Vector3(playerPosition.x, playerPosition.y, playerPosition.z);
                }

                if(sceneSave.stringDictionary != null)
                {
                    if(sceneSave.stringDictionary.TryGetValue("currentScene", out string currentScene))
                    {
                        SceneControllerManager.Instance.FadeAndLoadScene(currentScene, transform.position);
                    }

                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        
    }

    #endregion
}
