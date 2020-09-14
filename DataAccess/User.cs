using Model.Timeline.Data;
using Microsoft.AspNetCore.Identity;

namespace DataAccess
{
    public class User : IdentityUser
    {
        public virtual TimelineData TimelineData { get; set; }
    }
}
