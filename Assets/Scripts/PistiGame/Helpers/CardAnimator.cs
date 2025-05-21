using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PistiGame;
using PistiGame.Helpers;
using UnityEngine;

namespace Helpers
{
    public class CardAnimator : MonoBehaviour
    {
        public static event Action OnPisti;
        public void DecideAnimationToUse(List<Card> cards, Transform target, CollectType type, Action onComplete)
        {
            switch (type)
            {
                case CollectType.Pisti:
                    OnUserGotPisti(cards, target, onComplete);
                    break;
                case CollectType.Jack:
                    OnJackCollected(cards, target, onComplete);
                    break;
                default:
                    OnCardsCollected(cards, target, onComplete);
                    break;
            }
        }
        
        public void AnimateSelectedCard(Card card, Vector3 cardTarget, bool forceDisable, Action onComplete)
        {
            if(forceDisable) card.DisableBackground();
            card.transform.DOMove(cardTarget, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void OnCardsCollected(List<Card> cards, Transform target, Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();

            foreach (var card in cards)
            {
                sequence.Append(card.transform.DOMove(target.position, 0.1f).SetEase(Ease.Linear));
                sequence.AppendInterval(0.03f);
            }
            
            sequence.AppendCallback(() =>
            {
                foreach (var card in cards)
                {
                    PistiGameController.Instance.ReturnObjectToPool(card);
                }
                
                onComplete?.Invoke();
            });
        }
        
        private void OnJackCollected(List<Card> cards, Transform target, Action onComplete)
        {
            Sequence sequence = DOTween.Sequence();
            Card jackCard = cards[^1];
            var face = jackCard.GetCardFace();
            
            sequence.Append(jackCard.transform.DOScale(1.6f, 0.2f).SetEase(Ease.OutBack));
            sequence.Join(face.DOColor(new Color(1f, 1f, 1f, 190 / 255f), 0.3f).SetEase(Ease.OutBack));
            foreach (var card in cards)
            {
                if (card == jackCard) continue;

                sequence.Append(card.transform.DOMove(jackCard.transform.position + Vector3.up * 0.2f, 0.15f).SetEase(Ease.OutExpo));

                sequence.Append(card.transform.DOScale(0f, 0.15f).SetEase(Ease.InBack));
            }
            
            sequence.Append(jackCard.transform.DOMove(target.position, 0.2f).SetEase(Ease.InBack));

            sequence.AppendCallback(() =>
            {
                Camera.main.transform.DOShakePosition(0.2f, 0.25f, 15);
                face.DOColor(new Color(1f, 1f, 1f, 150 / 255f), 0.3f).SetEase(Ease.OutBack);
                face.color = Color.white;
                foreach (var card in cards)
                {
                    PistiGameController.Instance.ReturnObjectToPool(card);
                }
                onComplete?.Invoke();
            });
        }


        private void OnUserGotPisti(List<Card> cards, Transform target, Action onComplete)
        {
            OnPisti?.Invoke();
            Sequence sequence = DOTween.Sequence();

            foreach (var card in cards)
            {
                sequence.Append(card.transform.DOScale(1.5f, 0.15f).SetEase(Ease.OutBack));
                sequence.Append(card.transform.DOMove(target.position, 0.35f).SetEase(Ease.OutExpo))
                    .Join(card.transform.DOScale(1.2f, 0.15f));
                sequence.Append(card.transform.DOScale(0f, 0.12f).SetEase(Ease.InBack));
                sequence.AppendInterval(0.02f);
            }

            sequence.AppendCallback(() =>
            {
                Camera.main.transform.DOShakePosition(0.2f, 0.2f, 20);
        
                foreach (var card in cards)
                {
                    PistiGameController.Instance.ReturnObjectToPool(card);
                }
        
                onComplete?.Invoke();
            });
        }
    }
}
