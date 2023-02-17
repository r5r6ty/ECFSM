using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECFSM
{
    public class Entity : MonoBehaviour
    {
        public List<Com> coms = new List<Com>();
        // Start is called before the first frame update

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach (var com in coms)
            {
                com.OnUpdate();
            }
        }
    }
}