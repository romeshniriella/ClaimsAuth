using System;

namespace DinkLabs.ClaimsAuth.Web.Helpers
{
    public static class StringExtensions
    {
        public static bool IsNotBlank(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }
        public static bool IsBlank(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }


        public static TInner IfNotBlank<TInner>(this string source, Func<string, TInner> selector, bool createNew = false) 
        {
            return !IsNotBlank(source) // if the string is not blank/null/empty
                ? (createNew // if create new flag is set, 
                        ? Activator.CreateInstance<TInner>() // create a new TInner object 
                        : default(TInner))  // else return default value
                : selector(source); // if not blank, fire the selector
        }

    }
}