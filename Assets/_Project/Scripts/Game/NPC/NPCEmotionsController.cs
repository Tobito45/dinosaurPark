using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace NPC
{
    public class NPCEmotionsController : MonoBehaviour
    {
        [Header("Visualization")]
        [SerializeField]
        private SpriteRenderer _emogiShower;

        [SerializeField]
        private List<Sprite> _emothions;

        [Header("Animation")]
        [SerializeField]
        private Animation _animator;
        [SerializeField]
        private AnimationClip _shower, _closer;

        internal Action<EmotionType, string> OnMakeEmotion;
        internal Action OnStartWatch;

        private void Start() => _emogiShower.transform.localScale = Vector3.zero;

        private Sprite GetRandomSprite =>
            _emothions[UnityEngine.Random.Range(0, _emothions.Count)];

        public void StartWatching() => OnStartWatch?.Invoke();

        public void ShowEmogi(NPCInfo info, ItemRuntimeInfo item)
        {
            int index = (int)CalculatingScore.GetReactEmotion(info, item);//UnityEngine.Random.Range(0, _emothions.Count);
            ShowEmogiRPC(index);
        }

        private IEnumerator ShowEmogiAnimation(Sprite sprite, int index)
        {
            _emogiShower.sprite = sprite;
            _animator.Play(_shower.name);
            OnMakeEmotion?.Invoke((EmotionType)index, GetComponent<NPCController>().GetNPCInfo().Name);  
            yield return  new WaitForSeconds(3);
            _animator.Play(_closer.name);
        }

        [Rpc(SendTo.Everyone)]
        public void ShowEmogiRPC(int index) => StartCoroutine(ShowEmogiAnimation(_emothions[index], index));

    }

    public enum EmotionType
    {
        Happy = 2,
        Sad = 0,
        Angry = 1,
        MoreHappy = 3,
        MoreAngry = 4,


    }
}
