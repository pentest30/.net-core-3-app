using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Voluntary.App.Data.Entities
{
    public class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }
    }
}
