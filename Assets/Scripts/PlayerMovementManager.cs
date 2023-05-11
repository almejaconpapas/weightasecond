using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerMovementManager : MonoBehaviour
{
    #region Movement
    [Header("Movement")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintSpeedMultiplier = 3;
    private float sprintSpeedMultiplierReset;
    private bool isSprinting;
    [SerializeField] private float jumpHeight = 2f;
    private InputPlayer inputPlayer;
    private float movePlayerX;
    private float movePlayerY;
    bool jumped;
    bool isInAir;
    bool isFalling;
    bool pickPressed;
    private Rigidbody rBody;
    #endregion Movement
    
    #region Pickable
    [Header("Pickable")]
    [SerializeField] private GameObject pickableObject;
    private bool canPickObject;

    public float gravity = -10;

    [SerializeField] private float increaseHeightVelocity = 2f;
    [SerializeField] private float maxPickableHeight = 3f;
    [SerializeField] private float minPickableHeight = 1f;

    private float increase;
    private float decrease;

    #endregion Pickable

    #region Animation
    Vector3 m_EulerAngleVelocity;
    [Header("Animation")]
    [SerializeField]
    float angleTurnVelocity = 100;
    [SerializeField]
    Animator animator;
    int moveSpeed;
    float velocity;
    [SerializeField]
    float acceleration = 5f;
    [SerializeField]
    float deceleration = 2f;
    Quaternion rotacion;

    #endregion Animation

    #region Camera
    [Header("Camera Controls")]
    [SerializeField] CinemachineFreeLook c_vcam;
    [SerializeField] Transform cameraRotation;

    // ZOOM

    //[SerializeField] private InputActionAsset inputProvider;
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float zoomAcceleration = 2.5f;
    [SerializeField] private float zoomInnerRange = 3f;
    [SerializeField] private float zoomOuterRange = 50f;

    private float currentMiddleRigRadius = 3.9f;
    private float newMiddleRigRadius = 3.9f;

    [SerializeField] private float zoomXYAxis = 0f;
    public float ZoomXYAxis
    {
        get { return zoomXYAxis; }
        set
        {
            if(zoomXYAxis == value) return;
            zoomXYAxis = value;
            AdjustCameraZooomIndex(ZoomXYAxis);
        }
    }
    #endregion Camera
    
    void Awake(){
        // inputPlayer = GetComponent<InputPlayer>();
        rBody = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0f, gravity, 0f);
        //inputPlayer = GetComponent<InputPlayer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_EulerAngleVelocity = new Vector3(0,angleTurnVelocity,0);
        animator = GetComponent<Animator>();
        moveSpeed = Animator.StringToHash("Move Speed");
        isSprinting = false;
        sprintSpeedMultiplierReset = sprintSpeedMultiplier;
        isInAir = false;
        isFalling = false;
        c_vcam.m_CommonLens = true;
        increase = 0f;
        decrease = 0f;
    }

    #region UPDATE
    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Is sprinting: " + isSprinting);
    }
    void LateUpdate(){
        UpdateZoomLevel();
    }
    void FixedUpdate(){
        float horMouse = Input.GetAxis("Horizontal") * speed;
        float verMouse = Input.GetAxis("Vertical") * speed;

        Vector3 camForward = cameraRotation.forward;
        Vector3 camRight = cameraRotation.right;

        camForward.y = 0;
        camRight.y = 0;

        Vector3 forwardRelative = verMouse * camForward;
        Vector3 rightRelative = horMouse * camRight;
        Vector3 moveDirForward = forwardRelative + rightRelative;
        Vector3 moveDirLateral = verMouse * camRight + horMouse * camForward;
        // Vector3 vel;
        if(jumped && !isInAir){
            // vel = new Vector3(movePlayerX, jumpHeight, movePlayerY) * speed;
            rBody.AddForce(0f, jumpHeight * 100, 0f);

            rBody.velocity = new Vector3(moveDirForward.x  * sprintSpeedMultiplier, rBody.velocity.y, moveDirForward.z * sprintSpeedMultiplier);
            jumped = false;
        }
        else{
            rBody.velocity = new Vector3(moveDirForward.x, rBody.velocity.y, moveDirForward.z);
            // rBody.velocity = new Vector3(movePlayerX, 0,movePlayerY) * speed;
            // Debug.Log(cameraRotation.rotation.y);
            //transform.eulerAngles = new Vector3(0f, cameraRotation.eulerAngles.y, 0f);
            if(movePlayerX > 0 || movePlayerX < 0 || movePlayerY > 0 || movePlayerY < 0){
                // float angles = Mathf.Cos(movePlayerX + movePlayerY);
                // Debug.Log(angles);
                // rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y + movePlayerY * 180 + movePlayerX * 90, 0f);
                if(movePlayerY > 0){
                    rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y, 0f);
                }
                else if(movePlayerY < 0){
                    rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y -180, 0f);
                }
                if(movePlayerX > 0){
                    rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y +90, 0f);
                }
                else if(movePlayerX < 45){
                    rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y -45, 0f);
                }
                else if(movePlayerX < 0){
                    rotacion = Quaternion.Euler(0f, cameraRotation.eulerAngles.y -90, 0f);
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, rotacion, Time.deltaTime * 5f);
            }

            //rBody.rotation = new Quaternion(0f, cameraRotation.rotation.y, 0f, 0f);
            // m_EulerAngleVelocity = new Vector3(0, angleTurnVelocity * )
            // Quaternion deltaRotation = Quaternion.Euler(m_EulerAngleVelocity * Time.fixedDeltaTime);
            // rBody.MoveRotation(rBody.rotation * deltaRotation);
        }

        // Debug.Log($"{movePlayerX}, {movePlayerY}");

        adjustHeightPickable(increase, decrease);

        velocity = Mathf.Abs(velocity);
        //Debug.Log(velocity);
        // WALK
        if(!jumped && !isInAir){
            if(!isSprinting){
                if(Mathf.Abs(movePlayerX + movePlayerY) > 0 && velocity < 1.0f){
                    velocity += Time.deltaTime  * acceleration;
                }
                else if(Mathf.Abs(movePlayerX + movePlayerY) <= 0 && velocity > 0.0f){
                    velocity -= Time.deltaTime * deceleration;
                    c_vcam.m_Lens.FieldOfView = Mathf.Lerp(c_vcam.m_Lens.FieldOfView, 35f, Time.deltaTime * 5f);
                }
                sprintSpeedMultiplier = 0f;
            }
            // SPRINT
            if(isSprinting){
                c_vcam.m_Lens.FieldOfView = Mathf.Lerp(c_vcam.m_Lens.FieldOfView, 44.9f, Time.deltaTime * 5f);
                sprintSpeedMultiplier = sprintSpeedMultiplierReset;
                if(Mathf.Abs(movePlayerX + movePlayerY) > 0 && velocity < 2.0f){
                    velocity += Time.deltaTime  * acceleration;
                }
                else if(Mathf.Abs(movePlayerX + movePlayerY) <= 2 && velocity > 0.0f){
                    velocity -= Time.deltaTime * deceleration;
                }
            }
        }
        else{
            if(velocity < 3.0f || isFalling){
                velocity += Time.deltaTime  * acceleration;
                isFalling = true;
            }
            else if(velocity > 0.0f && isFalling){
                velocity -= Time.deltaTime * deceleration;
                isFalling = false;
            }

        }
        animator.SetFloat(moveSpeed, velocity);
    }
    #endregion UPDATE
    
    #region INPUTS
    void OnMove(InputValue value){
        Vector2 movement = value.Get<Vector2>();
        movePlayerX = movement.x;
        movePlayerY = movement.y;

    }
    void OnSprint(InputValue value){
        isSprinting = value.isPressed;
        /*if(value.isPressed)
            c_vcam.m_Lens.FieldOfView = 35f;
        else
            c_vcam.m_Lens.FieldOfView = 44.9f;*/
    }
    void OnJump(InputValue value){
        jumped = value.isPressed;
    }
    private void onEnable(){
        inputPlayer.Enable();
    }
    private void onDisable(){
        inputPlayer.Disable();
    }
    private void OnPick(InputValue value){
        pickPressed = value.isPressed;
        pickObject();
        //Debug.Log(pickPressed);
    }
    void OnIncreasePickableHeight(InputValue value){
        increase = value.Get<float>();
        // Debug.Log("Increase: " + value.Get<float>());
    }
    void OnDecreasePickableHeight(InputValue value){
        decrease = value.Get<float>();
        // Debug.Log("Decrease: " + value.Get<float>());
    }
    void OnMouseLookXY(InputValue delta){

    }
    void OnMouseZoom(InputValue axis){
        ZoomXYAxis = axis.Get<float>();
    }
    #endregion INPUTS
    void pickObject(){
        if(pickPressed && pickableObject.GetComponent<Pickable>().canPickObject){
            if(!pickableObject.GetComponent<Pickable>().isPicked){
                pickableObject.GetComponent<Pickable>().isPicked = true;
                pickableObject.GetComponent<Pickable>().changePressedText();
                pickableObject.transform.parent = this.transform;
                pickableObject.GetComponent<Pickable>().changePositionPhysics();
                //pickableObject.GetComponent<Pickable>().pressKey.SetActive(false);
                //pickableObject.transform.position = transform.forward * 5f;
            }
            else{
                pickableObject.transform.parent = null;
                pickableObject.GetComponent<Pickable>().isPicked = false;
                pickableObject.GetComponent<Pickable>().changePositionPhysics();
            }
        }
    }
    void adjustHeightPickable(float increase, float decrease){
        if(pickableObject.GetComponent<Pickable>().isPicked){
            float PickableHV = increaseHeightVelocity;
            // if(!increase)
            //     PickableHV *= -1f;
            if(pickableObject.transform.position.y <= maxPickableHeight && increase >= 1f){
                pickableObject.transform.position += Vector3.up * Time.deltaTime * PickableHV;
            }
            if(pickableObject.transform.position.y >= minPickableHeight && decrease >= 1f){
                pickableObject.transform.position += Vector3.down * Time.deltaTime * PickableHV;
            }
        }
    }

    private void UpdateZoomLevel(){
        if(currentMiddleRigRadius == newMiddleRigRadius) { return; }

        currentMiddleRigRadius = Mathf.Lerp(currentMiddleRigRadius, newMiddleRigRadius, zoomAcceleration * Time.deltaTime);
        currentMiddleRigRadius = Mathf.Clamp(currentMiddleRigRadius, zoomInnerRange, zoomOuterRange);

        c_vcam.m_Orbits[1].m_Radius = currentMiddleRigRadius;
        c_vcam.m_Orbits[0].m_Height = c_vcam.m_Orbits[1].m_Radius;
        c_vcam.m_Orbits[2].m_Height = -c_vcam.m_Orbits[1].m_Radius;
    }
    public void AdjustCameraZooomIndex(float zoomXYAxis){
        if(zoomXYAxis == 0){ return; }
        if(zoomXYAxis < 0){
            newMiddleRigRadius = currentMiddleRigRadius + zoomSpeed;
        }
        if(zoomXYAxis > 0){
            newMiddleRigRadius = currentMiddleRigRadius - zoomSpeed;
        }
    }
    void OnTriggerEnter(Collider collision){
        if(collision.tag == "Terrain"){
            isInAir = false;
        }
    }
    void OnTriggerExit(Collider collision){
        if(collision.tag == "Terrain"){
            isInAir = true;
        }
    }
    void OnExitGame(InputValue value){
        Application.Quit();
        Debug.Log("Se cierra el juego");
        Debug.Break();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
