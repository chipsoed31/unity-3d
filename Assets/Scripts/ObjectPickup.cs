// Скрипт для подбора и переноса предметов в Unity, как в Half-Life 2 (ручной захват)

using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [Header("Binds")]
    public string takeKey = "Q";
    public string fastPutInventory = "Alt";
    public string pickupKey = "E";
    public string putKey = "G";
    //public string throwObjectKey = "Alt";

    [Header("Pickup Settings")]
    public float pickupRange = 5f;
    public float holdDistance = 2f;
    public float moveForce = 250f;
    public float throwForce = 500f;
    public LayerMask pickupLayer;

    [Header("References")]
    public Transform grabPoint;
    public Transform holdPoint;
    public Camera playerCamera;

    private Rigidbody heldObject;
    public bool isHolding => heldObject != null;

    public SpellManager spellManager;

    private int selectedWeapon = 0;

    private GameObject takenItem;

    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isHolding)
                TryPickup();
            else
                DropObject();
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (isHolding)
                ThrowObject();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (takenItem == null)
                TryTake();
            //else
            //    PutInInventory();
        }
        if (Input.GetKeyDown(KeyCode.G) && takenItem != null)
        {
            PutItem();
        }

        if (takenItem != null)
        {
            //takenItem.transform.SetParent(grabPoint);
            //takenItem.transform.localPosition = Vector3.zero;
            //takenItem.transform.localRotation = Quaternion.identity;
            
            // сразу ставим туда позицию
            takenItem.transform.position = grabPoint.position;
            // и подгоняем поворот, если нужно
            takenItem.transform.rotation = grabPoint.rotation;
        }
    }

    void FixedUpdate()
    {
        if (isHolding)
        {
            Vector3 directionToHold = (holdPoint.position - heldObject.position);
            heldObject.linearVelocity = directionToHold * moveForce * Time.fixedDeltaTime;
        }
    }

    void LateUpdate()
    {
        holdPoint.position = playerCamera.transform.position + playerCamera.transform.forward * holdDistance;
    }

    void TryPickup()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            Rigidbody rb = hit.collider.attachedRigidbody;
            if (rb != null)
            {
                WasWeaponSelected();
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 0.5f;
            }
        }
    }

    void DropObject()
    {
        if (heldObject)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 0.1f;
            heldObject = null;
            if (selectedWeapon > 0)
            {
                ReturnSelectedWeapon();
            }
        }
    }

    void ThrowObject()
    {
        if (heldObject)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 0.1f;
            heldObject.AddForce(playerCamera.transform.forward * throwForce);
            heldObject = null;
            if (selectedWeapon > 0)
            {
                ReturnSelectedWeapon();
            }
        }
        
    }

    void TryTake()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickupLayer))
        {
            if (hit.collider.CompareTag("Pickable"))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;

                if (rb != null)
                {
                    spellManager.rainSelected = false;
                    spellManager.fireballSelected = false;
                    rb.useGravity = false;
                    rb.isKinematic = true;
                    rb.detectCollisions = false;

                    takenItem = hit.collider.gameObject;

                    
                }
            }
        }
    }
    
    void PutItem()
    {
        if (takenItem != null)
        {
            Rigidbody rb = takenItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.detectCollisions = true;
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                takenItem.transform.SetParent(null);
                takenItem.transform.position = hit.point;

                takenItem = null;
            }
            else
            {
                takenItem.transform.SetParent(null);
                takenItem = null;
            } 
        }
    }

    void WasWeaponSelected()
    {
        if (spellManager.fireballSelected)
        {
            selectedWeapon = 1;
        }
        else if (spellManager.rainSelected)
        {
            selectedWeapon = 2;
        }
        else
        { selectedWeapon = 0; }
    }

    void ReturnSelectedWeapon()
    {
        switch (selectedWeapon)
        {
            case 1:
                spellManager.fireballSelected = true;
                break;
            case 2:
                spellManager.rainSelected = true;
                break;
        }
    }
}
