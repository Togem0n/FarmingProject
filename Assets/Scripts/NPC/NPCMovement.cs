using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class NPCMovement : MonoBehaviour
{
    public SceneName npcCurrentScene;
    [HideInInspector] public SceneName npcTargetScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination;

    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f;

    [SerializeField] private float npcMinSpeed = 1f;
    [SerializeField] private float npcMaxSpeed = 3f;
    private bool npcIsMoving = false;

    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("Npc Animation")]
    [SerializeField] private AnimationClip blankAnimation = null;

    private Grid grid;
    private Rigidbody2D rigidbody2D;
    private BoxCollider2D boxCollider2D;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Animator animator;
    private AnimatorOverrideController animatorOverrideController;
    private int lastMoveAnimationParameter;
    private NPCPath npcPath;
    private bool npcInitialised = false;
    private SpriteRenderer spriteRenderer;
    [HideInInspector] public bool npcActiveInScene = false;

    private bool sceneLoaded = false;

    private Coroutine moveToGridPositionRoutine;

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoaded;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }

    private void AfterSceneLoaded()
    {
        grid = GameObject.FindObjectOfType<Grid>();

        if (!npcInitialised)
        {
            InitialiseNPC();
            npcInitialised = true;
        }

        sceneLoaded = true;
    }
    
    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        SetIdleAnimation();
    }

    private void FixedUpdate()
    {
        if (sceneLoaded)
        {
            if (!npcIsMoving)
            {
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;

                if(npcPath.npcMovementStepStack.Count > 0)
                {
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();

                    npcCurrentScene = npcMovementStep.sceneName;

                    if(npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        Debug.Log("go to new scene");
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        SetNPCActiveInScene();

                        npcMovementStep = npcPath.npcMovementStepStack.Pop();

                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }
                    else
                    {
                        SetNPCInactiveInScene();

                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);

                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        if (npcMovementStepTime < gameTime)
                        {
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();

                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }
                }
                else
                {
                    ResetMoveAnimation();

                    SetNPCFacingDirection();

                    SetNPCEventAnimation();
                }
            }
        }
    }

    private void SetNPCEventAnimation()
    {
        if(npcTargetAnimationClip != null)
        {
            ResetIdleAnimation();
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }

    public void ClearNPCEventAnimation()
    {
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        transform.rotation = Quaternion.identity;
    }

    private void SetNPCFacingDirection()
    {
        ResetIdleAnimation();

        switch (npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool("idle_up", true);
                break;

            case Direction.down:
                animator.SetBool("idle_down", true);
                break;

            case Direction.left:
                animator.SetBool("idle_left", true);
                break;

            case Direction.right:
                animator.SetBool("idle_right", true);
                break;

            default:
                break;
        }
    }

    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }

    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        npcIsMoving = true;

        SetMoveAnimation(gridPosition);

        npcNextWorldPosition = GetWorldPosition(gridPosition);

        if (npcMovementStepTime > gameTime)
        {
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            float npcCalculatedSpeed = Mathf.Max
                (npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            if(npcCalculatedSpeed <= npcMaxSpeed)
            {
                while(Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime,
                                                unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    rigidbody2D.MovePosition(rigidbody2D.position + move);

                    yield return waitForFixedUpdate;
                }
            }
        }

        rigidbody2D.position = npcNextWorldPosition;
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }

    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        ResetIdleAnimation();

        ResetMoveAnimation();

        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        Vector3 directionVector = toWorldPosition - transform.position;

        if(Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            if(directionVector.x > 0)
            {
                Debug.Log("walk right");
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                Debug.Log("walk left");

                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            if (directionVector.y > 0)
            {
                Debug.Log("walk up");

                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                Debug.Log("walk down");

                animator.SetBool(Settings.walkDown, true);
            }
        }
    }

    private void ResetMoveAnimation()
    {
        animator.SetBool("walk_down", false);
        animator.SetBool("walk_up", false);
        animator.SetBool("walk_left", false);
        animator.SetBool("walk_right", false);
    }

    private void SetIdleAnimation()
    {
        animator.SetBool("idle_down", true);
    }

    private void ResetIdleAnimation()
    {
        animator.SetBool("idle_down", false);
        animator.SetBool("idle_up", false);
        animator.SetBool("idle_left", false);
        animator.SetBool("idle_right", false);
    }

    public void SetNPCActiveInScene()
    {
        spriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }

    public void SetNPCInactiveInScene()
    {
        spriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }

    public Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }

    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }

    public void CancelNPCMovement()
    {
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3Int.zero;
        npcIsMoving = false;    

        if(moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        ResetMoveAnimation();

        ClearNPCEventAnimation();

        ResetIdleAnimation();

        SetIdleAnimation();
    }

    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;
        ClearNPCEventAnimation();
    }

    private void InitialiseNPC()
    {
        if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            SetNPCActiveInScene();
        }
        else
        {
            SetNPCInactiveInScene();
        }
        Debug.Log("npc current scene:" + npcCurrentScene.ToString());
        Debug.Log("get active scene:" + SceneManager.GetActiveScene().name);

        npcPreviousMovementStepScene = npcCurrentScene;

        npcCurrentGridPosition = GetGridPosition(transform.position);

        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }
}
