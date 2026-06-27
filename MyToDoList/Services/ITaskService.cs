using MyToDoList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyToDoList.Services
{
    public interface ITaskService
    {
        List<Task> GetAllTasks();
        List<Task> GetTasksByGroup(int groupId);
        void AddTask(Task task);
        void MarkAsDone(int taskId);

        void UnMarkAsDone(int taskId);
        void DeleteTask(Task task);
    }
}
