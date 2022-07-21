using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace test_client_unity
{
    public class testapp : MonoBehaviour
    {
        void Start()
        {
            Application.quitting += CallQuitFN;
            
        }

        void CallQuitFN()
        {
            Debug.Log("Á¾·á");
        }
    }
}
