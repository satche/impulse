using System;
using UnityEngine;

[Serializable]
public class RoadPart
{
	[Tooltip("The road part prefab")]
	public GameObject gameObject;

	[Tooltip("How many time this road part appears in the road")]
	[Range(1, 30)]
	public int iterations;
}