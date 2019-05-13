// **************************************************************************************************************** //

// This is an original script that is attached to the Player object of Survival Shooter game. We simply modified it 
// slightly to allow our HeartBPM script to influence the movement speed of the Player object.

// The modified areas areas clearly marked below.

// **************************************************************************************************************** //

using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerMovement : MonoBehaviour
    {
        public float speed = 6f;            // The speed that the player will move at (initially).


        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.

        // MODIFICATION CODE: START //
        HeartBPM playerHeartRate;   // This variable will be used to store the HeartBPM
        // MODIFICATION CODE: END   //

#if !MOBILE_INPUT
        int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 100f;          // The length of the ray from the camera into the scene.
#endif

        void Awake ()
        {
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            floorMask = LayerMask.GetMask ("Floor");
#endif

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();

            // MODIFICATION CODE: START //
            playerHeartRate = GetComponent<HeartBPM>();     // Get the HeartBPM component of the player object
            // MODIFICATION CODE: END   //
        }


        void FixedUpdate ()
        {
            // Store the input axes.
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            // MODIFICATION CODE: START //
            changeSpeed();      // Change speed based on a player's heart rate
            // MODIFICATION CODE: END   //

            // Move the player around the scene.
            Move (h, v);

            // Turn the player to face the mouse cursor.
            Turning ();

            // Animate the player.
            Animating (h, v);
        }


        void Move (float h, float v)
        {
            // Set the movement vector based on the axis input.
            movement.Set (h, 0f, v);
            
            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;

            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition (transform.position + movement);
        }


        void Turning ()
        {
#if !MOBILE_INPUT
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation (newRotatation);
            }
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }


        void Animating (float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool ("IsWalking", walking);
        }

        // MODIFICATION CODE: START //

        // This function will change a player's speed from his/her baseline BPM
        // This is a prototype function that allows HeartBPM to influence player's movement
        void changeSpeed()
        {
            if (playerHeartRate.beatsPerMinute >= playerHeartRate.baselineBPM + 5)
            {
                speed = 12.5f;
            }
            else if (playerHeartRate.beatsPerMinute >= playerHeartRate.baselineBPM + 4)
            {
                speed = 12f;
            }
            else if (playerHeartRate.beatsPerMinute >= playerHeartRate.baselineBPM + 3)
            {
                speed = 10f;
            }
            else if (playerHeartRate.beatsPerMinute >= playerHeartRate.baselineBPM + 1)
            {
                speed = 7f;
            }
            else
            {
                speed = 6f;
            }
        }
        // MODIFICATION CODE: END  //
    }
}