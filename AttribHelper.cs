using System;
using System.Linq;

namespace GNT.Business.Utils
{

    // getting a bit sick of the longwinded way of getting an attribute
    public static class AttribHelper
    {
        public static T GetAttrib<T>(Type t2)
        {

            return (T)t2.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
