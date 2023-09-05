using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBModels
{
    public class User
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? BirthDate { get; set; }

        public string? Activities { get; set; }

        public string? University { get; set; }

        public string? FacultyName { get; set; }

        public string? City { get; set; }

        public string? Country { get; set; }

        public string? Nickname { get; set; }

        public string? Sex { get; set; }

        public long VkId { get; set; }

        public List<Group> Groups { get; set; } = new List<Group>();
    }
}
