using PistiGame;
using UnityEngine;

namespace Helpers
{
    public class CardSlot : MonoBehaviour
    {
        [SerializeField] private Transform cardTarget;
        
        private Card _cardRef;
        
        public void SetCardReference(Card card)
        {
            _cardRef = card;
        }

        public Transform GetTarget()
        {
            return cardTarget;
        }

        public void Reset()
        {
            _cardRef = null;
        }
        
        public bool IsAvailable => _cardRef == null;
        public Card GetCardReference => _cardRef; 
    }
}
