using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace LearnDapper.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SuperController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
            var  heroes = await connection.QueryAsync<SuperHero>("select * from superheroes" );
            return Ok(heroes);
        }


        [HttpGet("heroId")]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
            var heroes = await connection.QueryAsync<SuperHero>("select * from superheroes where id = @Id", new {Id = heroId});
            return Ok(heroes);
        }

        [HttpPost("create-SuperHero")]
        public async Task<ActionResult<List<SuperHero>>> CreateSuperHeroes(SuperHero hero)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
            await connection.ExecuteAsync("insert into superheros (name, firstname, lastname, place) values (@Name, @Firstname, @Lastname, @Place)", hero);
            var response = await connection.QueryAsync<SuperHero>("select * from superheroes");
            return Ok(response);
        }


        [HttpPut("update-SuperHero")]
        public async Task<ActionResult<List<SuperHero>>> UpdateSuperHeroes(SuperHero request, int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
            var user = await connection.QueryFirstAsync<SuperHero>("select * from superheroes where id = @Id", new { Id = heroId });

            var hero = new SuperHero
            {
                Id = string.IsNullOrEmpty(request.Id.ToString()) ? user.Id : request.Id,
                Name = string.IsNullOrEmpty(request.Name.ToString()) ? user.Name : request.Name,
                Firstname = string.IsNullOrEmpty(request.Firstname.ToString()) ? user.Firstname : request.Firstname,
                Lastname = string.IsNullOrEmpty(request.Lastname.ToString()) ? user.Lastname : request.Lastname,
                Place = string.IsNullOrEmpty(request.Place.ToString()) ? user.Place : request.Place
            };
            await connection.ExecuteAsync("Update superheros (name, firstname, lastname, place) values (@Name, @Firstname, @Lastname, @Place)", hero);
            var response = await connection.QueryAsync<SuperHero>("select * from superheroes");
            return Ok(response);
        }



        [HttpDelete("heroId")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new SqlConnection(_configuration.GetConnectionString("SqlConnectionString"));
            var heroes = await connection.QueryAsync<SuperHero>("delete from superheroes where id = @Id", new { Id = heroId });
            return Ok(heroes);
        }
    }
}
