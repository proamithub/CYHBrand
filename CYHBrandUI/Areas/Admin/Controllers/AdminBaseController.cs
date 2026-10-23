using CYHBrandRepo;
using CYHBrandUI.Controllers;
using CYHBrandUI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CYHBrandUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public abstract class AdminBaseController : BaseController
    {
        public AdminBaseController(IHttpContextAccessor http, IConfiguration config,
                                   IUnitOfWork uof, IUrlEncryptor enc)
            : base(http, config, uof, enc)
        {
        }
    }
}
