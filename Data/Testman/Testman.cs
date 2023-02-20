using System;
using UnityEngine;

namespace ECFSM
{
    [CreateAssetMenu(fileName = "Testman", menuName = "ECFSM/Testman")]
    public class Testman : Data
    {
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDGrounded = Animator.StringToHash("Grounded");
        private int _animIDJump = Animator.StringToHash("Jump");
        private int _animIDFreeFall = Animator.StringToHash("FreeFall");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void OnValidate()
        {
            unsafe
            {
                // ≥ı ºªØ
                states[0] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("≥ı ºªØ£∫0");
                        _V.Set("_speed", 0.0f);
                        _V.Set("_animationBlend", 0.0f);
                        _V.Set("_targetRotation", 0.0f);
                        _V.Set("_rotationVelocity", 0.0f);
                        _V.Set("_verticalVelocity", 0.0f);
                        _V.Set("_terminalVelocity", 53.0f);
                        _V.Set("Grounded", true);

                        _V.Set("_jumpTimeoutDelta", 0.0f);
                        _V.Set("_fallTimeoutDelta", 0.0f);

                        _V.Get<Com>("_com").ChangeState(10);
                    },
                    onUpdate = (_V) =>
                    {
                        #region ≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘
                        //Val<Transform> t = (Val<Transform>)V["transform"];

                        //var t2 = _V.Get<Transform>("transform");
                        //Debug.Log(t2);



                        //Val<int> ggg = (Val<int>)_V["HP"];
                        //Val<float> ggg2 = (Val<float>)_V["fff"];

                        //fixed (float* gg = &ggg, gg2 = &ggg3)
                        //{
                        //    *gg = *gg + 1.1f;
                        //    *gg2 = *gg2 + 1.1f;
                        //}

                        //void* ptr = UnsafeUtility.AddressOf(ref ggg);
                        //void* ptr2 = UnsafeUtility.AddressOf(ref ggg2);
                        void* ptr = _V.GetPointer<int>("HP");
                        void* ptr2 = _V.GetPointer<float>("fff");

                        int* gg = (int*)_V.GetPointer<int>("HP");
                        *gg = *gg + 1;

                        float* gg2 = (float*)ptr2;
                        *gg2 = *gg2 + 30.1f;


                        //Debug.Log((float)((Val<int>)_V["HP"]));
                        //Debug.Log((float)((Val<float>)_V["fff"]));

                        //ref object HP = ref V["HP"];

                        //EInt ptr = (EInt)V["HP"];

                        //ptr = ptr + 1;


                        //Debug.Log(ptr);



                        //HP += 1;

                        //Debug.Log("sasdsd " + HP);
                        #endregion
                    }
                };
                // ¥˝ª˙/“∆∂Ø
                states[10] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("¥˝ª˙/“∆∂Ø£∫10");
                    },
                    onUpdate = (_V) =>
                    {

                        Transform transform = _V.Get<Transform>("transform");
                        //Rigidbody rigidbody = V.Get<Rigidbody>("rigidbody");
                        //CapsuleCollider capsuleCollider = V.Get<CapsuleCollider>("capsuleCollider");
                        Animator animator = _V.Get<Animator>("animator");
                        CharacterController _controller = _V.Get<CharacterController>("characterController");
                        float* _speed = (float*)_V.GetPointer<float>("_speed");
                        float* _animationBlend = (float*)_V.GetPointer<float>("_animationBlend");
                        float* _targetRotation = (float*)_V.GetPointer<float>("_targetRotation");
                        float* _rotationVelocity = (float*)_V.GetPointer<float>("_rotationVelocity");
                        float* _verticalVelocity = (float*)_V.GetPointer<float>("_verticalVelocity");

                        Vector2 move = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                        // set target speed based on move speed, sprint speed and if sprint is pressed
                        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;

                        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                        // if there is no input, set the target speed to 0
                        if (move == Vector2.zero) targetSpeed = 0.0f;

                        // a reference to the players current horizontal velocity
                        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

                        float speedOffset = 0.1f;
                        float inputMagnitude = /*_input.analogMovement ? _input.move.magnitude : */1f;

                        // accelerate or decelerate to target speed
                        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                            currentHorizontalSpeed > targetSpeed + speedOffset)
                        {
                            // creates curved result rather than a linear one giving a more organic speed change
                            // note T in Lerp is clamped, so we don't need to clamp our speed
                            *_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                                Time.deltaTime * SpeedChangeRate);

