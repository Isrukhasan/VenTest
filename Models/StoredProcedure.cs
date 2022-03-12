using Microsoft.EntityFrameworkCore;

namespace Venturus.Models
{
    public class StoredProcedure:DbContext
    {
        public int InvitationId { get; set; }
        //public string GetDataSP { get; set; }

    }
}
