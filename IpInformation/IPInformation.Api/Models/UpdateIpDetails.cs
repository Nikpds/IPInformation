using System.Collections.Generic;

namespace IPInformation.Api.Models
{
    public class UpdateIpDetails
    {
        public int Total { get; set; }
        public string Id { get; set; }
        public int Completed { get; set; }
        public HashSet<string> Ips { get; set; }

        public UpdateIpDetails()
        {
            Ips = new HashSet<string>();
        }
    }
}
