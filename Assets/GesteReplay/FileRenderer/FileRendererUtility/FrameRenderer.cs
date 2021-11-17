using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRenderer {
    public readonly List<Vector3> coordonnes;
    public readonly List<bool> joinRendered;

    public FrameRenderer(List<Vector3> coordIn,Vector3 minPos)
    {
        joinRendered = new List<bool>();
        coordonnes = new List<Vector3>();
        foreach (Vector3 coord in coordIn)
        {
            joinRendered.Add(coord != Vector3.zero);
            coordonnes.Add(coord );
        }
    }
}
