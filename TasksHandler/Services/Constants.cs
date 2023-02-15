using Microsoft.AspNetCore.Mvc.Rendering;

namespace TasksHandler.Services
{
    public class Constants
    {
        public const string RoleAdmin = "admin";

        public static readonly SelectListItem[] supportedUICultures = new SelectListItem[]
        {
            new SelectListItem{Value = "en", Text = "English"},
            new SelectListItem{Value = "es", Text = "Español"}
        };

    }
}
