using UnityEngine;

[CreateAssetMenu(fileName = "HealthInfo", menuName = "ScriptableObjects/HealthInfo")]
public class HealthInfo : ScriptableObject {
	public int MaxHealth => _maxHealth;

	[SerializeField] int _maxHealth = 0;
}
