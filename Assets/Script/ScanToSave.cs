using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanToSave 
{
    public Pose ScanPose;
    public int PipeType;

    public ScanToSave(Pose _pose, int _type)
    {
        ScanPose = _pose;
        PipeType = _type;
    }
}
