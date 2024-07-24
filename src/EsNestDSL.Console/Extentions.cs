using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsNestDSL.Console
{
    public static class Extentions
    {
        public static string ToRawRequest<T>(this IElasticClient self, SearchDescriptor<T> searchDescriptor) where T : class
        {
            using (var output = new MemoryStream())
            {
                self.RequestResponseSerializer.Serialize(searchDescriptor, output);
                output.Position = 0;
                var rawQuery = new StreamReader(output).ReadToEnd();
                return rawQuery;
            }
        }
    }
}
