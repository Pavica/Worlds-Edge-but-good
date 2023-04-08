using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    public class RigidbodyFirstPersonController : MonoBehaviour
    {
        [HideInInspector]
        public float ForwardSpeed;   // Speed when walking forward
        [HideInInspector]
        public float BackwardSpeed;  // Speed when walking backwards
        [HideInInspector]
        public float StrafeSpeed;    // Speed when walking sideways
        public float RunMultiplier;   // Speed when sprinting
        public KeyCode RunKey = KeyCode.LeftShift;
        public float JumpForce;
        public AnimationCurve SlopeCurveModifier = new AnimationCurve(new Keyframe(-90.0f, 1.0f), new Keyframe(0.0f, 1.0f), new Keyframe(90.0f, 0.0f));
        [HideInInspector] public float CurrentTargetSpeed;
        private bool m_Running;
            
        public void UpdateDesiredTargetSpeed(Vector2 input)
        {
            if (input == Vector2.zero) return;
            if (input.x > 0 || input.x < 0)
            {
                //strafe
                CurrentTargetSpeed = StrafeSpeed;
            }
            if (input.y < 0)
            {
                //backwards
                CurrentTargetSpeed = BackwardSpeed;
            }
            if (input.y > 0)
            {
                //forwards
                //handled last as if strafing and moving forward at the same time forwards speed should take precedence
                CurrentTargetSpeed = ForwardSpeed;
            }
            if (Input.GetKey(RunKey))
            {
                CurrentTargetSpeed *= RunMultiplier;
                m_Running = true;
            }
            else
            {
                m_Running = false;
            }
        }

        public bool Running
        {
            get { return m_Running; }
        }


        [Serializable]
        public class AdvancedSettings
        {
            public float groundCheckDistance = 0.01f; // distance for checking if the controller is grounded ( 0.01f seems to work best for this )
            public float stickToGroundHelperDistance = 0.5f; // stops the character
            public float slowDownRate = 20f; // rate at which the controller comes to a stop when there is no input
            public bool airControl; // can the user control the direction that is being moved in the air
            [Tooltip("set it to 0.1 or more if you get stuck in wall")]
            public float shellOffset; //reduce the radius by that ratio to avoid getting stuck in wall (a value of 0.1f is nice)
        }


        public Camera cam;
        public MouseLook mouseLook = new MouseLook();
        public AdvancedSettings advancedSettings = new AdvancedSettings();


        private Rigidbody m_RigidBody;
        private CapsuleCollider m_Capsule;
        private float m_YRotation;
        private Vector3 m_GroundContactNormal;
        private bool m_Jump, m_PreviouslyGrounded, m_Jumping, m_IsGrounded;


        public Vector3 Velocity
        {
            get { return m_RigidBody.velocity; }
        }

        public bool Grounded
        {
            get { return m_IsGrounded; }
        }

        public bool Jumping
        {
            get { return m_Jumping; }
        }


        private void Start()
        {
            m_RigidBody = GetComponent<Rigidbody>();
            m_Capsule = GetComponent<CapsuleCollider>();
            mouseLook.Init(transform, cam.transform);

            //our code
            maxHealth = baseMaxHealth;
            health = maxHealth;
            damage = baseDamage;
            attackSpeedAmount = baseAttackSpeed;
           
            healthBar.setMaxHealth(maxHealth);

            ForwardSpeed = baseSpeed;
            StrafeSpeed = baseSpeed / 2;
            BackwardSpeed = baseSpeed / 2;

            anim = sword.GetComponent<Animator>();
        }

    private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Escape)) {
                Debug.Break();
            }


            if (Input.GetButtonDown("Jump") && !m_Jump)
            {
                m_Jump = true;
            }
    }

    private void FixedUpdate()
    {
            //our code
            hitEnemy();

            RotateView();
            GroundCheck();
            Vector2 input = GetInput();

            if ((Mathf.Abs(input.x) > float.Epsilon || Mathf.Abs(input.y) > float.Epsilon) && (advancedSettings.airControl || m_IsGrounded))
            {
                // always move along the camera forward as it is the direction that it being aimed at
                Vector3 desiredMove = cam.transform.forward * input.y + cam.transform.right * input.x;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, m_GroundContactNormal).normalized;

                desiredMove.x = desiredMove.x * CurrentTargetSpeed;
                desiredMove.z = desiredMove.z * CurrentTargetSpeed;
                desiredMove.y = desiredMove.y * CurrentTargetSpeed;
                if (m_RigidBody.velocity.sqrMagnitude <
                    (CurrentTargetSpeed * CurrentTargetSpeed))
                {
                    m_RigidBody.AddForce(desiredMove * SlopeMultiplier(), ForceMode.Impulse);
                }
            }

            if (m_IsGrounded)
            {
                m_RigidBody.drag = 5f;

                if (m_Jump)
                {
                    m_RigidBody.drag = 0f;
                    m_RigidBody.velocity = new Vector3(m_RigidBody.velocity.x, 0f, m_RigidBody.velocity.z);
                    m_RigidBody.AddForce(new Vector3(0f, JumpForce, 0f), ForceMode.Impulse);
                    m_Jumping = true;
                }

                if (!m_Jumping && Mathf.Abs(input.x) < float.Epsilon && Mathf.Abs(input.y) < float.Epsilon && m_RigidBody.velocity.magnitude < 1f)
                {
                    m_RigidBody.Sleep();
                }
            }
            else
            {
                m_RigidBody.drag = 0f;
                if (m_PreviouslyGrounded && !m_Jumping)
                {
                    StickToGroundHelper();
                }
            }
            m_Jump = false;
    }


        private float SlopeMultiplier()
        {
            float angle = Vector3.Angle(m_GroundContactNormal, Vector3.up);
            return SlopeCurveModifier.Evaluate(angle);
        }


        private void StickToGroundHelper()
        {
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) +
                                   advancedSettings.stickToGroundHelperDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                if (Mathf.Abs(Vector3.Angle(hitInfo.normal, Vector3.up)) < 85f)
                {
                    m_RigidBody.velocity = Vector3.ProjectOnPlane(m_RigidBody.velocity, hitInfo.normal);
                }
            }
        }

        private Vector2 GetInput()
        {

            Vector2 input = new Vector2
            {
                x = Input.GetAxis("Horizontal"),
                y = Input.GetAxis("Vertical")
            };
            UpdateDesiredTargetSpeed(input);
            return input;
        }


        private void RotateView()
        {
            //avoids the mouse looking if the game is effectively paused
            if (Mathf.Abs(Time.timeScale) < float.Epsilon) return;

            // get the rotation before it's changed
            float oldYRotation = transform.eulerAngles.y;

            mouseLook.LookRotation(transform, cam.transform);

            if (m_IsGrounded || advancedSettings.airControl)
            {
                // Rotate the rigidbody velocity to match the new direction that the character is looking
                Quaternion velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
                m_RigidBody.velocity = velRotation * m_RigidBody.velocity;
            }
        }

        /// sphere cast down just beyond the bottom of the capsule to see if the capsule is colliding round the bottom
        private void GroundCheck()
        {
            m_PreviouslyGrounded = m_IsGrounded;
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, m_Capsule.radius * (1.0f - advancedSettings.shellOffset), Vector3.down, out hitInfo,
                                   ((m_Capsule.height / 2f) - m_Capsule.radius) + advancedSettings.groundCheckDistance, ~0, QueryTriggerInteraction.Ignore))
            {
                m_IsGrounded = true;
                m_GroundContactNormal = hitInfo.normal;
            }
            else
            {
                m_IsGrounded = false;
                m_GroundContactNormal = Vector3.up;
            }
            if (!m_PreviouslyGrounded && m_IsGrounded && m_Jumping)
            {
                m_Jumping = false;
            }
        }

        //our code ----------------------------------------------------------------
        public GameObject sword;
        public Animator anim;
        public HealthBar healthBar;
        public GameObject pauseMenu;

        [HideInInspector]
        public float attackSpeedAmount;
        public float baseAttackSpeed;  

        public float baseDamage;
        public float baseMaxHealth;
        [HideInInspector]
        public float damage;
        [HideInInspector]
        public float health;
        [HideInInspector]
        public float maxHealth;
        [HideInInspector]
        public float attackingEnemyDamage = 10;
        public float baseSpeed;
        [HideInInspector]
        public bool attackFinished = true;

         private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "rock")
            {
                Debug.Log("Enemy hit the player");
                TakeDamage(attackingEnemyDamage);
                Debug.Log("Player HP:" + health);
            }
        }

        public void TakeDamage(float damage)
        {
            health -= damage;

            if (health <= 0) Invoke(nameof(DestroyPlayer), 0.5f);
            healthBar.setHealth(health);
        }

        public void resetHealth()
        {
            health = maxHealth;
            healthBar.setHealth(maxHealth);
        }

        private void DestroyPlayer()
        {
            SceneManager.LoadScene(2);
        }

        private void hitEnemy()
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
 
                anim.SetTrigger("attack");

                if (attackFinished)
                {
                    attackFinished = false;

                Vector3 pos = cam.transform.position + cam.transform.forward * 1;
                Vector3 scale = new Vector3(0.25f, 0.5f, 2);
                Quaternion rotation = cam.transform.rotation;

                //visualize the attack range
                //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //cube.transform.position = pos;
                //cube.transform.rotation = rotation;
                //cube.transform.localScale = scale;
                
                Collider[] hitColliders = Physics.OverlapBox(pos, scale, rotation);
                foreach (var hitCollider in hitColliders)
                    {
                        if (hitCollider.name == "EnemyLegs(Clone)" || hitCollider.name == "EnemyShark(Clone)" || hitCollider.name == "EnemySlime(Clone)")
                            {
                                hitCollider.gameObject.GetComponent<EnemyAiTutorial>().getHit();
                        }
                    }
                } 
            }
        }
    }
