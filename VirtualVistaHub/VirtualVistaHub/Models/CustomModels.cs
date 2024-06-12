using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualVistaHub.Models
{
    public class EditPropertyViewModel
    {
        public Property Property { get; set; }
        public PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public int UserId { get; set; }
        public string[] ImagePaths { get; set; }
        public bool Denied { get; set; }
    }

    public class ImagesModel
    {
        public PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public string[] ImagePaths { get; set; }
    }

    public class ViewPropertyModel
    { 
        public Property Property { get; set; }
        public User User { get; set; }

        public PropertyDetailsTemplate PropertyDetails { get; set; }

        public string[] ImagePaths { get; set; }
    }

    public class PropertySearchViewModel
    {
        public IEnumerable<Property> Properties { get; set; }
        public string TypeOfProperty { get; set; }
        public string District { get; set; }
        public string Neighbourhood { get; set; }
        public string TypeOfConstruction { get; set; }
        public string TypeOfSale { get; set; }
        public Dictionary<int, (string TableName, string FirstImagePath)> PropertyDetails { get; set; }
    }

    public class PaginationModel
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }
}