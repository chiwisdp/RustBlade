using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Walkthrough.ChargingState
{
    public class MyPlayer : MonoBehaviour
    {
        public Transform CameraFollowPoint;
        public MyCharacterController Character;
        public float MouseSensitivity = 0.01f;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform

            // Ignore the character's collider(s) for camera obstruction checks
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            /* if (Input.GetKeyDown(KeyCode.E))
            {
                Item item = ItemDatabase.Instance.GetItem("sword");
                //Debug.Log(item.Stats);
		        FindObjectOfType<InventoryController>().EquipItem(item);
            } */

            //HandleCameraInput();
            HandleCharacterInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight * MouseSensitivity, mouseLookAxisUp * MouseSensitivity, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);
            characterInputs.ChargingDown = Input.GetButtonDown("Action4");
            characterInputs.Shield = Input.GetButton("Action0");
            characterInputs.EnergyCharge = Input.GetButton("Action3");
            characterInputs.Action1 = Input.GetButtonDown("Action1");
            characterInputs.Action2 = Input.GetButtonDown("Action2");
            characterInputs.Action3 = Input.GetButton("Action3");
            characterInputs.Action4 = Input.GetButton("Action4");
            characterInputs.Action5 = Input.GetButton("Action5");
            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }
}