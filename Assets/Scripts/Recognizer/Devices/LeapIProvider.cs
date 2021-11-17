using UnityEngine;

namespace Recognizer
{
    //Just to be detected as provider
    public class LeapIProvider : IDeviceProvider
    {
        public override Vector3 CameraPosRelativ { get; } = new Vector3(0,0.3f,-1);
    }
}