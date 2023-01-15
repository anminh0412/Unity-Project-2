using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine.UI;
using Photon.Pun;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        //Set Network IsMine Player
        PhotonView view;

        //Set Scope AWp
        public GameObject scope;
        public float zoomOnValue;
        public float zoomOffValue;

        public GameObject MainCharacter;
        public GameObject _sceneManager;
        InGameSceneManager iGSceneManager;
        public Camera MainCamera;
        CharacterAttackController characterAttack;
        CharacterHealthController characterHeal;
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;
        //public Image crossHair;
        public RawImage crossHair2;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        public bool isZoom;
        public bool isCrouch;
        public bool _openMouse = false;

        private void Start()
        {
            view = MainCharacter.GetComponent<PhotonView>();
            if (view.IsMine)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Character = MainCharacter.GetComponent<ExampleCharacterController>();
                // Tell camera to follow transform
                CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

                // Ignore the character's collider(s) for camera obstruction checks
                CharacterCamera.IgnoredColliders.Clear();
                CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
                _sceneManager = GameObject.Find("SceneManager");
                iGSceneManager = _sceneManager.GetComponent<InGameSceneManager>();
                characterAttack = MainCharacter.GetComponent<CharacterAttackController>();
                characterHeal = MainCharacter.GetComponent<CharacterHealthController>();
            }
        }
        private void Update()
        {
            if (view.IsMine) { 
                isCrouch = Character.isCrouch;
                if (Input.GetMouseButtonDown(0) && _openMouse == false)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                }else if(_openMouse == true) Cursor.lockState = CursorLockMode.Confined;

                HandleCharacterInput();
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    Character.isCrouch = true;
                }else Character.isCrouch = false;
            }

            //Set Mouse Lock
            if(_sceneManager != null)
            {
                _openMouse = iGSceneManager.openMouse;
            }
        }

        private void LateUpdate()
        {
            if (view.IsMine)
            {
                // Handle rotating the camera along with physics movers
                if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
                {
                    CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                    CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
                }

                HandleCameraInput();
            }
        }

        private void HandleCameraInput()
        {
            if (view.IsMine)
            {
                // Create the look input vector for the camera
                float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
                float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
                Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

                // Prevent moving the camera while the cursor isn't locked
                if (Cursor.lockState != CursorLockMode.Locked)
                {
                    lookInputVector = Vector3.zero;
                }

                // Input for zooming the camera (disabled in WebGL because it can cause problems)
                float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

                // Apply inputs to the camera
                CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

                // Handle toggling zoom level
                if(characterAttack.currentWeapon == 1 && characterHeal.isAlive)
                {
                    if (Input.GetKeyDown(KeyCode.V))
                    {
                        CharacterCamera.TargetDistance = (CharacterCamera.TargetDistance == 2f) ? CharacterCamera.DefaultDistance : 2f;
                    }
                    else if (Input.GetButton("Fire2"))
                    {
                        CharacterCamera.TargetDistance = 2f;
                    }
                    if(Input.GetButtonUp("Fire2")) CharacterCamera.TargetDistance = CharacterCamera.DefaultDistance;

                    if (CharacterCamera.TargetDistance == 2f)
                    {
                        isZoom = true;
                        if (scope)
                        {
                            MainCamera.fieldOfView = zoomOnValue;
                            scope.SetActive(true);
                            Color currColor = crossHair2.color;
                            currColor.a = 0f;
                            crossHair2.color = currColor;
                            CharacterCamera.RotationSpeed = 0.2f;
                        }
                        //crossHair2.rectTransform.sizeDelta = new Vector2(30f, 30f);
                    }
                    else
                    {
                        isZoom = false;
                        if (scope)
                        {
                            MainCamera.fieldOfView = zoomOffValue;
                            scope.SetActive(false);
                            Color currColor = crossHair2.color;
                            currColor.a = 255f;
                            crossHair2.color = currColor;
                            CharacterCamera.RotationSpeed = 1.3f;
                        }
                        //crossHair2.rectTransform.sizeDelta = new Vector2(30f, 30f);
                    }
                    characterAttack.isAim = isZoom;
                }
                else if (characterAttack.currentWeapon == 2 || !characterHeal.isAlive)
                {
                    if (scope)
                    {
                        MainCamera.fieldOfView = zoomOffValue;
                        scope.SetActive(false);
                        Color currColor = crossHair2.color;
                        currColor.a = 255f;
                        crossHair2.color = currColor;
                        CharacterCamera.RotationSpeed = 1.3f;
                    }
                    if (CharacterCamera.TargetDistance == 2f)
                    {
                        CharacterCamera.TargetDistance = CharacterCamera.DefaultDistance;
                    }
                }
            }
        }

        private void HandleCharacterInput()
        {
            if (view.IsMine)
            {
                PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

                // Build the CharacterInputs struct
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
                characterInputs.SlideDown = Input.GetKeyDown(KeyCode.LeftShift);
                //characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
                //characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

                // Apply inputs to character
                Character.SetInputs(ref characterInputs);
            }
        }
    }
}