using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools.ScrollComponent
{
    [Assert]
    [RequireComponent(typeof(RectTransform))]
    public class ScrollComponent : RectComponent
    {
        [SerializeField] private Scrollbar scrollbar;
        [SerializeField] private DragComponent dragComponent;
        [SerializeField] private GridLayoutGroup grid;

        [SerializeField] private float scrollSensitivity = 5;

        private RectComponent GridRectTransform => grid.GetComponent<RectComponent>();

        private Vector2 ItemSize => grid.cellSize;
        private Vector2 Offset => grid.spacing;

        private int CountInLine => Mathf.FloorToInt((GridRectTransform.Size.x + Offset.x) / (ItemSize.x + Offset.x));

        private int LinesCount
        {
            get
            {
                int linesCount = itemsCount / (CountInLine != 0 ? CountInLine : 1);
                if (itemsCount % (CountInLine != 0 ? CountInLine : 1) > 0)
                {
                    linesCount++;
                }

                return linesCount;
            }
        }

        private float MaxVirtualPosition
        {
            get => maxVirtualPosition;
            set
            {
                maxVirtualPosition = value;
                maxVirtualPosition = Mathf.Clamp(maxVirtualPosition, 0, maxVirtualPosition);
            }
        }

        private float VirtualPosition
        {
            get => virtualPosition;
            set
            {
                virtualPosition = value;
                virtualPosition = Mathf.Clamp(virtualPosition, 0, MaxVirtualPosition);
                if (MaxVirtualPosition != 0)
                {
                    scrollbar.value = virtualPosition / MaxVirtualPosition;
                    scrollbar.size = GridRectTransform.Size.y / (MaxVirtualPosition + GridRectTransform.Size.y);
                }
            }
        }

        private int itemsCount;
        private float virtualPosition;
        private float maxVirtualPosition = 0;

        private int CurrentFirstIndex => (int)virtualPosition / (int)(ItemSize.y + Offset.y) * CountInLine;

        private List<ScrollItem> items;

        #region Unity functions

        private void Awake()
        {
            InitDrag();
            InitClick();
            InitSlider();
            InitScroll();
            ForceGrid(); //to simplify calculations
        }

        private void InitScroll()
        {
            dragComponent.OnScrollEvent += (data, f) => { VirtualPosition -= f.y * scrollSensitivity; };
        }

        private void ForceGrid()
        {
            grid.padding = new RectOffset(0, 0, 0, 0);
        }

        #endregion

        private void InitSlider()
        {
            scrollbar.onValueChanged.AddListener(OnValueSliderChanged);
        }

        private void OnValueSliderChanged(float value)
        {
            value = Mathf.Clamp01(value);
            VirtualPosition = MaxVirtualPosition * value;
            Refresh();
        }

        public void InitWith(ScrollItem prefab, int itemsCount)
        {
            this.itemsCount = itemsCount;
            RecalculateMaxVirtualPosition();
            GenerateScrollItemsFromPrefab(prefab);
            VirtualPosition = 0;
            Refresh();

            CheckScrollSlider();
        }

        public void InitWith<T>(HandledScrollItem<T> prefab, IScrollItemHandler<T> handler, int itemsCount)
        {
            this.itemsCount = itemsCount;
            RecalculateMaxVirtualPosition();
            GenerateScrollItemsFromPrefab(prefab, handler);
            VirtualPosition = 0;
            Refresh();

            CheckScrollSlider();
        }

        public void Refresh()
        {
            //get current virtual position
            float firstPos = (int)virtualPosition % (int)(ItemSize.y + Offset.y);

            int counter = 0;
            for (int i = 0; i < items.Count; i++)
            {
                ScrollItem scrollItem = items[i];
                scrollItem.Refresh(i + CurrentFirstIndex);
                counter++;
                if (counter >= CountInLine)
                {
                    counter = 0;
                }
            }

            GridRectTransform.RectTransform.anchoredPosition = new Vector2(GridRectTransform.RectTransform.anchoredPosition.x, firstPos);
        }

        private void RecalculateMaxVirtualPosition()
        {
            MaxVirtualPosition = LinesCount * ItemSize.y + (LinesCount - 1) * Offset.y - Height;
            MaxVirtualPosition = Mathf.Max(0, MaxVirtualPosition);
        }

        private void InitClick()
        {
            dragComponent.OnClickEvent += (data, pos) =>
            {
                int index = GetIndexFromPos(pos);
                if (index >= 0 && index < items.Count)
                {
                    items[index].OnClick(CurrentFirstIndex + index);
                }
            };
        }

        private void InitGrab()
        {
        }

        private int GetIndexFromPos(Vector2 pos)
        {
            int index = 0;

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(dragComponent.RectTransform, pos, Camera.main, out Vector2 point);
            //pos.y = Mathf.Abs(pos.y);

            Vector2 topLeftPos = pos;
            topLeftPos.y = (int)(Screen.height - pos.y - dragComponent.OffsetGlobalTop) + (int)virtualPosition % (int)(ItemSize.y + Offset.y);

            int y = (int)topLeftPos.y / (int)(ItemSize.y + Offset.y);
            int missY = (int)topLeftPos.y % (int)(ItemSize.y + Offset.y);
            if (missY > ItemSize.y)
            {
                return -1;
            }

            int x = (int)topLeftPos.x / (int)(ItemSize.x + Offset.x);
            int missX = (int)topLeftPos.x % (int)(ItemSize.x + Offset.x);
            if (missX > ItemSize.x || x >= CountInLine)
            {
                return -1;
            }

            index = x + y * CountInLine;

            return index;
        }

        private void InitDrag()
        {
            dragComponent.OnDragEvent += (data, f) => { VirtualPosition += data.delta.y; };
        }

        private void GenerateScrollItemsFromPrefab(ScrollItem prefab)
        {
            //minimal integer +1 to cover all visible and +1 to make virtualizing
            int countOfClusters = Mathf.FloorToInt(GridRectTransform.Size.y / (ItemSize.y + Offset.y)) + 2;

            //destroy if have old data
            ClearItems();

            //create needable count of clusters
            for (int i = 0; i < countOfClusters; i++)
            {
                for (int j = 0; j < CountInLine; j++)
                {
                    //make item
                    ScrollItem item = Instantiate(prefab, GridRectTransform.RectTransform);
                    items.Add(item);
                }
            }
        }

        private void GenerateScrollItemsFromPrefab<T>(HandledScrollItem<T> prefab, IScrollItemHandler<T> handler)
        {
            //minimal integer +1 to cover all visible and +1 to make virtualizing
            int countOfClusters = Mathf.FloorToInt(GridRectTransform.Size.y / (ItemSize.y + Offset.y)) + 2;

            //destroy if have old data
            ClearItems();

            //create needable count of clusters
            for (int i = 0; i < countOfClusters; i++)
            {
                for (int j = 0; j < CountInLine; j++)
                {
                    //make item
                    HandledScrollItem<T> item = Instantiate(prefab, GridRectTransform.RectTransform);
                    item.InitWith(handler);
                    items.Add(item);
                }
            }
        }

        private void ClearItems()
        {
            if (items != null && items.Count > 0)
            {
                for (int index = 0; index < items.Count; index++)
                {
                    Destroy(items[index].gameObject);
                }
            }

            items = new List<ScrollItem>();
        }

        private void CheckScrollSlider()
        {
            scrollbar.gameObject.SetActive(MaxVirtualPosition != 0);
        }
    }
}