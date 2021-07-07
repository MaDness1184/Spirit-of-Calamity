using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extension // Class to convert a Vector3 into a Vector2
{
    public static Vector2 AsVector2(this Vector3 _v)
    {
        return new Vector2(_v.x, _v.y); //put Vector3's x and y values in new Vecotor2
    }
}
