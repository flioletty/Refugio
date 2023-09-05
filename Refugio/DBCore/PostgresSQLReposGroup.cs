using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBModels;
using Microsoft.EntityFrameworkCore;

namespace DBCore
{
    public class PostgresSQLReposGroup : BaseRepository, IGroupRepository
    {

        public PostgresSQLReposGroup(VKContext context) : base(context) { }

        public void CreateGroup(Group gr, User us)
        {
            gr.Users = new List<User>();
            gr.Users.Add(us);
            _context.Groups.Add(gr);
            _context.SaveChanges();
        }

        public void ClearConnection()
        {
            if (_context.Groups.Count() > 0)
            {
                foreach (var gr in _context.Groups)
                {
                    if (gr.Users != null)
                        gr.Users.Clear();
                }
            }
        }
        public void AddGroup(Group gr, User us)
        {
            Group group = _context.Groups.Include(x => x.Users).FirstOrDefault(s => s.Name == gr.Name);

            if (group == null)
            {
                CreateGroup(gr, us);
            }
            else
            {
                if (_context.Users.FirstOrDefault(x => x.VkId == us.VkId) != null)
                    group.Users.Remove(us);

                group.Users.Add(us);
                UpdateGroup(group, gr);
                _context.Entry(group).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }

        public List<string?> GetAllActivity()
        {
            return _context.Groups.Where(x => x.Activity != null &&
                                         x.Activity.Contains(":") == false &&
                                         x.Activity.Contains("Этот материал заблокирован") == false)
                                  .Select(x => x.Activity).Distinct().ToList();
        }

        public void UpdateGroup(Group OldGr, Group NewGr)
        {
            if(OldGr.IsClosed!=NewGr.IsClosed)
                OldGr.IsClosed = NewGr.IsClosed;
            if(OldGr.Activity!=NewGr.Activity)
                OldGr.Activity = NewGr.Activity;
            if(OldGr.Description!=NewGr.Description)
                OldGr.Description = NewGr.Description;
            if(OldGr.Place!=NewGr.Place)
                OldGr.Place = NewGr.Place;
            if(OldGr.Country!=NewGr.Country)
                OldGr.Country = NewGr.Country;
            if(OldGr.Type!=NewGr.Type)
                OldGr.Type = NewGr.Type;
            if(OldGr.City!=NewGr.City)
                OldGr.City = NewGr.City;
        }
    }
}
