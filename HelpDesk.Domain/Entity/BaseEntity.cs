using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelpDesk.Domain.Entity
{
    public class BaseEntity<T>
    {

        public T Id { get; set; }
        public DateTime CreatedAt {  get; set; }= DateTime.Now;

    }
}
