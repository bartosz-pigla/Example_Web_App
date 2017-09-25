using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example_Web_App_1.Helpers
{
    public class Pagination
    {
        public int Size { get; set; }
        public int Number { get; set; }

        public Pagination(int number, int size)
        {
            Size = size;
            Number = number;
        }

        public Pagination()
        {
        }
    }
}
