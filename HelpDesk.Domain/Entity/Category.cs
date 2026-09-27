using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Domain.Entity
{
    public class Category :BaseEntity<int>
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        
        public IEnumerable<Ticket> Tickets { get; set; }
    }
}
