using System;
using System.Collections.Generic;

namespace Backend.Models
{
    public partial class TagValue
    {
        public int Id { get; set; }
        public string Tagname { get; set; }
        public double Value { get; set; }
    }
}
