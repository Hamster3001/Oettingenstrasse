using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    public class MyFirstPersonController : MonoBehaviour
    {
		[SerializeField] private float stepSize;					// in meters
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    	// an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_LandSound;           	// the sound played when character touches back on ground.

		private Vector3 oldPosition;
		private bool step;
		private float stepDistance;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private float m_StepCycle;
        private float m_NextStep;
        private AudioSource m_AudioSource;

        // Use this for initialization
        private void Start()
        {
			step = false;
            m_CharacterController = GetComponentInParent<CharacterController>();
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_AudioSource = GetComponentInParent<AudioSource>();
            CrossPlatformInputManager.SwitchActiveInputMethod (CrossPlatformInputManager.ActiveInputMethod.Hardware);
        }


        // Update is called once per frame
        private void Update()
        {
            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
     //           PlayLandingSound();
                m_MoveDir.y = 0f;
            }
            if (!m_CharacterController.isGrounded && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }
            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
//            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }


        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);

            // always move along the camera forward as it is the direction that it being aimed at
			Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f);
            //desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }

			// Movement
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

			stepDistance = Vector3.Distance(oldPosition, m_CharacterController.transform.position);
			if (stepDistance >= stepSize) {
				step = false;
			}

            ProgressStepCycle(speed);
        }

        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * 1f)) * Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

        //    PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
 //           m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }

        private void GetInput(out float speed)
        {
			// Test
			if (Input.GetKeyDown (KeyCode.V))
				MakeStep ();

            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

			if (step) {
				horizontal = 0.0f;
				vertical = 1.0f;
			}

#if MOBILE_INPUT
			if (!Menuscript.vrEnabled) {
				Touch[] touches = Input.touches;
				if (touches.Length == 1) {
					if (touches[0].phase.Equals (TouchPhase.Stationary)
					    && touches[0].position.x > Screen.width-220
					    && touches[0].position.y < 120) {
						horizontal = 0.0f;
						vertical = 1.0f;
					}
				}
			}
#endif

            // set the desired speed to be walking
            speed = m_WalkSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }

		public void MakeStep() {
			oldPosition = m_CharacterController.transform.position;
			stepDistance = 0.0f;

			if (Menuscript.movementEnabled)
				step = true;
		}
		
        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    }
}
