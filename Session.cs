using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddingFriends.DB;

namespace WeddingFriends
{
    public static class Session
    {
        public static Users CurrentUser { get; set; }

        public static bool IsAuthenticated => CurrentUser != null;

        public static bool IsAdmin => CurrentUser != null && CurrentUser.RoleID == 1;

        public static bool IsCastingDirector => CurrentUser != null && CurrentUser.RoleID == 3;

        public static bool IsCustomer => CurrentUser != null && CurrentUser.RoleID == 2;

        public static void Logout()
        {
            CurrentUser = null;
        }
    }
}
