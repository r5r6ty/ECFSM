using System;
using Unity.Collections.LowLevel.Unsafe;
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

        private int _animIDSpeed = Animator.StringToHash("Speed");
        private int _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");

        private void OnValidate()
        {
            // ≥ı ºªØ
            states[0] = new State
            {
                onEnter = (variables) =>
                {
                    Debug.Log("≥ı ºªØ£∫0");
                    variables.Set("_speed", 0.1f);
                    variables.Set("_animationBlend", 100.1f);
                    variables.Set("_targetRotation", 0.0f);
                    variables.Set("_rotationVelocity", 0.0f);
                    variables.Set("_verticalVelocity", 0.0f);

                    unsafe
                    {
                        ((MyClass<Com>)variables["_com"]).value.ChangeState(10);
                    }
                },
                onUpdate = (V) =>
                {
                    unsafe
                    {
                        #region ≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘≤‚ ‘
                        //MyClass<Transform> t = (MyClass<Transform>)V["transform"];

                        var t2 = V.Get<Transform>("transform");
                        Debug.Log(t2.value);



                        float* _speed = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_speed"]).value);
                        float* _animationBlend = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_animationBlend"]).value);

                        //fixed (float* gg = &ggg.value, gg2 = &ggg3.value)
                        //{
                        //    *gg = *gg + 1.1f;
                        //    *gg2 = *gg2 + 1.1f;
                        //}

                        //void* ptr = UnsafeUtility.AddressOf(ref ggg.value);
                        //void* ptr2 = UnsafeUtility.AddressOf(ref ggg3.value);

                        //float* gg = (float*)ggg.ptr;
                        *_speed = *_speed + 1.1f;

                        //float* gg2 = (float*)ggg3.ptr;
                        *_animationBlend = *_animationBlend + 30.1f;


                        Debug.Log((float)((MyClass<float>)V["_speed"]).value);
                        Debug.Log((float)((MyClass<float>)V["_animationBlend"]).value);

                        //ref object HP = ref V["HP"];

                        //EInt ptr = (EInt)V["HP"];

                        //ptr = ptr + 1;


                        //Debug.Log(ptr);



                        //HP += 1;

                        //Debug.Log("sasdsd " + HP);
                        #endregion
                    }
                }
            };
            // ¥˝ª˙
            states[10] = new State
            {
                onEnter = (variables) =>
                {
                    Debug.Log("¥˝ª˙£∫10");
                },
                onUpdate = (V) =>
                {
                    unsafe
                    {
                        Transform transform = V.Get<Transform>("transform").value;
                        //Rigidbody rigidbody = V.Get<Rigidbody>("rigidbody").value;
                        //CapsuleCollider capsuleCollider = V.Get<CapsuleCollider>("capsuleCollider").value;
                        Animator animator = V.Get<Animator>("animator").value;
                        CharacterController _controller = V.Get<CharacterController>("characterController").value;
                        float* _speed = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_speed"]).value);
                        float* _animationBlend = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_animationBlend"]).value);
                        float* _targetRotation = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_targetRotation"]).value);
                        float* _rotationVelocity = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_rotationVelocity"]).value);
                        float* _verticalVelocity = (float*)UnsafeUtility.AddressOf(ref ((MyClass<float>)V["_verticalVelocity"]).value);

                        Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

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
                            //*_speed = Mathf.Round(*_speed * 1000f) / 1000f;
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
                    }

                },
                onExit = (variables) => { }
            };

            // “∆∂Ø
            states[20] = new State
            {
                onEnter = (variables) =>
                {
                    Debug.Log(20);
                },
                onUpdate = (variables) =>
                {
                    if (!Input.GetKey(KeyCode.W))
                    {
                        //(variables["_com"] as Com).ChangeState(10);
                    }
                },
                onExit = (variables) => { }
            };

            // Ã¯‘æ
            states[30] = new State
            {
                onEnter = (variables) => { },
                onUpdate = (variables) => { },
                onExit = (variables) => { }
            };
        }

    }
}