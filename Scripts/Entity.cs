using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECFSM
{
    public class Entity : MonoBehaviour
    {
        public List<Com> coms = new List<Com>();

        void Start()
        {
            foreach (var com in coms)
            {
                com.OnStart();
            }
        }

        void Update()
        {
            foreach (var com in coms)
            {
                com.OnUpdate();
            }
        }

        void FixedUpdate()
        {
            foreach (var com in coms)
            {
                com.OnFixedUpdate();
            }
        }
    }
}
