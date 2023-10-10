using Photon.Pun;
using UnityEngine;

public class Trash : MonoBehaviour {
	public void TrashObj(GameObject obj) {
		if (obj.GetPhotonView() != null) {
			PhotonNetwork.Destroy(obj);
		} else {
			Debug.LogError("Unable to trash an object with no PhotonView.");
		}
	}
}
