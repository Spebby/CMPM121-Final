using System.Collections;
using UnityEngine;


namespace CMPM.Core {
    public class CoroutineManager : MonoBehaviour {
        public static CoroutineManager Instance;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            Instance = this;
        }

        public void Run(IEnumerator coroutine) {
            StartCoroutine(coroutine);
        }
    }
}