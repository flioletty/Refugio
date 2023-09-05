using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBModels;

namespace DBCore
{
    public interface IGroupRepository
    {
        void AddGroup(Group gr, User user);

        void ClearConnection();

        List<string?> GetAllActivity();
    }
}
