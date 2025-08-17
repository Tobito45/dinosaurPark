using NPC;
using TMPro;
using Unity.Netcode;
using UnityEngine;



namespace GameUI
{
    public class StatisticsUI : NetworkBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _textCountNegative, _textCountMiddle, _textCountPozitive;
    
        private int _countNegative = 0, _countMiddle = 0, _countPozitive = 0;

        private void Start()
        {
           _textCountNegative.text = _countNegative.ToString();
           _textCountMiddle.text = _countMiddle.ToString();
           _textCountPozitive.text = _countPozitive.ToString();
        }

        public void OnNewEmothion(EmotionType emotion)
        {
            switch (emotion)
            {
                case EmotionType.Happy:
                    _countPozitive++;
                    break;
                case EmotionType.Sad:
                    _countMiddle++;
                    break;
                case EmotionType.Angry:
                    _countNegative++;
                    break;
            }
            UpdateTextsRPC(_countMiddle, _countNegative, _countPozitive);
        }

        [Rpc(SendTo.Everyone)]
        public void UpdateTextsRPC(int countMiddle, int countNegative, int countPozitive)
        {
            _textCountMiddle.text = countMiddle.ToString();
            _textCountNegative.text = countNegative.ToString();
            _textCountPozitive.text = countPozitive.ToString();    
        }
    }
}
