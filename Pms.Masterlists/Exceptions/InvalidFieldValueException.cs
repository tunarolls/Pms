using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pms.Masterlists.Exceptions
{
    public class InvalidFieldValueException : Exception
    {
        public InvalidFieldValueException(string field, string value, string eeId, string remarks = "Kaya mo yan.")
            : base($"'{value}' is not a valid value for field {field} found in Employee {eeId}. {remarks}")
        {
            Field = field;
            Value = value;
            EEId = eeId;
        }

        public string EEId { get; set; }
        public string Field { get; set; }
        public string Value { get; set; }
    }
}
