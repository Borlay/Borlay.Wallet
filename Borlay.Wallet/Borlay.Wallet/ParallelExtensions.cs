using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet
{
    public static class ParallelExtensions
    {
        public static async Task ParallelAsync<T>(this IEnumerable<T> enumerable, Func<T, Task> action)
        {
            var tasks = new List<Task>();
            foreach (var e in enumerable)
            {
                var task = action(e);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        public static async Task<TResult> ParallelAnyAsync<T, TResult>(this IEnumerable<T> enumerable, Func<T, Task<TResult>> action)
        {
            var tasks = new List<Task<TResult>>();
            foreach (var e in enumerable)
            {
                var task = action(e);
                tasks.Add(task);
            }
            var t = await Task.WhenAny<TResult>(tasks);
            return await t;
        }
    }
}
