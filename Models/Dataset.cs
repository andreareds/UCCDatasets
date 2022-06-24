using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UCC_Datasets.Models
{
    public partial class Dataset
    {
        public Dataset()
        {
            Datasetcategory = new HashSet<Datasetcategory>();
            Date = DateTime.Now;
        }

        public int IdDataset { get; set; }
        public string IdUser { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Available { get; set; }
        public string Url { get; set; }
        public byte[] Filesource { get; set; }
        public string Filepath { get; set; }

        public DateTime? Date { get; set; }
        public DateTime? LastModified { get; set; }

        public virtual Aspnetusers IdUserNavigation { get; set; }
        public virtual ICollection<Datasetcategory> Datasetcategory { get; set; }
    }
}
