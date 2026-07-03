using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyToDoList.Models;
using MyToDoList.DB;
using Microsoft.EntityFrameworkCore;

namespace MyToDoList.Services
{
    public class TaskGroupService : ITaskGroupService
    {
        private readonly AppDbContext _db;

        public TaskGroupService(AppDbContext db)
        {
            _db = db;
        }

        public List<TaskGroup> GetAllTaskGroups()
        {
            return _db.TaskGroups
                .Include(g => g.Tasks)
                .ToList();
        }

        public void AddTaskGroup(TaskGroup group)
        {
            _db.TaskGroups.Add(group);
            _db.SaveChanges();
        }

        public void DeleteTaskGroup(int groupId)
        {
            var tasks = _db.Tasks.Where(t => t.GroupId == groupId).ToList();
            _db.Tasks.RemoveRange(tasks);

            var group = _db.TaskGroups.Find(groupId);
            if (group != null)
            {
                _db.TaskGroups.Remove(group);
                _db.SaveChanges();
            }
        }

        public void EditGroup(int groupId, string newName)
        {
            var group = _db.TaskGroups.Find(groupId);
            if (group != null)
            {
                group.Name = newName;
                _db.SaveChanges();
            }
        }

        public void RefreshLastChanges(int groupId)
        {
            var group = _db.TaskGroups.Find(groupId);
            if (group != null)
            {
                group.LastChangeAt = DateTime.Now;
                _db.SaveChanges();
            }
        }

        public List<TaskGroup> SearchTaskGroups(string s)
        {
            var NecGroups = _db.TaskGroups.Where(g => g.Name.Contains(s)).ToList();
            var tasksByTName = _db.Tasks.Where(t => t.Content.Contains(s)).ToList();
            var groupIdsByTName = tasksByTName.Select(t => t.GroupId).ToList();
            foreach (int ind in groupIdsByTName)
            {
                NecGroups.Add(_db.TaskGroups.Where(g => g.Id == ind).First());
            }

            return NecGroups;
        }
    }
}
