using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BasicManagementBoard.Models;
using Dapper;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BasicManagementBoard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly MySqlDataSource _db;
        private readonly ProjectContext _context;

        public ProjectController(MySqlDataSource db, ProjectContext context)
        {
            _db = db;
            _context = context;
        }

        [HttpGet]
        public async Task<IReadOnlyList<ProjectItem>> GetProjectItems()
        {
            using var connection = await _db.OpenConnectionAsync();
            var query = "SELECT * FROM `PROJECT` ORDER BY `ID` DESC LIMIT 10;";
            return (IReadOnlyList<ProjectItem>)await connection.QueryAsync<ProjectItem>(query);
        }

        [HttpGet("{id}")]
        public async Task<ProjectItem?> GetProjectItem(int id)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Using QueryFirstOrDefaultAsync to retrieve the desired row
            return await connection.QueryFirstOrDefaultAsync<ProjectItem>(
                @"SELECT * FROM `PROJECT` WHERE `ID` = @Id",
                new { id }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, ProjectItem projectItem)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Using ExecuteAsync to perform the update
            var affectedRows = await connection.ExecuteAsync(
                @"UPDATE `PROJECT` SET `ID` = @Id, `TITLE = @Title`, `DESCRIPTION` = @Description, `NAME` = @Name, `PROGRESS` = @Progress, `STARTDATE` = @StartDate, `FINISHDATE` = @FinishDate  WHERE `ID` = @Id;",
                new { Id = id,
                    Title = projectItem.title,
                    Description = projectItem.description,
                    Name = projectItem.name,
                    Progress = projectItem.progress, 
                    StartDate = projectItem.startDate, 
                    FinishDate = projectItem.finishDate }
            );

            if (affectedRows > 0)
            {
                return Ok(); // Return 200 OK on success
            }

            return NotFound(); // Return 404 Not Found if the task with the specified id is not found
        }

        [HttpPost]
        public async Task<IActionResult> PostTaskItem(ProjectItem projectItem)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Use Dapper's ExecuteAsync to perform the insertion
            await connection.ExecuteAsync(
                @"INSERT INTO `PROJECT` (`ID`, `TITLE`, `DESCRIPTION`, `NAME`, `PROGRESS`, `STARTDATE`, `FINISHDATE`) VALUES (@Id, @Title, @Description, @Name, @Progress, @StartDate, @FinishDate)",
                new { Id = projectItem.Id,
                    Title = projectItem.title,
                    Description = projectItem.description, 
                    Name = projectItem.name, 
                    Progress = projectItem.progress, 
                    StartDate = projectItem.startDate, 
                    FinishDate = projectItem.finishDate }
            );
            if ((projectItem.progress < 100 || projectItem.progress > 0))
            {
                return CreatedAtAction(nameof(GetProjectItem), new { id = projectItem.Id }, projectItem);// Return 201 Created on success, along with the created taskItem
            }
            else
            {
                return BadRequest("Progress should be between 0 and 100");
            }
            
        }

        //Deleting a record by its id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            using var connection = await _db.OpenConnectionAsync();
            var query = "DELETE FROM `PROJECT` WHERE `ID` = @Id;";
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

            if (affectedRows > 0)
            {
                return NoContent(); // Return 204 No Content on success
            }

            return NotFound(); // Return 404 Not Found
        }
    }
}
