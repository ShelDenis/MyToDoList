using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyToDoList.Models;
using MyToDoList.DB;
using Microsoft.EntityFrameworkCore;

namespace MyToDoList.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _db;

        public TaskService(AppDbContext db)
        {
            _db = db;
        }

        public List<Task> GetAllTasks()
        {
            var tasks = _db.Tasks.ToList();
            return tasks;
        }

        public List<Task> GetTasksByGroup(int groupId)
        {
            return _db.Tasks
                .Where(t => t.GroupId == groupId)  
                .OrderByDescending(t => t.CreatedAt) 
                .ToList();
        }

        public void AddTask(Task task)
        {
            try
            {
                _db.Tasks.Add(task);

                int result = _db.SaveChanges();
            }
            catch (Exception ex)
            {
          
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
                throw;
            }
        }

        public void MarkAsDone(int taskId)
        {
            var task = _db.Tasks.Find(taskId);
            if (task != null)
            {
                task.IsCompleted = true;
                _db.SaveChanges();
            }
        }

        public void UnMarkAsDone(int taskId)
        {
            var task = _db.Tasks.Find(taskId);
            if (task != null)
            {
                task.IsCompleted = false;
                _db.SaveChanges();
            }
        }

        public void DeleteTask(Task task)
        {
            try
            {

                var existingTask = _db.Tasks.Find(task.Id);
                if (existingTask != null)
                {
                    _db.Tasks.Remove(existingTask);
                    int result = _db.SaveChanges();
                }
                else
                {
                    Console.WriteLine($"Задача {task.Id} не найдена в БД");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
