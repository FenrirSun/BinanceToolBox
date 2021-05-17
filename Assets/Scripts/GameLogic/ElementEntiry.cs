using System;
using GameEvents;
using UnityEngine;
using UnityEngine.UI;

public class ElementEntity : EntityBase
{
    public ElementType elementType;
    // 是否正在被拖动
    public bool isBeingDrag;

    public void Init(LinkUpWorld world)
    {
        
    }
    
    private void Start()
    {

    }

    public void OnTouchStart()
    {
        isBeingDrag = true;
    }

    public void OnTouchEnd()
    {
        
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!isBeingDrag)
            return;
        
        var otherEntity = other.GetComponent<ElementEntity>();
        if (otherEntity)
        {
            if (elementType == otherEntity.elementType)
            {
                OnEliminate(otherEntity);
            }
        }
    }

    // 消除
    private void OnEliminate(ElementEntity other)
    {
       
    }
}