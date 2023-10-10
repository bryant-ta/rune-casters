using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour {
	[SerializeField] Image _hpBar;

	public void UpdateHpBar(float percent) {
		_hpBar.fillAmount = percent;
	}
}
