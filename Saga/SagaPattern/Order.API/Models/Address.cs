using Microsoft.EntityFrameworkCore;

namespace Order.API.Models
{
    [Owned] //Ayrı bir tabloda olmasını istemediğimiz için Owned olarak işaretledik.
    public class Address
    {
        public string Line { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
}