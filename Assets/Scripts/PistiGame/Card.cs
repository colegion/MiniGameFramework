using Helpers;
using Interfaces;
using PistiGame.Helpers;
using UnityEngine;
using IPoolable = Interfaces.IPoolable;

namespace PistiGame
{
    public class Card : MonoBehaviour, IPoolable
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private GameObject cardBg;
        [SerializeField] private SpriteRenderer cardFace;
        [SerializeField] private TextMesh cardValue;
        [SerializeField] private Collider cardCollider;
    
        private CardConfig _cardConfig;
        private PoolableTypes _type;
        private int _points;
    
        public void ConfigureSelf(CardConfig config, bool isFaceDown)
        {
            _cardConfig = config;
            if (isFaceDown)
            {
                cardBg.gameObject.SetActive(true);
            }
            else
            {
                cardBg.gameObject.SetActive(false);
            }
        
            cardFace.sprite = PistiUtilities.GetCardSprite(_cardConfig.cardSuit, _cardConfig.cardValue);
            cardValue.text = (int)_cardConfig.cardValue < (int)CardValue.Jack ? $"{(int)_cardConfig.cardValue}" : "";
        }

        public void DisableBackground()
        {
            cardBg.gameObject.SetActive(false);
        }

        public CardConfig GetConfig()
        {
            return _cardConfig;
        }

        public bool IsJackCard()
        {
            return _cardConfig.cardValue == CardValue.Jack;
        }
    
        public void OnPooled()
        {
            _type = PoolableTypes.Card;
            visuals.gameObject.SetActive(false);
            ToggleInteractable(false);
        }

        public void OnFetchFromPool()
        {
            visuals.gameObject.SetActive(true);
            cardBg.gameObject.SetActive(true);
        }

        public void OnReturnPool()
        {
            _cardConfig = new CardConfig();
            transform.localScale = Vector3.one;
            cardFace.sprite = null;
            cardValue.text = "";
            cardBg.gameObject.SetActive(true);
            visuals.gameObject.SetActive(false);
            cardFace.color = new Color(1f, 1f, 1f, 1f);
            ToggleInteractable(false);
        }

        public void ToggleInteractable(bool toggle)
        {
            cardCollider.enabled = toggle;
        }

        public PoolableTypes GetPoolableType()
        {
            return _type;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public SpriteRenderer GetCardFace()
        {
            return cardFace;
        }
    }
}
