using System;
using System.Collections.Generic;
using DG.Tweening;
using Helpers;
using PistiGame.GameStates;
using PistiGame.Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PistiGame
{
    public class PistiIntroPanel : MonoBehaviour
    {
        [SerializeField] private GameObject blackishPanel;
        [SerializeField] private TextMeshProUGUI endGameField;
        [SerializeField] private TextMeshProUGUI infoField;
        [SerializeField] private Button startGameButton;
        [SerializeField] private TMP_Dropdown botTypeDropdown;

        [SerializeField] private PistiBootstrapper bootstrapper;

        private BotType _selectedBotType;
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void Start()
        {
            PopulateBotDropdown();
        }

        private void RequestGame()
        {
            blackishPanel.gameObject.SetActive(false);
            bootstrapper.HandleOnGameRequested();
        }
        
        private void PopulateBotDropdown()
        {
            botTypeDropdown.ClearOptions();
            List<string> options = new List<string>(Enum.GetNames(typeof(BotType)));
            botTypeDropdown.AddOptions(options);
        
            botTypeDropdown.onValueChanged.AddListener(index =>
            {
                _selectedBotType = (BotType)index;
                bootstrapper.SetBotType(_selectedBotType);
            });

            _selectedBotType = (BotType)botTypeDropdown.value;
        }

        private void AnimateOnNewRound(int round, Action onComplete)
        {
            infoField.text = $"Round {round}";
            infoField.transform.localScale = Vector3.one;
            infoField.gameObject.SetActive(true);
            Sequence sequence = DOTween.Sequence();

            sequence.Append(infoField.transform.DOScale(1.5f, 0.15f)
                .SetEase(Ease.OutBack));
            sequence.Join(infoField.DOColor(Color.yellow, 0.15f));
            sequence.Append(infoField.transform.DOScale(1.0f, 0.35f)
                .SetEase(Ease.InOutQuad));
            sequence.Join(infoField.DOColor(Color.white, 0.35f));
            sequence.OnComplete(() =>
            {
                infoField.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        }

        private void AnimateOnPisti()
        {
            infoField.text = "PİŞTİ!";
            infoField.transform.localScale = Vector3.one;
            infoField.gameObject.SetActive(true);

            Sequence sequence = DOTween.Sequence();
            sequence.Append(infoField.transform.DOScale(2f, 0.2f).SetEase(Ease.OutBack));
            sequence.Join(infoField.DOColor(Color.yellow, 0.2f));
            sequence.AppendInterval(0.2f);
            sequence.Append(infoField.transform.DOScale(1.0f, 0.2f).SetEase(Ease.InOutQuad));
            sequence.Join(infoField.DOColor(Color.white, 0.2f));
            sequence.AppendCallback(() => infoField.transform.DOShakePosition(0.3f, 10f, 20));
            sequence.AppendInterval(0.3f);
            sequence.AppendCallback(() => infoField.gameObject.SetActive(false));
        }


        private void AddListeners()
        {
            startGameButton.onClick.AddListener(RequestGame);
            CardDistributionState.OnRoundDistributed += AnimateOnNewRound;
            CardAnimator.OnPisti += AnimateOnPisti;
        }

        private void RemoveListeners()
        {
            startGameButton.onClick.RemoveListener(RequestGame);
            CardDistributionState.OnRoundDistributed -= AnimateOnNewRound;
            CardAnimator.OnPisti -= AnimateOnPisti;
        }
    }
}
