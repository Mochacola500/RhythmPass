using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public interface IInfiniteScrollItem
{
    GameObject ItemObject { get; set; }
    Action<IInfiniteScrollItem> OnResult { get; set; }
    int DataIndex { get; set; }

    void SetData<T>(T data, int dataIndex, Action<IInfiniteScrollItem> onResult);
}


public class InfiniteScrollView : MonoBehaviour {
    public delegate void ModifyEvent(IInfiniteScrollItem item, int itemCount);
    public enum Direction {
        Top,
        Bottom
    };
    ModifyEvent itemEvent;
    [Header("ScrollView ")]
    public bool isHorizontal = false;
    [Header("Item settings")]
    public GameObject prefabAsset;
    public Vector2 itemSize;
    public int spacing = 2;
    [Header("Item Option")]
    public bool isLastItemHide = true;
    [Header("Item constraint")]
    public int constraintCount = 0;
    public int itemSpacing = 0;
    [Header("Padding")]
    public int top = 10;
    public int bottom = 10;

    [Header("Pull coefficient")]
    [Range(0.01f, 0.1f)]
    public float pullValue = 0.025f;

    private ScrollRect _scroll;
    public RectTransform _content;
    public RectTransform[] _rects;
    private IInfiniteScrollItem[] _views;
    private int[] _dataIndex;
    private int _previousPosition;
    private int _divideCount;
    private int itemTotalCount;
    public IInfiniteScrollItem[] GetItems { get { return _views; } }
    public int[] GetDataIndexs{ get { return _dataIndex; } }

    void Awake() {
        if(_views == null)
            CreateViews();
    }

    void Update() 
    {
        UpdateScrollView();
    }


    void UpdateScrollView()
    {
        if (itemTotalCount == 0)
            return;
        if (isHorizontal)
            UpdateHorizontal();
        else
            UpdateVertical();
    }

    public void UpdateVerticalForced()
    {
        var count = 0;
        while(count < 20)
        {
            count++;
            float _topPosition = _content.anchoredPosition.y - spacing;
            if (_topPosition < 0f)
                return;
            int position = Mathf.FloorToInt(_topPosition / (itemSize.y + spacing));
            position -= 1;

            if (_previousPosition == position)
                return;

            if (position > _previousPosition)
            {
                if (position - _previousPosition > 1)
                    position = _previousPosition + 1;
                int newPosition = constraintCount * position % _views.Length;
                newPosition -= constraintCount;
                if (newPosition < 0)
                    newPosition = _views.Length - constraintCount;
                int index = constraintCount * position + _views.Length - constraintCount;
                if (index < itemTotalCount)
                {
                    int size = Mathf.CeilToInt((float)index / (float)constraintCount);
                    VerticalItemMove(newPosition, size);
                }
            }
            else
            {
                if (_previousPosition - position > 1)
                    position = _previousPosition - 1;

                int newIndex = constraintCount * position % _views.Length;
                if (newIndex >= 0)
                {
                    VerticalItemMove(newIndex, position);
                }
            }

            _previousPosition = position;
        }

        Debug.Log("low performance. must be optimize used your script or dont use this func");
    }

    void UpdateVertical()
    {
        float _topPosition = _content.anchoredPosition.y - spacing;
        if (_topPosition <= 0f && _rects[0].anchoredPosition.y < -top - 10f)
        {
            InitData(itemTotalCount);
            return;
        }
        if (_topPosition < 0f)
            return;

        int position = Mathf.FloorToInt(_topPosition / (itemSize.y + spacing));
        position -= 1;
        if (_previousPosition == position)
            return;

        if (position > _previousPosition)
        {
            if (position - _previousPosition > 1)
                position = _previousPosition + 1;
            int newPosition = constraintCount * position % _views.Length;
            newPosition -= constraintCount;
            if (newPosition < 0)
                newPosition = _views.Length - constraintCount;
            int index = constraintCount * position + _views.Length - constraintCount;
            if (index < itemTotalCount)
            {
                int size = Mathf.CeilToInt((float)index / (float)constraintCount);
                VerticalItemMove(newPosition, size);
            }
        }
        else
        {
            if (_previousPosition - position > 1)
                position = _previousPosition - 1;
            int newIndex = constraintCount * position % _views.Length;
            if (newIndex >= 0)
            {
                VerticalItemMove(newIndex, position);
            }
        }
        _previousPosition = position;
    }

    void VerticalItemMove(int newIndex, int position)
    {
        int itemCount = constraintCount * position;
        for (int i = 0; i < constraintCount; i++)
        {
            Vector2 pos = _rects[newIndex + i].anchoredPosition;
            pos.y = -(top + position * spacing + position * itemSize.y);
            pos.x = ((itemSize.x * i) + (itemSpacing * i));
            _rects[newIndex + i].anchoredPosition = pos;
            if (itemTotalCount > (itemCount + i))
                itemEvent(_views[newIndex + i], itemCount + i);
            _dataIndex[newIndex + i] = itemCount + i;
            GameObjectSetActive((newIndex + i), (itemCount + i));
        }
    }