                            // round speed to 3 decimal places
                            *_speed = Mathf.Round(*_speed * 1000f) / 1000f;
                        }
                        else
                        {
                            *_speed = targetSpeed;
                        }

                        *_animationBlend = Mathf.Lerp(*_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
                        if (*_animationBlend < 0.01f) *_animationBlend = 0f;

                        // normalise input direction
                        Vector3 inputDirection = new Vector3(move.x, 0.0f, move.y).normalized;

                        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                        // if there is a move input rotate player when the player is moving
                        if (move != Vector2.zero)
                        {
                            *_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                                Camera.main.transform.eulerAngles.y;
                            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, *_targetRotation, ref *_rotationVelocity,
                                RotationSmoothTime);

                            // rotate to face input direction relative to camera position
                            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                        }


                        Vector3 targetDirection = Quaternion.Euler(0.0f, *_targetRotation, 0.0f) * Vector3.forward;

                        // move the player
                        _controller.Move(targetDirection.normalized * (*_speed * Time.deltaTime) +
                                            new Vector3(0.0f, *_verticalVelocity, 0.0f) * Time.deltaTime);

                        // update animator if using character
                        if (animator != null)
                        {
                            animator.SetFloat(_animIDSpeed, *_animationBlend);
                            animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                        }

                        if (Input.GetKey(KeyCode.Space))
                        {
                            _V.Get<Com>("_com").ChangeState(30);
                        }

                    },
                    onExit = (_V) => { }
                };
                // ∆Ã¯
                states[30] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("∆Ã¯£∫30");

                        Animator animator = _V.Get<Animator>("animator");
                        float* _verticalVelocity = (float*)_V.GetPointer<float>("_verticalVelocity");

                        // the square root of H * -2 * G = how much velocity needed to reach desired height
                        *_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                        // update animator if using character
                        animator.SetBool(_animIDJump, true);
                    },
                    onUpdate = (_V) =>
                    {

                        Transform transform = _V.Get<Transform>("transform");
                        Animator animator = _V.Get<Animator>("animator");
                        float* _verticalVelocity = (float*)_V.GetPointer<float>("_verticalVelocity");
                        float* _terminalVelocity = (float*)_V.GetPointer<float>("_terminalVelocity");
                        bool* Grounded = (bool*)_V.GetPointer<bool>("Grounded");
                        CharacterController _controller = _V.Get<CharacterController>("characterController");
                        float* _targetRotation = (float*)_V.GetPointer<float>("_targetRotation");
                        float* _speed = (float*)_V.GetPointer<float>("_speed");
                        LayerMask* GroundLayers = (LayerMask*)_V.GetPointer<LayerMask>("GroundLayers");


                        if (*_verticalVelocity < 0.0f)
                        {
                            //animator.SetBool(_animIDFreeFall, true);
                            // set sphere position, with offset
                            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
                            *Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, *GroundLayers,
                            QueryTriggerInteraction.Ignore);

                            // update animator if using character
                            if (animator != null)
                            {
                                animator.SetBool(_animIDGrounded, *Grounded);
                            }
                            if (*Grounded)
                            {
                                animator.SetBool(_animIDJump, false);

                                _V.Get<Com>("_com").ChangeState(10);
                            }
                        }
                        else
                        {
                            //animator.SetBool(_animIDFreeFall, false);
                        }



                        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
                        if (*_verticalVelocity < *_terminalVelocity)
                        {
                            *_verticalVelocity += Gravity * Time.deltaTime;
                        }
                        Vector3 targetDirection = Quaternion.Euler(0.0f, *_targetRotation, 0.0f) * Vector3.forward;
                        // move the player
                        _controller.Move(targetDirection.normalized * (*_speed * Time.deltaTime) +
                                     new Vector3(0.0f, *_verticalVelocity, 0.0f) * Time.deltaTime);
                    },
                    onExit = (_V) => { }
                };
            }
        }
    }
}