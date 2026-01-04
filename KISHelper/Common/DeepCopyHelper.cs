using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace KISHelper.Common
{
    public static class DeepCopyHelper
    {
        private static readonly JsonSerializerOptions _opt = new() { WriteIndented = false };

        public static T DeepClone<T>(this T obj)
        {
            var json = JsonSerializer.Serialize(obj, _opt);
            return JsonSerializer.Deserialize<T>(json)!;
        }
    }
}
