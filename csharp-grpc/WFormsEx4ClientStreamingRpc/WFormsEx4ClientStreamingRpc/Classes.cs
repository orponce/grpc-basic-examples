using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WFormsEx4ClientStreamingRpc
{
    //internal class Class1
    //{
    //}
    class Location
    {
        public int latitude { get; set; }
        public int longitude { get; set; }
    }

    class DataFormat
    {
        public Location? location { get; set; }
        public String? name { get; set; }
    }

}
