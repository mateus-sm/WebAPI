using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Entidades;

namespace WebAPI.Controllers
{
    public class AlunoController
    {
        [Route("[controller]")]
        [ApiController]
        [Authorize(Policy = "APIAuth")]
        public class AlunosController
        {

        }
    }
}
