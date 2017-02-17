

namespace TaskProcess.MS.Model
{
    using System;

    public class MsValidationAttribute : Attribute
    {
        public string Description { get; set; }
        public int ColumnIndex { get; set; }
    }
}
