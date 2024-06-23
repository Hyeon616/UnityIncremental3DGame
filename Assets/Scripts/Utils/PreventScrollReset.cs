using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PreventScrollReset : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect scrollRect;
    private bool isDragging = false;
    private Vector2 lastScrollPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        // 드래그 종료 시 마지막 스크롤 위치 저장
        lastScrollPosition = scrollRect.content.anchoredPosition;
    }

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
       
    }

    void Update()
    {
        if (!isDragging)
        {
            // 스크롤 위치를 유지
            scrollRect.content.anchoredPosition = lastScrollPosition;
        }
    }

}
