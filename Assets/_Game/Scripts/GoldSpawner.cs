using DG.Tweening;
using TMPro;
using UnityEngine;

namespace _Game.Scripts
{
    public class GoldSpawner : MonoBehaviour
    {
        public GameObject goldPrefab;

        public void SpawnGold(Vector3 position, int goldAmount)
        {
            var spawnedGold = Instantiate(goldPrefab, position, Quaternion.identity);

            var textMeshPro = spawnedGold.GetComponent<TextMeshPro>();
            if (textMeshPro != null)
            {
                textMeshPro.text = $"+{goldAmount}G";
                textMeshPro.color = Color.yellow;
            }

            float randomX = Random.Range(-0.5f, 0.5f);
            float randomY = Random.Range(1f, 2f);
            spawnedGold.transform.DOJump(position + new Vector3(randomX, 0, 0), randomY, 1, 1f).SetEase(Ease.OutBounce);

            if (textMeshPro != null)
            {
                textMeshPro.DOFade(0, 1f).SetEase(Ease.InQuad).SetDelay(1f).OnComplete(() =>
                {
                    DataManager.SetCoin(DataManager.currCoin + goldAmount);
                });
            }

            Destroy(spawnedGold, 2f);
        }
    }
}