using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RodriguezCalvaRualesMAUIUniWay.API;

namespace RodriguezCalvaRualesMAUIUniWay.API
{
    public static class SessionService
    {
        public static Usuario LoggedInUser { get; private set; }

        public static void SetUser(Usuario usuario)
        {
            LoggedInUser = usuario;
        }

        public static void Clear()
        {
            LoggedInUser = null;
        }

        public static int? GetUserId()
        {
            return LoggedInUser?.Id;
        }
    }
}
