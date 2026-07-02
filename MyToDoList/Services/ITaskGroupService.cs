using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyToDoList.Models;

namespace MyToDoList.Services
{
    internal interface ITaskGroupService
    {
        List<TaskGroup> GetAllTaskGroups();
        void AddTaskGroup(TaskGroup taskGroup);
        void DeleteTaskGroup(int taskGrId);

        void EditGroup(int taskGrId, string newName);
    }
}