    void UpdateHorizontal()
    {
        float _topPosition = -(_content.anchoredPosition.x - spacing);
        if (_topPosition <= 0f && _rects[0].anchoredPosition.x < -(top + 10f))
        {
            InitData(itemTotalCount);
            return;
        }
        if (_topPosition < 0f)
            return;
        int position = Mathf.FloorToInt(_topPosition / (itemSize.x + spacing));
        position -= 1;
        if (_previousPosition == position)
            return;

        if (position > _previousPosition)
        {
            if (position - _previousPosition > 1)
                position = _previousPosition + 1;
            int newPosition = constraintCount * position % _views.Length;
            newPosition -= constraintCount;
            if (newPosition < 0)
                newPosition = _views.Length - constraintCount;
            int index = constraintCount * position + _views.Length - constraintCount;
            if (index < itemTotalCount)
            {
                int size = Mathf.CeilToInt((float)index / (float)constraintCount);
                HorizontalItemMove(newPosition, size);
            }
        }
        else
        {
            if (_previousPosition - position > 1)
                position = _previousPosition - 1;

            int newIndex = constraintCount * position % _views.Length;
            if (newIndex >= 0)
            {
                HorizontalItemMove(newIndex, position);
            }
        }

        _previousPosition = position;
    }

    void HorizontalItemMove(int newIndex, int position)
    {
        int itemCount = constraintCount * position;
        for (int i = 0; i < constraintCount; i++)
        {
            Vector2 pos = _rects[newIndex + i].anchoredPosition;
            pos.x = (top + position * spacing + position * itemSize.x);
            pos.y = -((itemSize.y * i) + (itemSpacing * i));
            _rects[newIndex + i].anchoredPosition = pos;
            if (itemTotalCount > (itemCount + i))
                itemEvent(_views[newIndex + i], (itemCount + i));
            _dataIndex[newIndex + i] = itemCount + i;
            GameObjectSetActive((newIndex + i), (itemCount + i));
        }
    }

    void GameObjectSetActive(int itemIndex, int itemCount)
    {
        if (itemTotalCount <= (itemCount) && isLastItemHide)
            _views[itemIndex].ItemObject.SetActive(false);
        else
            _views[itemIndex].ItemObject.SetActive(true);
    }

    public void InitData(int count, ModifyEvent itemEvent = null) {
        if (itemEvent != null)
            this.itemEvent = itemEvent;

        if (isHorizontal)
            InitDataHorizontal(count, false);
        else
            InitDataVertical(count, false);
    }

    public void InitEvent(ModifyEvent itemEvent = null)
    {
        this.itemEvent = itemEvent;
    }

    void InitDataVertical(int count, bool onlyInfoUpdate)
    {
        if (_views == null)
            CreateViews();

        
        itemTotalCount = count;
        _divideCount = Mathf.CeilToInt((float)count / (float)constraintCount);
        float h = itemSize.y * _divideCount * 1f + top + bottom + (_divideCount == 0 ? 0 : ((_divideCount - 1) * spacing));

        _content.sizeDelta = new Vector2(_content.sizeDelta.x, h);
        if (onlyInfoUpdate) 
            return;

        _previousPosition = 0;
        Vector2 pos = _content.anchoredPosition;
        pos.y = 0f;
        _content.anchoredPosition = pos;
        int y = top;
        int x = 0;
        int total = 0;
        for (int i = 0; i < _views.Length;)
        {
            total = constraintCount + i;
            for (int j = i; j < total; j++)
            {
                _views[j].ItemObject.SetActive(true);
                if (isLastItemHide && j >= count)
                    _views[j].ItemObject.SetActive(!isLastItemHide);
                pos = _rects[j].anchoredPosition;
                pos.y = -y;
                pos.x = x;
                _rects[j].anchoredPosition = pos;
                x += itemSpacing + (int)itemSize.x;
                if(itemTotalCount > j)
                    itemEvent(_views[j], j);
                _dataIndex[j] = j;
                i++;
            }
            y += spacing + (int)itemSize.y;
            x = 0;
        }
    }

    void InitDataHorizontal(int count, bool onlyInfoUpdate)
    {
        if(_views == null)
            CreateViews();
        itemTotalCount = count;
        _divideCount = Mathf.CeilToInt((float)count / (float)constraintCount);
        float v = ((itemSize.x * (_divideCount) + ((_divideCount - 1) * spacing)) + top + bottom);
        _content.sizeDelta = new Vector2(v, _content.sizeDelta.y);

        if (onlyInfoUpdate)
            return;

        _previousPosition = 0;
        Vector2 pos = _content.anchoredPosition;
        pos.x = 0f;
        _content.anchoredPosition = pos;
        int x = top;
        int y = 0;
        int total = 0;
        for (int i = 0; i < _views.Length;)
        {
            total = constraintCount + i;
            for (int j = i; j < total; j++)
            {
                _views[j].ItemObject.SetActive(true);
                if (isLastItemHide && j >= count)
                    _views[j].ItemObject.SetActive(!isLastItemHide);
                pos = _rects[j].anchoredPosition;
                pos.y = y;
                pos.x = x;
                _rects[j].anchoredPosition = pos;
                y -= (int)itemSize.y + itemSpacing;
                if (itemTotalCount > j)
                    itemEvent(_views[j], j);
                _dataIndex[j] = j;
                i++;
            }
            y = 0;
            x += spacing + (int)itemSize.x;
        }
    }

