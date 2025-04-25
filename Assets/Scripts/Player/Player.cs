using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Player : NetworkBehaviour, IKitchenObjectParent
{
    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickupSomething;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
        OnAnyPlayerPickupSomething = null;
    }

    public static Player LocalInstance { get; private set; }
    // -> many players will be spawned. but only the owner of this player (machine that running the game) is LocalPlayer
    // see OnNetworkSpawn() function
    // this LocalPlayer will be used to set visual for selected counter

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }
    public event EventHandler OnPickupSomething;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private LayerMask counterLayer;
    private Vector2 inputVector => GameInput.Instance.GetMovementVectorNormalized();
    private Vector3 moveDir => new Vector3(inputVector.x, 0, inputVector.y);
    private Vector3 lastMoveDir;
    private bool isWalking;
    private float playerRadius = 0.7f;
    private float playerHeight = 2;
    private BaseCounter selectedCounter;

    [SerializeField] private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        //Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Start()
    {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if(!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null)
            selectedCounter.InteractAlternate(this);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (!GameManager.Instance.IsGamePlaying()) return;

        if (selectedCounter != null) 
            selectedCounter.Interact(this);
    }

    private void Update()
    {
        if(!IsOwner) return;
        //HandleMovementsServerRpc(inputVector, moveDir); // This is for server authoritative principle. Clients send input to server then server processes input then make clients move
        // Pass arguments to rpc is required, test by removing. 
        // Use NetworkTransform for server auth, ClientNetworkTransform for client auth
        HandleMovements(); // Comment this when use server authoritative
        HandleSelectedCounterVisual();
    }

    private void HandleSelectedCounterVisual()
    {
        if (moveDir != Vector3.zero)
            lastMoveDir = moveDir;

        float interactDistance = 1;
        if (Physics.Raycast(transform.position, lastMoveDir, out RaycastHit hitInfo, interactDistance, counterLayer))
        {
            if (hitInfo.transform.TryGetComponent(out BaseCounter counter))
            {
                if (counter != selectedCounter) SetSelectedCounter(counter);
            }
            else SetSelectedCounter(null);
        }
        else SetSelectedCounter(null);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    [ServerRpc]
    private void HandleMovementsServerRpc(Vector2 inputVector, Vector3 moveDir)
    {
        isWalking = inputVector != Vector2.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        Vector3 updatedMoveDir = moveDir;

        if (!canMove && (Mathf.Abs(moveDir.x) > 0.2f) && (Mathf.Abs(moveDir.z) > 0.2f))
        {
            if (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, new Vector3(moveDir.x, 0, 0), moveDistance))
            {
                canMove = true;
                updatedMoveDir.z = 0;
            }
            else if (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, new Vector3(0, 0, moveDir.z), moveDistance))
            {
                canMove = true;
                updatedMoveDir.x = 0;
            }
        }

        if (canMove)
            transform.position += updatedMoveDir * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private void HandleMovements()
    {
        isWalking = inputVector != Vector2.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        Vector3 updatedMoveDir = moveDir;

        if (!canMove && (Mathf.Abs(moveDir.x) > 0.2f) && (Mathf.Abs(moveDir.z) > 0.2f))
        {
            if (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, new Vector3(moveDir.x, 0, 0), moveDistance))
            {
                canMove = true;
                updatedMoveDir.z = 0;
            }
            else if (!Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, new Vector3(0, 0, moveDir.z), moveDistance))
            {
                canMove = true;
                updatedMoveDir.x = 0;
            }
        }

        if (canMove)
            transform.position += updatedMoveDir * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public Transform GetKitchenObjectFollowTransform()
    {
        return kitchenObjectHoldPoint;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickupSomething?.Invoke(this, EventArgs.Empty);
            OnAnyPlayerPickupSomething?.Invoke(this, EventArgs.Empty);
        }
    }

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
