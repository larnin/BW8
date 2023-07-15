using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VacuumData : ScriptableObject
{
    public float moveSpeedMultiplier = 0.5f;
    public float attractDistance = 3;
    public float attractWidth = 1;
    public float attractDelay = 0.5f;
    public float attractSpeed = 5.0f;
    public float attractCatchDistance = 0.5f;
    public GameObject particlesLeftPrefab = null;
    public GameObject particlesUpPrefab = null;
    public GameObject particleDownPrefab = null;
    public float particleAppearDuration = 1.0f;
    public LayerMask attractLayer;
}