using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace RedOwl.Engine
{
    public enum CameraSplitOptions
    {
        Rows,
        Columns,
        Special,
    }
    
    [HideMonoScript]
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraSplitManager : MonoBehaviour
    {
        [SerializeField]
        [LabelText("Split Options"), Tooltip("How to split the camera screen space for 2 or 3 local players")]
        private CameraSplitOptions splitOption = CameraSplitOptions.Special;
        
        [SerializeField]
        [LabelText("Lerp Duration"), Tooltip("The amount of time for the cameras to lerp into position")]
        private float lerpDuration = 0.3f;
        
        private Camera _cam;
        private Tweener _tweener;

        [Button]
        private void OnEnable()
        {
            _cam = GetComponent<Camera>();
            _cam.rect = new Rect(0.5f,0.5f,0,0);
            UpdateCameras();
        }

        private void OnDisable()
        {
            UpdateCameras();
        }

        private void OnDestory()
        {
            if (_cam == null) return;
            _tweener?.Kill();
        }

        private void UpdateRect(Rect rect)
        {
            if (_cam == null) return;
            if (Game.IsRunning)
            {
                _cam.rect = rect;
                _tweener?.Kill();
                _tweener = _cam.DORect(rect, lerpDuration);
            }
            else
            {
                _cam.rect = rect;
            }
        }
        
        #region Static
        private class TableEntry : Dictionary<CameraSplitOptions, List<Rect>> {}
        private class TableData : Dictionary<int, TableEntry> {}
        private static readonly List<Rect> One = new List<Rect> {new Rect(0, 0, 1f, 1f)};
        private static readonly List<Rect> Four = new List<Rect>
        {
            new Rect(0, 0.501f, 0.499f, 0.499f), new Rect(0.501f, 0.501f, 0.499f, 0.499f),
            new Rect(0, 0, 0.499f, 0.499f), new Rect(0.501f, 0, 0.499f, 0.499f)
        };
        private static TableData _table = new TableData
            {
                {
                    1,
                    new TableEntry
                    {
                        {
                            CameraSplitOptions.Rows,
                            One
                        },
                        {
                            CameraSplitOptions.Columns,
                            One
                        },
                        {
                            CameraSplitOptions.Special,
                            One
                        }
                    }
                },
                {
                    2,
                    new TableEntry
                    {
                        {
                            CameraSplitOptions.Rows,
                            new List<Rect> {new Rect(0, 0.501f, 1, 0.499f), new Rect(0, 0f, 1, 0.499f)}
                        },
                        {
                            CameraSplitOptions.Columns,
                            new List<Rect> {new Rect(0, 0, 0.499f, 1), new Rect(0.501f, 0f, 0.499f, 1)}
                        },
                        {
                            CameraSplitOptions.Special,
                            new List<Rect> {new Rect(0, 0.501f, 1, 0.499f), new Rect(0, 0f, 1, 0.499f)}
                        }
                    }
                },
                {
                    3,
                    new TableEntry
                    {
                        {
                            CameraSplitOptions.Rows,
                            new List<Rect> {new Rect(0, 0.668f, 1, 0.331f), new Rect(0, 0.334f, 1, 0.332f),  new Rect(0, 0, 1, 0.332f)}
                        },
                        {
                            CameraSplitOptions.Columns,
                            new List<Rect> {new Rect(0, 0, 0.332f, 1), new Rect(0.334f, 0, 0.332f, 1), new Rect(0.668f, 0, 0.331f, 1)}
                        },
                        {
                            CameraSplitOptions.Special,
                            new List<Rect> {new Rect(0, 0.501f, 0.499f, 0.499f), new Rect(0.501f, 0.501f, 0.499f, 0.499f), new Rect(0.25f, 0, 0.499f, 0.499f)}
                        }
                    }
                },
                {
                    4,
                    new TableEntry
                    {
                        {
                            CameraSplitOptions.Rows,
                            Four
                        },
                        {
                            CameraSplitOptions.Columns,
                            Four
                        },
                        {
                            CameraSplitOptions.Special,
                            Four
                        }
                    }
                }
            };

        [Button]
        private static void UpdateCameras()
        {
            var all = FindObjectsOfType<CameraSplitManager>();
            Array.Sort(all, (x, y) => string.Compare(x.gameObject.name, y.gameObject.name, StringComparison.Ordinal));
            int count = all.Length;
            if (count < 1 || count > 4) return;
            var data = _table[count];
            for (int i = 0; i < count; i++)
            {
                var item = all[i];
                item.UpdateRect(data[item.splitOption][i]);
            }
        }
        
        #endregion
    }
}