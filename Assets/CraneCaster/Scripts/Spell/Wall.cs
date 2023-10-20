using System;
using UnityEngine;

public class Wall : MonoBehaviour {
	void Start() {
		Destroy(gameObject, 10);
	}
}
