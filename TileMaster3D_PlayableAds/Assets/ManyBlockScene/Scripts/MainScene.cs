using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace ManyBlockScene.Scripts
{
    public enum GameStatus
    {
        Waiting,
        Play,
        Victory,
        Lose
    }
    public class MainScene : MonoBehaviour
    {
        [SerializeField] private Transform tfSlotLayer;
        [SerializeField] private Sprite[] spIcons;
        [SerializeField] private SlotItem pItemCell;
        [SerializeField] private Transform[] tfSlotPos;
        [SerializeField] private Transform tfContent;
        [SerializeField] private GameObject effPrefab;
        [SerializeField] private UpdateScoreControl chest;
        [SerializeField] private AudioClip[] audioClips;

        private readonly List<SlotItem> _slots = new List<SlotItem>();
        private const int _col = 12;
        private int _row;
        public GameStatus _kStatus;
        private float size;
        private int yCheck;//检测y层是否消完，消完就降一层
        public int comboCount;
        public AudioManager audioManager;
        
        private readonly List<int> _dataA = new List<int>();
        private readonly List<int> _dataMap = new List<int>();
        private void Start()
        {
            _kStatus = GameStatus.Waiting;
            const float ySpace = 0.65f;
            var rect = ((RectTransform) tfSlotLayer).rect;
            size = rect.width / _col;
            _row = Mathf.FloorToInt(rect.height / (size * ySpace) - 1);
            var length = _col * _row;
            var iconLength = spIcons.Length;
            var x = (_col - 1) * size * -0.5f;
            var y = (rect.height - size) * -0.5f;
            CreateData();
            yCheck = 0;
            // Debug.Log($"====start rect:{rect}  size:{size}");

            for (var r = 0; r < _row; r++)
            {
                for (var c = 0; c < _col; c++)
                {
                    var item = GameObject.Instantiate(pItemCell, tfSlotLayer, false);
                    item.button.onClick.AddListener(()=>ClickSlot(item));
                    _slots.Add(item);
                    var i = r * _col + c;
                    var pos = new Vector3(x + c * size, y + r * size * ySpace);
                    
                    item.Init(spIcons[_dataMap[i]], pos, size);
                    item.SetGray(r != 0);
                    item.v2iLoc.x = c;
                    item.v2iLoc.y = r;
                    item.tmpLoc = item.v2iLoc;
                }
            }

            UpdateSiblingIndex();
            
            pItemCell.gameObject.SetActive(false);
            _kStatus = GameStatus.Play;
        }

        private void CreateData()
        {
            for (int i = 0; i < _row; i++)
            {
                var index = Random.Range(0, spIcons.Length);
                _dataA.Add(index);
            }

            int len = _col * _row / 3 - _row * 2;
            var spriteGroup = len / 30;
            for (int i = 0; i < len/spriteGroup; i++)
            {
                var index = Random.Range(0, spIcons.Length);
                for (int y = 0; y < spriteGroup * 3; y++)
                {
                    _dataMap.Add(index);
                }
            }
            _dataMap.SortR();
            _dataMap.SortR();
            _dataMap.SortR();
            for (int i = 0; i < _row ; i++)
            {
                int count = 6;
                while (count > 0)
                {
                    int range = Random.Range(0, 12-count);
                    int index = range + i * _col ;
                    _dataMap.Insert(index,_dataA[i]);
                    count--;
                }
            }
            
        }
        
        private void UpdateSiblingIndex()
        {
            var index = 0;
            
            for (var r = _row - 1; r > -1; r--)
            {
                for (var c = 0; c < _col; c++)
                {
                    var i = r * _col + c;
                    var slot = _slots[i];
                    slot.transform.SetSiblingIndex(index++);
                }
            }
        }

        //===========================================================
        

        private void VictoryCompleteCount(int index)
        {
            var slot1 = _selects[index];
            var slot2 = _selects[index + 1];
            var slot3 = _selects[index + 2];
            _selects.RemoveRange(index,3);
            slot1.transform.DOMove(tfSlotPos[index+1].position, 0.3f).SetDelay(0.15f);
            slot3.transform.DOMove(tfSlotPos[index+1].position, 0.3f).SetDelay(0.15f).OnComplete((() =>
            {
                slot1.Eliminate(tfSlotLayer);
                slot2.Eliminate(tfSlotLayer);
                slot3.Eliminate(tfSlotLayer);
                GameObject eff = Instantiate(effPrefab, tfContent, false);
                eff.transform.position = tfSlotPos[index+1].position - Vector3.forward * 10;
                eff.transform.localScale *= 40;
                Destroy(eff,3f);

                var tmpAudio = audioManager.SearchAudio();
                tmpAudio.clip = audioClips[1];
                tmpAudio.Play();
                chest.CreateProp(tfContent, tfSlotPos[index + 1].position, ++comboCount,audioManager.SearchAudio(),audioClips[3]);
                if (IsEmpty())
                {
                    _kStatus = GameStatus.Victory;
                    EventManager.GetInstance().Dispatcher(EventType.GameWin,new Message("win"));
                }
                PushForward( );
            }));
        }


        public void ClickUrlBtn()
        {
            // Luna.Unity.Playable.InstallFullGame(
            //     "https://apps.apple.com/us/app/id1562663583",
            //     "https://play.google.com/store/apps/details?id=com.higgs.tilemaster3d"
            // );
        }
        //===========================================================

        private readonly List<SlotItem> _selects = new List<SlotItem>();

        private void ClickSlot(SlotItem slot)
        {
            if (_kStatus != GameStatus.Play) return;
            var count = _selects.Count;
            // Debug.Log($"clickSlot:{slot.sKind}");
            if (count > 6) return;
            var lastSlot = _selects.FindLast(a => a.sKind == slot.sKind);
            if (lastSlot == null)
            {
                SetSlotToTable(slot, count);
                return;
            }
            int index = _selects.IndexOf(lastSlot);
            PushBack(index + 1);
            SetSlotToTable(slot, index + 1);
        }

        private void PushBack(int index)
        {
            for (int i = index ; i < _selects.Count; i++)
            {
                _selects[i].transform.DOMove(tfSlotPos[i + 1].position, 0.3f);
            }
        }

        private void SetSlotToTable(SlotItem slot, int index)
        {
            _selects.Insert(index, slot);
            CheckSlotWhite(slot);
            slot.button.interactable = false;
            var tfPos = tfSlotPos[index];
            slot.transform.DOKill();
            slot.SetSize(tfPos.Rect().width);
            slot.transform.SetParent(tfContent, true);
            
            var tmpAudio = audioManager.SearchAudio();
            tmpAudio.clip = audioClips[0];
            tmpAudio.Play();
            
            slot.transform.DOMove(tfPos.position, 0.4f)
                .OnComplete(JudgeTable);
        }
        //消除逻辑
        private void JudgeTable()
        {
            SlotItem lastSlot = null;
            var count = 0;
            for(int i = 0; i<_selects.Count;i++)
            {
                if (lastSlot == null || lastSlot.sKind != _selects[i].sKind)
                {
                    lastSlot = _selects[i];
                    count = 1;
                }
                else if(++count == 3)
                {
                    var index = _selects.IndexOf(lastSlot);
                    VictoryCompleteCount(index);
                    lastSlot = null;
                }
            }

            if (_selects.Count == 7)
            {
                FailureComplete();
            }
        }

        private void PushForward()
        {
            foreach (var slot in _selects)
            {
                var index = _selects.IndexOf(slot);
                slot.transform.DOMove(tfSlotPos[index].position, 0.3f);
            }
        }
        
        private void FailureComplete()
        {
            _kStatus = GameStatus.Waiting;
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(0.1f);
            
            var tmpAudio = audioManager.SearchAudio();
            tmpAudio.clip = audioClips[2];
            tmpAudio.Play();
            
            foreach (var slot in _selects)
            {
                slot.transform.DOKill();
                slot.SetSize(slot.fSizeOrigin);
                slot.transform.SetParent(tfSlotLayer, true);
                slot.button.interactable = true;
                sequence.Join(slot.transform.DOLocalMove(slot.v3Origin, 0.3f)
                    .OnComplete(() =>
                    {
                        CheckSlotGray(slot);
                    }));
            }

            sequence.OnComplete(() =>
            {
                _kStatus = GameStatus.Play;
                UpdateSiblingIndex();
            });
            _selects.Clear();
        }

        private void DropDown()
        {
            bool drop = true;
            for (int i = yCheck * _col; i < yCheck * _col +12 ; i++)
            {
                if (_slots[i].gameObject.activeSelf == true) drop = false;
            }

            if (drop){
                _kStatus = GameStatus.Waiting;
                tfSlotLayer.DOMove(tfSlotLayer.position - Vector3.up * size * 0.65f, 0.5f).SetEase(Ease.InOutBounce).OnComplete((() =>
                {
                    _kStatus = GameStatus.Play;
                    yCheck += 1;
                }));
            }
        }
        
        private void DropDownMap()
        {
            _kStatus = GameStatus.Waiting;
            for (int i = 0; i < _slots.Count; i++)
            {
                var slotRoot = _slots[i];
                DOTween.SetTweensCapacity(500,500);
                Sequence sequence = DOTween.Sequence();
                sequence.AppendInterval(0.1f);
                if (slotRoot.button.interactable == true && slotRoot.tmpLoc.y != 0)
                {
                    int count = 0;
                    int y = slotRoot.tmpLoc.y;
                    while (count <28)
                    {
                        int index = slotRoot.tmpLoc.x + count * _col;
                        if (_slots[index].transform.parent == tfSlotLayer)
                        {
                            sequence.Join(_slots[index].transform
                                .DOMove(
                                    _slots[index].transform.position -
                                    Vector3.up * size * slotRoot.tmpLoc.y * 0.65f,
                                    0.3f).OnComplete(() => _slots[index].tmpLoc.y -= y));
                        }

                        count++;
                    }
                }

                sequence.OnComplete((() => _kStatus = GameStatus.Play));
            }
        }

        private SlotItem GetSlot(int x, int y) => _slots[y * _col + x];
        private void CheckSlotGray(SlotItem slot)
        {
            
            if (slot.v2iLoc.y == _row - 1) return;
            var temp = GetSlot(slot.v2iLoc.x, slot.v2iLoc.y + 1);
            temp.SetGray(true);
        }

        private void CheckSlotWhite(SlotItem slot)
        {
            if (slot.v2iLoc.y == _row - 1) return;
            var temp = GetSlot(slot.v2iLoc.x, slot.v2iLoc.y + 1);
            temp.SetGray(false);
        }

        private bool IsEmpty()
        {
            return tfSlotLayer.childCount == 0;
        }
    }
}