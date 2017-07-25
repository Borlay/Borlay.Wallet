using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Borlay.Wallet
{
    public static class Extensions
    {
        public static T GetAttribute<T>(this Enum enumValue) where T : Attribute
        {
            T attribute;

            MemberInfo memberInfo = enumValue.GetType().GetMember(enumValue.ToString())
                                            .FirstOrDefault();

            if (memberInfo == null)
                throw new Exception($"Member info for enum '{enumValue}' not found");

            if (memberInfo != null)
            {
                attribute = (T)memberInfo.GetCustomAttributes(typeof(T), false).FirstOrDefault();
                return attribute;
            }
            return null;
        }

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
