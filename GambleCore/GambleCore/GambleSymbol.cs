using System;
using GambleCore.Interface;

namespace GambleCore
{
    public class GambleEnumSymbol<T> : ISymbol where T : Enum
    {
        public readonly T Value;
        public GambleEnumSymbol(T value) => Value = value;
        public override string ToString() => Value.ToString();
        public override int GetHashCode() => Value.GetHashCode();

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case GambleEnumSymbol<T> symbol:
                    return Value.Equals(symbol.Value);
                case T t:
                    return Value.Equals(t);
                default:
                    return false;
            }
        }
    }
}