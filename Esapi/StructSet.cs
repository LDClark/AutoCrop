using System;

namespace AutoCrop
{
    public class StructSet
    {
        public string ImageId { get; set; }
        public string StructureSetId { get; set; }
        public DateTime CreationDate { get; set; }
        public string StructureSetIdWithCreationDate { get; set; }
    }
}