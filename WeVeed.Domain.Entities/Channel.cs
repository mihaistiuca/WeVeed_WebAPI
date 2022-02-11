using System.Collections.Generic;

namespace WeVeed.Domain.Entities
{
    public class Channel : EntityBase
    {

        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // ************************************************************************************
        // IF YOU CHANGE SOMETHING HERE, ALSO CHANGE IN WEVEED INTEGRATOR VIDEO.CS, OR THE UPLOAD OF A VIDEO WILL FAIL 

        public string Name { get; set; }

        public List<string> Videos { get; set; } = new List<string>();
    }
}
