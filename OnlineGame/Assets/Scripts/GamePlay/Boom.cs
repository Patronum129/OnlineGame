using System;
using System.Collections;
using UnityEngine;

namespace GamePlay
{
    public class Boom : MonoBehaviour
    {
        private float timer;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1.5f);
            
            GetComponent<Animator>().SetTrigger("Boom");
        }
    }
}