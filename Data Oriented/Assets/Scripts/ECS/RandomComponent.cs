using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

public struct RandomComponent : IComponentData
{
    public Unity.Mathematics.Random random;
}
