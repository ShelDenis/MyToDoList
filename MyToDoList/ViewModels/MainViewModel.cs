using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyToDoList.DB;
using MyToDoList.Models;
using MyToDoList.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyToDoList.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Task> Tasks { get; } = new();
        public ObservableCollection<TaskGroup> Groups { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private string? _newItemContent;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddGroupCommand))]
        private string? _newGroupContent;

        [ObservableProperty]
        private TaskGroup? _currentGroup = null;

        [ObservableProperty]
        private bool _isShowingTasks = false;

        [ObservableProperty]
        private bool _isEnteringGroupName = false;

        [ObservableProperty]
        private bool _isShowingGroups = true;

        public MainViewModel()
        {
            LoadGroups();
        }

        private void LoadGroups()
        {
            using var db = new AppDbContext();
            var groupService = new TaskGroupService(db);

            Groups.Clear();
            foreach (var group in groupService.GetAllTaskGroups())
            {
                Groups.Add(group);
            }
        }

        private void LoadTasks()
        {
            if (CurrentGroup == null)
            {
                Tasks.Clear();
                return;
            }

            using var db = new AppDbContext();
            var taskService = new TaskService(db);

            Tasks.Clear();
            var tasks = taskService.GetTasksByGroup(CurrentGroup.Id);  

            foreach (var task in tasks)
            {
                Tasks.Add(task);
            }
        }

        partial void OnCurrentGroupChanged(TaskGroup? value)
        {
            if (value != null)
            {
                IsShowingTasks = true;  
                LoadTasks();
                IsShowingGroups = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanAddTask))]
        private void AddItem()
        {
            if (CurrentGroup == null) return;

            var newTask = new Task()
            {
                Content = NewItemContent!,
                IsCompleted = false,
                CreatedAt = DateTime.Now,
                GroupId = CurrentGroup.Id 
            };

            using var db = new AppDbContext();
            var taskService = new TaskService(db);
            taskService.AddTask(newTask);

            Tasks.Add(newTask);
            NewItemContent = null;
        }

        private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewItemContent) && CurrentGroup != null;

        [RelayCommand(CanExecute = nameof(CanAddGroup))]
        private void AddGroup()
        {
            var newGroup = new TaskGroup
            {
                Name = NewGroupContent!.Trim(),
                CreatedAt = DateTime.Now
            };

            using var db = new AppDbContext();
            var groupService = new TaskGroupService(db);
            groupService.AddTaskGroup(newGroup);

            Groups.Add(newGroup);
            NewGroupContent = null;
            CurrentGroup = newGroup;
            IsEnteringGroupName = false;
            IsShowingGroups = false;
        }

        private bool CanAddGroup() => !string.IsNullOrWhiteSpace(NewGroupContent);

        [RelayCommand]
        private void RemoveItem(Task task)
        {
            using var db = new AppDbContext();
            var taskService = new TaskService(db);
            taskService.DeleteTask(task);
            Tasks.Remove(task);
        }

        [RelayCommand]
        private void ToggleTask(Task task)
        {
            if (task == null) return;

            using var db = new AppDbContext();
            var taskService = new TaskService(db);

            if (task.IsCompleted)
                taskService.UnMarkAsDone(task.Id);
            else
                taskService.MarkAsDone(task.Id);

            task.IsCompleted = !task.IsCompleted;
        }

        [RelayCommand]
        private void RemoveGroup(TaskGroup group)
        {
            if (group == null) return;

            using var db = new AppDbContext();
            var groupService = new TaskGroupService(db);
            groupService.DeleteTaskGroup(group.Id);

           
            var tasksToRemove = Tasks.Where(t => t.GroupId == group.Id).ToList();
            foreach (var task in tasksToRemove)
            {
                Tasks.Remove(task);
            }

            Groups.Remove(group);

            if (CurrentGroup == group)
            {
                CurrentGroup = Groups.FirstOrDefault();  
            }
        }

        [RelayCommand]
        private void SelectGroup(TaskGroup group)
        {
            if (group == null) return;

            CurrentGroup = group;
            IsShowingTasks = true;
            LoadTasks();
            IsShowingGroups = false;
        }

        [RelayCommand]
        private void BackToGroups()
        {
            IsShowingTasks = false;
            CurrentGroup = null;
            Tasks.Clear();
            IsShowingGroups = true;
            IsEnteringGroupName = false;
        }

        [RelayCommand]
        private void NewGroup()
        {
            if (IsEnteringGroupName) IsEnteringGroupName = false;
            else IsEnteringGroupName = true;
            IsShowingGroups = false;
        }
    }
}
