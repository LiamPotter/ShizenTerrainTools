using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Shizen
{
    public class ShizenComponent : MonoBehaviour
    {
        protected string openButtonLabel = "Open";

        public string OpenButtonLabel { get { return openButtonLabel; } set { openButtonLabel = value; } }

        public virtual bool Open()
        {
            Debug.Log("Base Shizen Component was Opened!");
            return true;
        }

    }
}