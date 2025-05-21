using DG.Tweening;
using TMPro;
using UnityEngine;

namespace PistiGame.Helpers
{
    public class UserUIHelper : MonoBehaviour
    {
        [SerializeField] private User user;
        [SerializeField] private TextMeshProUGUI totalCardCountField;
        [SerializeField] private TextMeshProUGUI totalPointField;

        private int _previousCollectedCount;
        private int _previousPoint;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void HandleOnCollectedCardsUpdated(int collectedCardCount, int points)
        {
            DOTween.To(() => _previousCollectedCount, x =>
            {
                _previousCollectedCount = x;
                totalCardCountField.text = $"Collected: {x}";
            }, collectedCardCount, 0.5f).SetEase(Ease.OutQuad);

            totalCardCountField.transform.DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => totalCardCountField.transform.DOScale(1f, 0.2f));
            
            DOTween.To(() => _previousPoint, x =>
            {
                _previousPoint = x;
                totalPointField.text = $"Points: {x}";
            }, points, 0.5f).SetEase(Ease.OutQuad);

            totalPointField.transform.DOScale(1.2f, 0.2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() => totalPointField.transform.DOScale(1f, 0.2f));
        }


        private void AddListeners()
        {
            user.OnCollectedCardsUpdated += HandleOnCollectedCardsUpdated;
        }

        private void RemoveListeners()
        {
            user.OnCollectedCardsUpdated -= HandleOnCollectedCardsUpdated;
        }
    }
}
