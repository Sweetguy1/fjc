using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ManyBlockScene.Scripts
{
    public class SlotItem : MonoBehaviour
    {
        public Image imgKind;
        public string sKind;
        public GameObject objMask;
        public Button button;

        public Vector3 v3Origin;
        public float fSizeOrigin;

        public Vector2Int v2iLoc;
        public Vector2Int tmpLoc;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        public void Init(Sprite sprite, Vector3 pos, float size)
        {
            imgKind.sprite = sprite;
            sKind = sprite.name;
            v3Origin = pos;
            transform.localPosition = v3Origin;
            fSizeOrigin = size;
            SetSize(size);
        }

        public void SetGray(bool bSure)
        {
            objMask.SetActive(bSure);
            button.interactable = !bSure;
        }

        public void SetSize(float size)
        {
            transform.SetSize(size, size);
            transform.Find("Root").SetScale3(size / 100f);
            // Debug.Log($"====SetSize:{size / 100f}  root:{transform.Find("Root").localScale}");
        }

        public void Eliminate(Transform parent)
        {
            _transform.DOKill();
            _transform.SetParent(parent, false);
            gameObject.SetActive(false);
        }
        
    }
}