using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VirtualVistaHub.Models
{
    public class EditPropertyViewModel
    {
        public VirtualVistaHub.Models.Property Property { get; set; }
        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public int UserId { get; set; }
        public string[] ImagePaths { get; set; }
        public bool Denied { get; set; }
    }

    public class ImagesModel
    {
        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }
        public string TableName { get; set; }
        public string[] ImagePaths { get; set; }
    }

    public class ViewPropertyModel
    { 
        public VirtualVistaHub.Models.Property Property { get; set; }
        public VirtualVistaHub.Models.User User { get; set; }

        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }

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

}