    void CreateViews() {
        if(_scroll == null)
        {
            _scroll = GetComponent<ScrollRect>();
            _content = _scroll.content.GetComponent<RectTransform>();
            isHorizontal = _scroll.horizontal == true ? true : false;
        }

        GameObject clone;
        IInfiniteScrollItem scrollItem;
        int fillCount = 0;
        if (isHorizontal)
            fillCount = (Mathf.RoundToInt((float)_scroll.viewport.rect.width / (itemSize.x + spacing))) + 4;
        else
            fillCount = (Mathf.RoundToInt((float)_scroll.viewport.rect.height / (itemSize.y + spacing))) + 4;

        int count = fillCount * constraintCount;
        _views = new IInfiniteScrollItem[count];
        _dataIndex = new int[count];
        _rects = new RectTransform[count];
        for (int i = 0; i < count; i++) {
            clone = GameObject.Instantiate(prefabAsset, _content) ;
            clone.transform.localScale = Vector3.one;
            clone.transform.localPosition = Vector3.zero;
            scrollItem = clone.GetComponent<IInfiniteScrollItem>();
            scrollItem.ItemObject = clone;
            _views[i] = scrollItem;
            _rects[i] = clone.GetComponent<RectTransform>();
        }
    }

    void itemUpdate(IInfiniteScrollItem item, int index)
    {
        itemEvent?.Invoke(item, index);
    }

    public void ItemInfoUpdate(bool resize = false, int count = 0)
    {
        for (int i = 0; i < _views.Length; i++)
        {
            if (i >= _dataIndex.Length)
                break;
            itemUpdate(_views[i], _dataIndex[i]);
        }

        if(resize)
        {
            if(isHorizontal)
            {
                InitDataHorizontal(count, true);
            }
            else
            {
                InitDataVertical(count, true);
            }
        }
    }
    

    public void ItemMove(int itemindex)
    {
        if (isHorizontal)
            HorizontalMove(itemindex);
        else
            VerticalMove(itemindex);
    }

    public void HorizontalMove(int itemindex)
    {
        _previousPosition = 0;

        Vector2 pos = _content.anchoredPosition;
        pos.x = -(((int)itemSize.x * (itemindex) + spacing * itemindex) + top);
        _content.anchoredPosition = pos;
        pos.x = 0;
        int x = top;
        int y = 0;
        int total = 0;
        for (int i = 0; i < _views.Length;)
        {
            total = constraintCount + i;
            for (int j = i; j < total; j++)
            {
                _views[j].ItemObject.SetActive(true);
                if (isLastItemHide && j >= itemTotalCount)
                    _views[j].ItemObject.SetActive(!isLastItemHide);
                pos = _rects[j].anchoredPosition;
                pos.y = y;
                pos.x = x;
                _rects[j].anchoredPosition = pos;
                y -= (int)itemSize.y + itemSpacing;
                if (itemTotalCount > j)
                    itemEvent(_views[j], j);
                _dataIndex[j] = j;
                i++;
            }
            y = 0;
            x += spacing + (int)itemSize.x;
        }
    }

    public void VerticalMove(int itemindex)
    {
        _previousPosition = 0;

        Vector2 pos = _content.anchoredPosition;
        pos.y = ((int)itemSize.x * (itemindex));
        _content.anchoredPosition = pos;
        int y = top;
        int x = 0;
        int total = 0;
        for (int i = 0; i < _views.Length;)
        {
            total = constraintCount + i;
            for (int j = i; j < total; j++)
            {
                _views[j].ItemObject.SetActive(true);
                if (isLastItemHide && j >= itemTotalCount)
                    _views[j].ItemObject.SetActive(!isLastItemHide);
                pos = _rects[j].anchoredPosition;
                pos.y = -y;
                pos.x = x;
                _rects[j].anchoredPosition = pos;
                x += itemSpacing + (int)itemSize.x;
                if (itemTotalCount > j)
                    itemEvent(_views[j], j);
                _dataIndex[j] = j;
                i++;
            }
            y += spacing + (int)itemSize.y;
            x = 0;
        }
    }

    public int GetItemPosition()
    {
        int index = 0;
        if (isHorizontal)
        {
            float _topPosition = -(_content.anchoredPosition.x - spacing);
            index = Mathf.FloorToInt(_topPosition / (itemSize.x + spacing));
        }
        else
        {
            float _topPosition = _content.anchoredPosition.y - spacing;
            index = Mathf.FloorToInt(_topPosition / (itemSize.y + spacing));
        }
        
        return index;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _scroll.OnDrag(eventData);
    }

    public void OnMove(PointerEventData eventData)
    {
        _scroll.OnScroll(eventData);
    }

    public IInfiniteScrollItem FindItem(Func<IInfiniteScrollItem, bool> checkFunc)
    {
        foreach(var item in _views)
        {
            if (checkFunc(item))
                return item;
        }

        return null;
    }
}
