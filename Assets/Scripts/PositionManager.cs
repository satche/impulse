using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PositionManager
{
    public float sensibility;
    public List<float> coordinates;
    public List<float> angles;
    public float axis_minValue;

    public float axis_maxValue;
    public float angle_minValue;
    public float angle_maxValue;

    public PositionManager(float axisMin = -100, float axisMax = 100, float angleMin = -90, float angleMax = 90)
    {
        sensibility = 0;
        coordinates = new List<float>() { 0f, 0f, 0f };
        angles = new List<float>() { 0f, 0f, 0f };
        axis_minValue = axisMin;
        axis_maxValue = axisMax;
        angle_minValue = angleMin;
        angle_maxValue = angleMax;
    }

    public void UpdateMovementSensibility(float newSensibility)
    {
        sensibility = newSensibility;
    }

    public void StoreSpatialValues(string data)
    {
        // Split the message by comma
        string[] values = data.Split(',');

        // Create the coordinates list with the first 3 values
        coordinates[0] = float.Parse(values[0]);
        coordinates[1] = float.Parse(values[1]);
        coordinates[2] = float.Parse(values[2]);

        // Create the angles list with the last 3 values
        angles[0] = float.Parse(values[3]);
        angles[1] = float.Parse(values[4]);
        angles[2] = float.Parse(values[5]);
    }

    public float NormalizeValue(float value, float sensibility)
    {
        float normalizedValue = 2 * ((value - axis_minValue) / (axis_maxValue - axis_minValue)) - 1;
        Debug.Log("normalizedValue: " + normalizedValue);
        return normalizedValue * sensibility;
    }
}

