using System;

namespace MedStaffConsult.Storage.Abstraction
{
    public abstract class AbstractEntity
    {
        public int UId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
