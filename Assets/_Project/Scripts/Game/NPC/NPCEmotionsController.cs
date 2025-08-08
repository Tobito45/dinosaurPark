using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
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

        private void Start() => _emogiShower.transform.localScale = Vector3.zero;

        private Sprite GetRandomSprite =>
            _emothions[Random.Range(0, _emothions.Count)];

        public void ShowEmogi() => StartCoroutine(ShowEmogiAnimation(GetRandomSprite));

        private IEnumerator ShowEmogiAnimation(Sprite sprite)
        {
            _emogiShower.sprite = sprite;
            _animator.Play(_shower.name);
            yield return new WaitForSeconds(3);
            _animator.Play(_closer.name);
        } 

    }
}
