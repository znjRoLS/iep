using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iep_newer.Models;
using Microsoft.AspNet.Identity;

public static class Extensions
{
    public static ApplicationUser GetApplicationUser(this System.Security.Principal.IIdentity identity)
    {
        if (identity.IsAuthenticated)
        {
            using (var db = new ApplicationDbContext())
            {
                return db.Users.Find(identity.GetUserId());
            }
        }
        else
        {
            return null;
        }
    }
}