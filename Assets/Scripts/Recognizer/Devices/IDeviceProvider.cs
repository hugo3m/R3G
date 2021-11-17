using UnityEngine;

namespace Recognizer
{
    public abstract class IDeviceProvider : MonoBehaviour
    {
        public abstract Vector3 CameraPosRelativ { get; }
    }
}