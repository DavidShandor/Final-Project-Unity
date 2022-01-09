using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lrTesting : MonoBehaviour
{
    [SerializeField] private Transform[] points;
    [SerializeField] private lr_LineController line;

    private void Start()
    {
        line.SetUpLine(points);

    }
}
