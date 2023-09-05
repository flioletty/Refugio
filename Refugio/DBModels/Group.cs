using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DBModels
{
    public class Group
    {
        public long Id { get; set; }

        public string? Name { get; set; }

        public string? Activity { get; set; }

        public string? Description { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Type { get; set; }

        public string? MembersCount { get; set; }

        public string? Place { get; set; }

        public bool? IsClosed { get; set; }

        public List<User> Users { get; set; }
    }
}
