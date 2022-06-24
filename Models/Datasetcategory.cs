using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace UCC_Datasets.Models
{
    public partial class Datasetcategory
    {
        public int IdDataset { get; set; }
        public int IdCategory { get; set; }

        public virtual Category IdCategoryNavigation { get; set; }
        public virtual Dataset IdDatasetNavigation { get; set; }
    }
}
