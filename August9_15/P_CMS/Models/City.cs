

namespace P_CMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class City
    {  
    
        public int CityId { get; set; }
        public string CityName { get; set; }
    
        public virtual ICollection<Client> Clients { get; set; }
    }
}
