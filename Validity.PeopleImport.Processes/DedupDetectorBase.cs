using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Validity.PeopleImport.Processes
{
    public abstract class DedupDetectorBase
    {
        public string DuplicateJson { get; set; }
        public string NonDuplicateJson { get; set; }

        public abstract  void FindDuplicateAndNonDuplicate(string fileName);
    }
}
 