using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Sürükleme Başladı!");
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Sürükleniyor!");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Sürükleme Bitti!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Tıklandın!");
    }
}
