using Microsoft.Extensions.ObjectPool;
using ObjectPoolPatternExample.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectPoolPatternExample.Policy
{
    public class ExamplPoolPolicy : IPooledObjectPolicy<Example>
    {
        public Example Create()
        {
            return new Example(Guid.NewGuid());
        }

        public bool Return(Example obj)
        {
            return obj != null && obj.Key != null;
        }
    }
}
