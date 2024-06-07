using System;

namespace TSG_Library.Attributes
{
    public class UFuncAttribute : Attribute
    {
        public readonly string ufunc_name;

        public UFuncAttribute(string ufunc_name)
        {
            this.ufunc_name = ufunc_name;
        }
    }
}