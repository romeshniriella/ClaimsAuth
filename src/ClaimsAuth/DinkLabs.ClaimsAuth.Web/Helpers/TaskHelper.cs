using System;
using System.Threading.Tasks;

namespace DinkLabs.ClaimsAuth.Web.Helpers
{
    public static class TaskHelper
    {
        public static void RunBg(Func<Task> fn)
        {
            Task.Run(fn).ConfigureAwait(false);
        }

        public static void RunBg(Action fn)
        {
            Task.Run(fn).ConfigureAwait(false);
        }
    }
}