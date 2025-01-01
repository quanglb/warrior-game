using TMPro;
using UnityEngine;

namespace _Game.Scripts
{
    public class CoinGroup : MonoBehaviour
    {
        public TextMeshProUGUI txtCoin;
        //public Transform icon;

        void Start()
        {
            DataManager.OnChangeCoin += OnChangeCoin;
            ShowCoin();

            DataManager.SetCoin(DataManager.GetCoin());
        }

        private void OnDestroy()
        {
            DataManager.OnChangeCoin -= OnChangeCoin;
        }

        private void OnChangeCoin(int coinValue)
        {
            OnEffectAddCoin();
            ShowCoin();
        }

        private void ShowCoin()
        {
            if (txtCoin == null) return;

            txtCoin.text = $"{DataManager.currCoin}G";
            txtCoin.transform.localScale = Vector3.one;
            // if (icon == null) return;
            // DOTween.Kill(txtCoin.transform);
            // icon.DOPunchScale(Vector3.one * 0.05f, 0.05f).SetEase(Ease.InOutBack).SetRelative(true)
            //     .SetLoops(3, LoopType.Restart).OnComplete(() => { icon.DOScale(1f, 0.2f); });
        }

        public void OnEffectAddCoin()
        {
        }
    }
}