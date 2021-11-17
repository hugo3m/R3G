using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Recognizer
{
    //Just to be detected as provider
    public class KinectV2IProvider : IDeviceProvider
    {
        public override Vector3 CameraPosRelativ { get; } = new Vector3(0, 0, -10);
    }
}
