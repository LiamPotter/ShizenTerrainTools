using UnityEngine;

namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class IntProperty : RandomIntProperty
    {
        private int value;
        public int Value { get { return value; } }

        public void SetValue(int toValue)
        {
            value = toValue;
        }
  
        public override bool CreateRandomValue()
        {
            if (!Randomized)
                return false;
            value = System.Convert.ToInt32(Random.Range(Min, Max));
            return true;
        }

        public IntProperty(string name)
        {
            SetName(name);
            value = 0;
        }
        public IntProperty(string name, int toValue)
        {
            SetName(name);
            value = toValue;
        }
    }
}
