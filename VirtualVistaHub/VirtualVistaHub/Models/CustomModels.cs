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

        public VirtualVistaHub.Models.PropertyDetailsTemplate PropertyDetails { get; set; }

        public string[] ImagePaths { get; set; }
    }
}