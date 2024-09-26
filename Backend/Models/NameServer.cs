using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class NameServer
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int NameServerGroupId { get; set; }
        [ForeignKey("NameServerGroupId")]
        public NameServerGroup NameServerGroup { get; set; }
    }
}
