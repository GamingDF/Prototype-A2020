using UnityEngine;
using TMPro;

public class UITextUpdateHealth : MonoBehaviour {
	[SerializeField] Health _health = null;
	TextMeshProUGUI _text = null;

	void Awake() {
		_text = gameObject.GetComponent<TextMeshProUGUI>();
		if (_text == null) {
			Debug.LogWarning("no textmeshpro component");
		}

		_health.healthUpdate += UpdateText;
	}

	void UpdateText(int p_health) {
		_text.text = "HP: " + p_health;
	}
}
