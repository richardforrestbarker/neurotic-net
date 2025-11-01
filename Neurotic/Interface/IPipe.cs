using System;

namespace Neurotic
{
    public class IPipe
    {
        protected double val = 0;
        public void SetValue(double value) => val = value;
        public double GetValue() => val;

        public override bool Equals(object obj)
        {
            return obj != null && obj.GetType().Equals(this.GetType()) && obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() * 31;
        }

        public override string ToString()
        {
            return string.Format($"Pipe -- Val:{val}");
        }
    }
}
