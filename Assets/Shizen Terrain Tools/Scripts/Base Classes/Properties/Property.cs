namespace Shizen.Base.Properties
{
    [System.Serializable]
    public class Property
    {
        private string name;
        public string Name { get { return name; } }
        private bool randomized;
        public bool Randomized { get { return randomized; } }

        protected void SetName(string toName) { name = toName; }
        public void SetRandomized(bool toValue) { randomized = toValue; }
        public virtual bool CreateRandomValue() { return false; }
    }
}
