using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask counterLayer;
    private Vector2 inputVector;
    private Vector3 moveDir;
    private Vector3 lastMoveDir;
    private bool isWalking;
    private float playerRadius = 0.7f;
    private float playerHeight = 2;
    private BaseCounter selectedCounter;

    [SerializeField] private Transform kitchenObjectHoldPoint;

    private KitchenObject kitchenObject;

    private void Awake()
    {
        if (Instance != null)
            Debug.LogError("More than 1 Player instance");
        Instance = this;
    }

    private void Start()
    {
        gameInput.OnInteractAction += GameInput_OnInteractAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        if (selectedCounter != null)
            selectedCounter.InteractAlternate(this);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (selectedCounter != null) 
            selectedCounter.Interact(this);
    }

    private void Update()
    {
        HandleInput();
        HandleMovements();
        HandleInteractions();
    }

    private void HandleInteractions()
    {
        if (moveDir != Vector3.zero)
            lastMoveDir = moveDir;

        float interactDistance = 1;
        if (Physics.Raycast(transform.position, lastMoveDir, out RaycastHit hitInfo, interactDistance, counterLayer))
        {
            if (hitInfo.transform.TryGetComponent(out BaseCounter counter))
            {
                if (counter != selectedCounter)
                    SetSelectedCounter(counter);
            }
            else
                SetSelectedCounter(null);
        }
        else
            SetSelectedCounter(null);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs
        {
            selectedCounter = selectedCounter
        });
    }

    private void HandleInput()
    {
        inputVector = gameInput.GetMovementVectorNormalized();
        moveDir = new Vector3(inputVector.x, 0, inputVector.y);
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    private void HandleMovements()
    {
        isWalking = inputVector != Vector2.zero;

        float moveDistance = moveSpeed * Time.deltaTime;

        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        Vector3 updatedMoveDir = moveDir;

        if (!canMove && moveDir.x != 0 && moveDir.z != 0)
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
    }
}
