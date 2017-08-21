using System.Linq;

namespace ClassRandom
{
    /// <summary>
    /// Class ProxyHandling.
    /// </summary>
    /// <remarks>Useful if you *really* need to care if it's a proxy or not. Think twice before using</remarks>
    public static class ProxyHandling
    {
        public static bool IsProxy(object x)
        {
            return x.GetType().Name.Contains('_');
        }

        public static string GetTrueClassName(object x)
        {
            if (!IsProxy(x)) return x.GetType().Name;
            var t = x.GetType().BaseType;
            return t != null ? t.Name : x.GetType().Name;
        }

        public static string GetNonProxyNameFromClassName(string c)
        {

            return c.Split('_')[0];
        }
    }
}
