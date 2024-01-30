using Microsoft.AspNetCore.Mvc;
using BasicManagementBoard.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using Microsoft.CodeAnalysis;

namespace BasicManagementBoard.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly MySqlDataSource _db;
        private readonly TaskContext _context;

        public TaskController(MySqlDataSource db, TaskContext context)
        {
            _db = db;
            _context = context;
        }

        [HttpGet]
        public async Task<IReadOnlyList<TaskItem>> GetTaskItems()
        {
            using var connection = await _db.OpenConnectionAsync();
            var query = "SELECT * FROM `TASK` ORDER BY `TASKID` DESC LIMIT 10;";
            return (IReadOnlyList<TaskItem>)await connection.QueryAsync<TaskItem>(query);
        }

        [HttpGet("{id}")]
        public async Task<TaskItem?> GetTaskItem(int id)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Using Dapper's QueryFirstOrDefaultAsync to retrieve a single row
            return await connection.QueryFirstOrDefaultAsync<TaskItem>(
                @"SELECT * FROM `TASK` WHERE `TASKID` = @Id",
                new { id }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Use Dapper's ExecuteAsync to perform the update
            var affectedRows = await connection.ExecuteAsync(
                @"UPDATE `TASK` SET `PROJECTID` = @projectId, `DESCRIPTION` = @Description, `PROGRESS` = @Progress, `STATUS` = @Status, `STARTDATE` = @StartDate, `FINISHDATE` = @FinishDate  WHERE `TASKID` = @Id;",
                new { projectId = taskItem.projectId, description = taskItem.description, progress = taskItem.progress, status = taskItem.status, startDate = taskItem.startDate, finishDate = taskItem.finishDate }
            );

            if (affectedRows > 0)
            {
                return Ok(); // Return 200 OK on success
            }

            return NotFound(); // Return 404 Not Found if the task with the specified id is not found
        }

        [HttpPost]
        public async Task<IActionResult> PostTaskItem(TaskItem taskItem)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Using ExecuteAsync to perform the insertion
            await connection.ExecuteAsync(
                @"INSERT INTO `TASK` (`TASKID`, `PROJECTID`, `DESCRIPTION`, `PROGRESS`, `STATUS`, `STARTDATE`, `FINISHDATE`) VALUES (@Id, @ProjectId, @Description, @Progress, @Status, @StartDate, @FinishDate)",
                new { Id = taskItem.Id, projectId = taskItem.projectId, description = taskItem.description, progress = taskItem.progress, status = taskItem.status, startDate = taskItem.startDate, finishDate = taskItem.finishDate }
            );

            if ((taskItem.progress == 0 && taskItem.status == "TODO")
                || (taskItem.progress == 100 && taskItem.status == "COMPLETED")
                || ((taskItem.progress < 100 || taskItem.progress > 0) && (taskItem.status == "IN-REVIEW" || taskItem.status == "PROGRESS")))
            {
                return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
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
            var query = "DELETE FROM `TASK` WHERE `TASKID` = @Id;";
            var affectedRows = await connection.ExecuteAsync(query, new { Id = id });

            if (affectedRows > 0)
            {
                return NoContent(); // Return 204 No Content on success
            }

            return NotFound(); // Return 404 Not Found if the task with the specified id is not found
        }
    }
}
