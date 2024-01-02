using System;
using UnityEngine;

[Serializable]
public class TerrainAsset
{
	[Tooltip("The asset prefab")]
	public GameObject gameObject;

	[Tooltip("How many time this asset should appears in the scene")]
	[Range(1, 30)]
	public int iterations = 3;

	[Tooltip("Minimum size variation")]
	[Range(0.1f, 2)]
	public float minScaleVariation = 0.6f;

	[Tooltip("Maximum size variation")]
	[Range(0.1f, 2)]
	public float maxScaleVariation = 1.2f;

	[Tooltip("Minimum horizontal position variation")]
	[Range(-200, 200)]
	public int minPositionVariation = -50;

	[Tooltip("Maximum horizontal position variation")]
	[Range(-200, 200)]
	public int maxPositionVariation = 150;

	[Tooltip("Minimum vertical position variation")]
	[Range(-100, 100)]
	public int minAltitudeVariation = -20;

	[Tooltip("Maximum vertical position variation")]
	[Range(-100, 100)]
	public int maxAltitudeVariation = 30;
}