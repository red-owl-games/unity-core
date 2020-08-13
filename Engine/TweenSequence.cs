using System;
using System.Linq;
using DG.DOTweenEditor;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Core
{
    [HideMonoScript]
    public class TweenSequence : MonoBehaviour
    {
        public enum SequenceTypes
        {
            Restart,
            PingPong,
            Toggle
        }
        [HorizontalGroup("Settings")]
        [ToggleLeft]
        public bool StartOnAwake;

        [HorizontalGroup("Settings", 0.75f), LabelWidth(45)]
        public SequenceTypes type;

        [SerializeReference]
        public TweenData[] tweens;

        private Sequence _sequence;
        private bool _started;

        private void Awake()
        {
            BuildSequence();
            if (StartOnAwake) Play();
        }

        private void BuildSequence()
        {
            _sequence = DOTween.Sequence().Pause().SetAutoKill(false);
            foreach (var tween in tweens)
            {
                tween.ApplyTween(_sequence);
            }

            switch (type)
            {
                case SequenceTypes.Restart:
                    _sequence.SetLoops(-1, LoopType.Restart);
                    break;
                case SequenceTypes.PingPong:
                    _sequence.SetLoops(-1, LoopType.Yoyo);
                    break;
                case SequenceTypes.Toggle:
                    _sequence.SetLoops(-1, LoopType.Yoyo);
                    _sequence.OnStepComplete(() => _sequence.Pause());
                    break;
            }
        }

        [Button(ButtonSizes.Medium), ButtonGroup("Controls"), DisableInEditorMode]
        public void Play() => _sequence.Play();

#if UNITY_EDITOR

        private static bool _isPreviewing;
        [Button(ButtonSizes.Medium), ButtonGroup("Controls"), DisableInPlayMode]
        private void Preview()
        {
            if (_isPreviewing) return;
            _isPreviewing = true;
            int loops = 0;
            BuildSequence();
            if (type == SequenceTypes.Toggle) _sequence.AppendInterval(0.5f);
            DOTweenEditorPreview.PrepareTweenForPreview(_sequence);
            _sequence.OnStepComplete(() =>
            {
                switch (type)
                {
                    case SequenceTypes.Restart:
                        loops = 2;
                        break;
                    case SequenceTypes.PingPong:
                    case SequenceTypes.Toggle:
                        loops += 1;
                        break;
                }

                if (loops > 1)
                {
                    _isPreviewing = false;
                    DOTweenEditorPreview.Stop(true);
                }
            });
            DOTweenEditorPreview.Start();
        }
        #endif
    }
}