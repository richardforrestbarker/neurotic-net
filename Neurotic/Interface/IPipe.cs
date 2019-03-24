using System;

namespace Neurotic
{
    public class IPipe
    {


        protected float val = 0;
        protected Tuple<float, float> weightBias = new Tuple<float, float>(0,0);
        public void SetValue(float value) => val = value;
        public float GetValue() => val;
        public void SetWeightBias(Tuple<float, float> wb) => weightBias = wb;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 31;
        }

        public override string ToString()
        {
            return string.Format($"Pipe -- Val:{val} WB:{weightBias.ToString()}");
        }
    }
}
