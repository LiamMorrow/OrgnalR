using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OrgnalR.Core
{
    internal static class Util
    {

        /// <summary>
        /// DANGEROUS! Ignores the completion of this task. Also ignores exceptions.
        /// Credit to StephenCleary.  Don't quite need AsyncEx yet but if we do decide it's necessary we can remove this
        /// https://github.com/StephenCleary/AsyncEx/blob/master/src/Nito.AsyncEx.Tasks/TaskExtensions.cs
        /// </summary>
        /// <param name="this">The task to ignore.</param>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static async void Ignore(this Task @this)
        {
            try
            {
                await @this.ConfigureAwait(false);
            }
            catch
            {
                // ignored
            }
        }


        public static ISet<T> ToSet<T>(this IEnumerable<T> items)
        {
            return new HashSet<T>(items);
        }
    }
}