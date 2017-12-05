/*
 * Author:      #AUTHORNAME#
 * CreateTime:  #CREATETIME#
 * Description:
 * 
*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EZhex1991
{
    public class Trigger : MonoBehaviour
    {
        public bool value;

        public event Action<bool> onValueChanged;

        private HashSet<Collider2D> colliders = new HashSet<Collider2D>();

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Trigger")
            {
                colliders.Add(other);
                SetValue();
            }
        }
        void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Trigger")
            {
                colliders.Remove(other);
                SetValue();
            }
        }

        void SetValue()
        {
            bool hasTrigger = colliders.Count > 0;
            if (value != hasTrigger)
            {
                value = hasTrigger;
                if (onValueChanged != null) onValueChanged(value);
            }
        }
    }
}