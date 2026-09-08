using UnityEngine;

namespace Features.Persistent
{
    public class Persistent : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}
