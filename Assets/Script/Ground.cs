/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 * Description:
 * 
*/
using System;
using UnityEngine;

namespace EZhex1991
{
    public class Ground : MonoBehaviour
    {
        public Trigger trigger;
        public Color triggeredColor;

        public event Action<bool> onTriggerValueChanged;

        private SpriteRenderer spriteRenderer;
        private Color normalColor;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            normalColor = spriteRenderer.color;
            trigger.onValueChanged += OnTriggerValueChanged;
        }

        void OnTriggerValueChanged(bool value)
        {
            spriteRenderer.color = value ? triggeredColor : normalColor;
            if (onTriggerValueChanged != null) onTriggerValueChanged(value);
        }
    }
}