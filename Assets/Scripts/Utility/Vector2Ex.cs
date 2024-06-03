using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Vector2Ex
{
    //angle in radiants
    public static Vector2 Rotate(Vector2 vect, float angle)
    {
        float c = Mathf.Cos(angle);
        float s = Mathf.Sin(angle);

        return new Vector2(vect.x * c + vect.y * s, vect.x * s + vect.y * c);
    }
}
