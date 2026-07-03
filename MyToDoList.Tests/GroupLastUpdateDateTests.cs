using Microsoft.EntityFrameworkCore;
using Moq;
using MyToDoList.DB;
using MyToDoList.Models;
using MyToDoList.Services;
using MyToDoList.ViewModels;
using Xunit;

namespace MyToDoList.Tests
{
    public class GroupLastUpdateDateTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var db = new AppDbContext(options);
            db.Database.OpenConnection();
            db.Database.EnsureCreated();
            return db;
        }

        [Fact]
        public void AddTask_LastUpdateDateIsCurrent()
        {
            using var db = CreateInMemoryDb();
            var group = new TaskGroup
            {
                Name = "Test Group",
                CreatedAt = DateTime.Parse("01.07.2026 00:00"),
                LastChangeAt = DateTime.Parse("01.07.2026 12:00")
            };
            db.TaskGroups.Add(group);
            db.SaveChanges();

            var vm = new MainViewModel(db);
            vm.CurrentGroup = group;
            vm.NewItemContent = "Test Task";

            var timeBefore = DateTime.Now;
            vm.AddItemCommand.Execute(null);
            var timeAfter = DateTime.Now;

            Assert.InRange(vm.CurrentGroup!.LastChangeAt, timeBefore, timeAfter);
        }

        [Fact]
        public void RenameTask_LastUpdateDateIsCurrent()
        {
            using var db = CreateInMemoryDb();
            var group = new TaskGroup
            {
                Name = "Test Group",
                CreatedAt = DateTime.Parse("01.07.2026 00:00"),
                LastChangeAt = DateTime.Parse("01.07.2026 12:00")
            };
            db.TaskGroups.Add(group);
            db.SaveChanges();

            var task = new Models.Task
            {
                Content = "Task",
                GroupId=group.Id
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            var vm = new MainViewModel(db);
            vm.CurrentGroup = group;
            vm.NewTaskName = "New Task Name";
            vm.TaskIdToRename = 0;

            var timeBefore = DateTime.Now;
            vm.EditTaskCommand.Execute(task.Id);
            var timeAfter = DateTime.Now;

            Assert.InRange(vm.CurrentGroup!.LastChangeAt, timeBefore, timeAfter);
        }

        [Fact]
        public void RemoveTask_LastUpdateDateIsCurrent()
        {
            using var db = CreateInMemoryDb();
            var group = new TaskGroup
            {
                Name = "Test Group",
                CreatedAt = DateTime.Parse("01.07.2026 00:00"),
                LastChangeAt = DateTime.Parse("01.07.2026 12:00")
            };
            db.TaskGroups.Add(group);
            db.SaveChanges();

            var task = new Models.Task
            {
                Content = "Task",
                GroupId = group.Id
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            var vm = new MainViewModel(db);
            vm.CurrentGroup = group;
            vm.TaskIdToRename = 0;

            var timeBefore = DateTime.Now;
            vm.RemoveItemCommand.Execute(task);
            var timeAfter = DateTime.Now;

            Assert.InRange(vm.CurrentGroup!.LastChangeAt, timeBefore, timeAfter);
        }

        [Fact]
        public void ToggleTask_LastUpdateDateIsCurrent()
        {
            using var db = CreateInMemoryDb();
            var group = new TaskGroup
            {
                Name = "Test Group",
                CreatedAt = DateTime.Parse("01.07.2026 00:00"),
                LastChangeAt = DateTime.Parse("01.07.2026 12:00")
            };
            db.TaskGroups.Add(group);
            db.SaveChanges();

            var task = new Models.Task
            {
                Content = "Task",
                GroupId = group.Id
            };

            db.Tasks.Add(task);
            db.SaveChanges();

            var vm = new MainViewModel(db);
            vm.CurrentGroup = group;
            vm.TaskIdToRename = 0;

            var timeBefore = DateTime.Now;
            vm.ToggleTaskCommand.Execute(task);
            var timeAfter = DateTime.Now;

            Assert.InRange(vm.CurrentGroup!.LastChangeAt, timeBefore, timeAfter);
        }

        [Fact]
        public void RenameGroup_LastUpdateDateIsCurrent()
        {
            using var db = CreateInMemoryDb();
            var group = new TaskGroup
            {
                Name = "Test Group",
                CreatedAt = DateTime.Parse("01.07.2026 00:00"),
                LastChangeAt = DateTime.Parse("01.07.2026 12:00")
            };
            db.TaskGroups.Add(group);
            db.SaveChanges();

            var vm = new MainViewModel(db);
            vm.CurrentGroup = group;
            vm.NewGroupName = "New Group Name";

            var timeBefore = DateTime.Now;
            vm.EditGroupCommand.Execute(null);
            var timeAfter = DateTime.Now;

            Assert.InRange(vm.CurrentGroup!.LastChangeAt, timeBefore, timeAfter);
        }
    }
}
