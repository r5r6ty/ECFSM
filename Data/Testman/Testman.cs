using DictPlus;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

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

        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;
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
                // 初始化
                states[0] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("初始化：0");
                        _V.Set("_speed", 0.0f);
                        _V.Set("_animationBlend", 0.0f);
                        _V.Set("_targetRotation", 0.0f);
                        _V.Set("_rotationVelocity", 0.0f);
                        _V.Set("_verticalVelocity", 0.0f);
                        _V.Set("_terminalVelocity", 53.0f);
                        _V.Set("Grounded", true);

                        _V.Set("_jumpTimeoutDelta", 0.0f);
                        _V.Set("_fallTimeoutDelta", 0.0f);

                        _V.Get("transform", out Transform transform);

                        float* _speed = (float*)_V.Set("dasdas", 0.0f);
                        BoxCollider asdd = _V.Set("boxCollider", transform.gameObject.AddComponent<BoxCollider>());

                        _V.Get<Com>("_com").ChangeState(10);
                    }
                };
                // 待机/移动
                states[10] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("待机/移动：10");
                    },
                    onUpdate = (_V) =>
                    {

                        _V.Get("transform", out Transform transform);
                        //Rigidbody rigidbody = V.Get<Rigidbody>("rigidbody");
                        //CapsuleCollider capsuleCollider = V.Get<CapsuleCollider>("capsuleCollider");
                        _V.Get("animator", out Animator animator);
                        _V.Get("characterController", out CharacterController characterController);
                        _V.Get("_speed", out float* _speed);
                        _V.Get("_animationBlend", out float* _animationBlend);
                        _V.Get("_targetRotation", out float* _targetRotation);
                        _V.Get("_rotationVelocity", out float* _rotationVelocity);
                        _V.Get("_verticalVelocity", out float* _verticalVelocity);

                        Vector2 move = new Vector2(UnityEngine.Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

                        // set target speed based on move speed, sprint speed and if sprint is pressed
                        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? SprintSpeed : MoveSpeed;

                        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

                        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
                        // if there is no input, set the target speed to 0
                        if (move == Vector2.zero) targetSpeed = 0.0f;

                        // a reference to the players current horizontal velocity
                        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

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
                        characterController.Move(targetDirection.normalized * (*_speed * Time.deltaTime) +
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
                    onAnimationEvent = (_V) => {
                        _V.Get("transform", out Transform transform);
                        _V.Get("characterController", out CharacterController characterController);
                        AudioClip LandingAudioClip = (AudioClip)_V.Get<Data>("_data").injectObject["LandingAudioClip"];
                        ////AudioClip[] FootstepAudioClips = _V.Get<AudioClip[]>("FootstepAudioClips");

                        ////var index = Random.Range(0, FootstepAudioClips.Length);
                        ////AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(characterController.center), FootstepAudioVolume);
                        AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(characterController.center), FootstepAudioVolume);
                    },
                    onExit = (_V) => { }
                };
                // 起跳
                states[30] = new State
                {
                    onEnter = (_V) =>
                    {
                        Debug.Log("起跳：30");

                        _V.Get("animator", out Animator animator);
                        _V.Get("_verticalVelocity", out float* _verticalVelocity);

                        // the square root of H * -2 * G = how much velocity needed to reach desired height
                        *_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                        // update animator if using character
                        animator.SetBool(_animIDJump, true);
                    },
                    onUpdate = (_V) =>
                    {

                        _V.Get("transform", out Transform transform);
                        _V.Get("animator", out Animator animator);
                        _V.Get("_verticalVelocity", out float* _verticalVelocity);
                        _V.Get("_terminalVelocity", out float* _terminalVelocity);
                        _V.Get("Grounded", out bool* Grounded);
                        _V.Get("characterController", out CharacterController characterController);
                        _V.Get("_targetRotation", out float* _targetRotation);
                        _V.Get("_speed", out float* _speed);
                        _V.Get("GroundLayers", out LayerMask* GroundLayers);


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
                        characterController.Move(targetDirection.normalized * (*_speed * Time.deltaTime) +
                                     new Vector3(0.0f, *_verticalVelocity, 0.0f) * Time.deltaTime);
                    },
                    onExit = (_V) => {
                        //_V.Del("_speed");
                    }
                };
            }
        }
    }
}