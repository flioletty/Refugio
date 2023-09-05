using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCore
{
    public class BaseRepository
    {
        protected readonly VKContext _context;

        public BaseRepository(VKContext context)
        {
            _context = context;
        }
    }
}