using UnityEngine;
using DG.Tweening;
using TMPro;

public class DamageTextSpawner : MonoBehaviour
{
    public GameObject damageTextPrefab;

    public void SpawnDamageText(Vector3 position, uint damage)
    {
        var spawnedText = Instantiate(damageTextPrefab, position, Quaternion.identity);

        var text = spawnedText.GetComponent<TextMeshPro>();
        if (text != null)
        {
            text.text = $"-{damage}";
        }

        var targetPosition = position + Vector3.up * 1.4f; 
        spawnedText.transform.DOMove(targetPosition, .5f).SetEase(Ease.OutCubic);

        if (text != null)
        {
            text.DOFade(0, 1f).SetEase(Ease.OutCubic); 
        }
        Destroy(spawnedText, 1.5f);
    }
}