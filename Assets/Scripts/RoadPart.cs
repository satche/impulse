using System;
using UnityEngine;

[Serializable]
public class RoadPart
{
	[Tooltip("Name of the road part. Used to identify the road part in the road generator")]
	public string partName;

	[Tooltip("The road part prefab")]
	public GameObject gameObject;

	[Tooltip("How many time this road part appears in the road")]
	[Range(1, 100)]
	public int iterations;
}