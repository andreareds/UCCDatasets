using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace UCC_Datasets.Models
{
    public class DatasetCategoryViewModel
    {
        public List<Dataset>? Datasets { get; set; }
        public SelectList? Categories { get; set; }
        public string? DatasetCategory { get; set; }
        public string? SearchString { get; set; }
    }
}
