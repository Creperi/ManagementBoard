using Microsoft.AspNetCore.Mvc;
using BasicManagementBoard.Models;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;

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
                @"SELECT * FROM `TASK` WHERE `TASKID` = @id",
                new { id }
            );
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTaskItem(int id, TaskItem taskItem)
        {
            using var connection = await _db.OpenConnectionAsync();

            // Use Dapper's ExecuteAsync to perform the update
            var affectedRows = await connection.ExecuteAsync(
                @"UPDATE `TASK` SET `PROJECTID` = @ProjectId, `DESCRIPTION` = @Description, `PROGRESS` = @Progress, `STATUS` = @Status, `STARTDATE` = @StartDate, `FINISHDATE` = @FinishDate  WHERE `TASKID` = @Id;",
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

            // Use Dapper's ExecuteAsync to perform the insertion
            await connection.ExecuteAsync(
                @"INSERT INTO `TASK` (`PROJECTID`, `DESCRIPTION`, `PROGRESS`, `STATUS`, `STARTDATE`, `FINISHDATE`) VALUES (@ProjectId, @Description, @Progress, @Status, @StartDate, @FinishDate)",
                new { projectId = taskItem.projectId, description = taskItem.description, progress = taskItem.progress, status = taskItem.status, startDate = taskItem.startDate, finishDate = taskItem.finishDate }
            );

            // Return 201 Created on success, along with the created taskItem
            return CreatedAtAction(nameof(GetTaskItem), new { id = taskItem.Id }, taskItem);
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

        private bool TaskItemExists(long id)
        {
            return _context.TaskItems?.Any(e => e.Id == id) ?? false;
        }
    }
}
