using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Normalize and pass the position and angles values received from the UPD client to a GameObject
/// </summary>
public class PositionManager : MonoBehaviour
{

    private float[] coordinates;
    private float[] angles;

    [Header("Coordinates")]
    [Tooltip("The range between the min and max possible values received for the axis")]
    public float[] coordinates_inputRange;

    [Tooltip("How sensitive the game object movement are according to the received position values. 0 nullifies all received input, 1 doesn't change the received input.")]
    public float coordinates_sensibility;


    [Header("Angles")]
    [Tooltip("The range between the min and max possible values received for the angles")]
    public float[] angles_inputRange;

    [Tooltip("How sensitive the game object rotation is according to the received angle values. 0 nullifies all received input, 1 doesn't change the received input.")]
    public float angles_sensibility;


    public PositionManager()
    {
        // Coordinates
        coordinates = new float[3] { 0, 0, 0 };
        coordinates_inputRange = new float[2] { -1, 1 };
        coordinates_sensibility = 1;

        // Angles
        angles = new float[3] { 0, 0, 0 };
        angles_inputRange = new float[2] { 0, 0 };
        angles_sensibility = 1;

    }

    /// <summary>
    /// Updates the position of the game object based on the provided spatial data.
    /// </summary>
    /// <param name="data">A string with coordinates and angles: (x, y, z, theta_x, theta_y, theta_z)</param>
    public float[] updatePosition(string data)
    {
        // Store the spatial values in the class properties
        StoreSpatialValues(data);
        NormalizeValues();

        // Update game object position and angle    
        float x = coordinates[0];
        float y = coordinates[2];
        float z = coordinates[1];

        float theta_x = angles[0];
        float theta_y = angles[1];
        float theta_z = angles[2];

        return new float[6] { x, y, z, theta_x, theta_y, theta_z };
    }

    /// <summary>
    /// Stores the spatial values in the <c>coordinates</c> and <c>angles</c> class properties.
    /// </summary>
    /// <param name="data">A string that include position and angle values: (x,y,z,theta_x,theta_y,theta_z)</param>
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

    /// <summary>
    /// Normalizes coordinates and angles properties based on the min/max values of the axis and angles. Add a sensibility factor.
    /// </summary>
    public void NormalizeValues()
    {
        // Normalize the coordinates
        float min = coordinates_inputRange[0];
        float max = coordinates_inputRange[1];

        // Normalize the values between -1 and 1
        for (int i = 0; i < coordinates.Length; i++)
        {
            float minmaxNormalized_value = 2 * ((coordinates[i] - min) / (max - min)) - 1;
            coordinates[i] = minmaxNormalized_value * coordinates_sensibility;
        }

    }
}

