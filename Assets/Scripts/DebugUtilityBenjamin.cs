using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugUtilityBenjamin
{
    public static void Draw3DCross(Vector3 position, float size, Color color)
    {
        // Draw line along the X axis
        Debug.DrawLine(position - Vector3.right * size, position + Vector3.right * size, color);

        // Draw line along the Y axis
        Debug.DrawLine(position - Vector3.up * size, position + Vector3.up * size, color);

        // Draw line along the Z axis
        Debug.DrawLine(position - Vector3.forward * size, position + Vector3.forward * size, color);
    }
}
