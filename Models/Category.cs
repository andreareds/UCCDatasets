using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UCC_Datasets.Models
{
    public partial class Category
    {
        public Category()
        {
            Datasetcategory = new HashSet<Datasetcategory>();
        }

        public int IdCategory { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Datasetcategory> Datasetcategory { get; set; }
    }
}
