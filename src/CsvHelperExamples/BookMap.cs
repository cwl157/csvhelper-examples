using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvHelperExamples
{
    public sealed class BookMap : ClassMap<Book>
    {
        public BookMap()
        {
            AutoMap();
            Map(m => m.Length).Validate(field =>
            {
                int l;
                int.TryParse(field, out l);
                if (l > 0)
                {
                    return true;
                }
                return false;
            });
        }
    }
}
