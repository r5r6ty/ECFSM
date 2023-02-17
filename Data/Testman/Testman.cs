using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ECFSM
{
    [CreateAssetMenu(fileName = "Testman", menuName = "ECFSM/Testman")]
    public class Testman : Data
    {
        public Testman()
        {
            // ´ý»ú
            states[10] = new State
            {
                onEnter = (variables) =>
                {
                    Debug.Log(10);
                },
                onUpdate = (variables) =>
                {
                    int HP = (int)variables["HP"];
                    HP = 1;
                    ;
                    if (Input.GetKey(KeyCode.W))
                    {
                        (variables["_com"] as Com).ChangeState(20);
                    }
                },
                onExit = (variables) => { }
            };

            // ÒÆ¶¯
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
                        (variables["_com"] as Com).ChangeState(10);
                    }
                },
                onExit = (variables) => { }
            };

            // ÌøÔ¾
            states[30] = new State
            {
                onEnter = (variables) => { },
                onUpdate = (variables) => { },
                onExit = (variables) => { }
            };
        }
    }
}