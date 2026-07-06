using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using MyToDoList.DB;
using MyToDoList.Models;
using MyToDoList.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace MyToDoList.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly AppDbContext _db;
        private readonly TaskService _taskService;
        private readonly TaskGroupService _groupService;

        public MainViewModel() : this(new AppDbContext()) { }

        public MainViewModel(AppDbContext db)
        {
            _db = db;
            _taskService = new TaskService(db);
            _groupService = new TaskGroupService(db);
            LoadGroups();
        }

        public ObservableCollection<Task> Tasks { get; } = new();
        public ObservableCollection<TaskGroup> Groups { get; } = new();

        public ObservableCollection<TaskGroup> SearchedGroups { get; } = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddItemCommand))]
        private string? _newItemContent;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddGroupCommand))]
        private string? _newGroupContent;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditGroupCommand))]
        private string? _newGroupName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditTaskCommand))]
        private string? _newTaskName;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
        private string? _searchVal;

        [ObservableProperty]
        private TaskGroup? _currentGroup = null;

        [ObservableProperty]
        private bool _isShowingTasks = false;

        [ObservableProperty]
        private bool _isShowingGroups = true;

        [ObservableProperty]
        private bool _isEnteringGroupName = false;

        [ObservableProperty]
        private bool _isEnteringTaskName = false;

        [ObservableProperty]
        private bool _isRenamingGroup = false;

        [ObservableProperty]
        private bool _isRenamingTask = false;

        [ObservableProperty]
        private bool _isSearching = false;

        [ObservableProperty]
        private int _taskIdToRename;

        private void LoadGroups()
        {

            Groups.Clear();
            foreach (var group in _groupService.GetAllTaskGroups())
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
            else
                NewGroupName = CurrentGroup.Name;

            Tasks.Clear();
            var tasks = _taskService.GetTasksByGroup(CurrentGroup.Id);  

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
                IsSearching = false;
                ClearSearchField();
            }
        }

        partial void OnSearchValChanged(string? value)
        {
            if (!IsShowingGroups) return;
            Search(value ?? string.Empty);
            IsSearching = true;
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

            _taskService.AddTask(newTask);

            Tasks.Add(newTask);
            NewItemContent = null;
            IsEnteringTaskName = false;
            UpdateGroupDate();
        }

        private bool CanAddTask() => !string.IsNullOrWhiteSpace(NewItemContent) && CurrentGroup != null;

        [RelayCommand(CanExecute = nameof(CanAddGroup))]
        private void AddGroup()
        {
            var newGroup = new TaskGroup
            {
                Name = NewGroupContent!.Trim(),
                CreatedAt = DateTime.Now,
                LastChangeAt = DateTime.Now,
            };

            _groupService.AddTaskGroup(newGroup);

            Groups.Add(newGroup);
            NewGroupContent = null;
            CurrentGroup = newGroup;
            IsEnteringGroupName = false;
            IsShowingGroups = false;
            IsEnteringTaskName = false;
        }

        private bool CanAddGroup() => !string.IsNullOrWhiteSpace(NewGroupContent);

        [RelayCommand(CanExecute = nameof(CanEditGroup))]
        private void EditGroup()
        {
            _groupService.EditGroup(CurrentGroup!.Id, NewGroupName!);

            foreach (TaskGroup g in Groups)
            {
                if (g.Id == CurrentGroup!.Id)
                    g.Name = NewGroupName!.Trim();
            }

            IsRenamingGroup = false;
            UpdateGroupDate();
        }

        private bool CanEditGroup() => !string.IsNullOrWhiteSpace(NewGroupName);

        [RelayCommand(CanExecute = nameof(CanEditTask))]
        private void EditTask()
        {
            _taskService.EditTask(TaskIdToRename, NewTaskName!);

            foreach (Task t in Tasks)
            {
                if (t.Id == TaskIdToRename)
                    t.Content = NewTaskName!.Trim();
            }

            IsRenamingTask = false;
            UpdateGroupDate();
        }

        private bool CanEditTask() => !string.IsNullOrWhiteSpace(NewTaskName);

        [RelayCommand]
        private void RemoveItem(Task task)
        {
            _taskService.DeleteTask(task);
            Tasks.Remove(task);

            UpdateGroupDate();
        }

        [RelayCommand]
        private void ToggleTask(Task task)
        {
            if (task == null) return;

            if (task.IsCompleted)
                _taskService.UnMarkAsDone(task.Id);
            else
                _taskService.MarkAsDone(task.Id);

            UpdateGroupDate();
        }

        [RelayCommand]
        private async void RemoveGroup(TaskGroup group)
        {
            if (group == null) return;

           
            var box = MessageBoxManager.GetMessageBoxStandard(
                "Delete",
                "Are you sure to remove the group?",
                ButtonEnum.OkCancel);


            var result = await box.ShowAsync();

            if (result != ButtonResult.Ok)
                return;
            _groupService.DeleteTaskGroup(group.Id);

           
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

            IsShowingTasks = false;
            IsShowingGroups = true;
        }

        [RelayCommand]
        private async void Search(string s)
        {
            SearchedGroups.Clear();

            if (string.IsNullOrWhiteSpace(s))
                return;

            foreach (var group in _groupService.SearchTaskGroups(s))
            {
                if (!SearchedGroups.Contains(group))
                SearchedGroups.Add(group);
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
            IsSearching = false;
            SearchVal = string.Empty;
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

        [RelayCommand]
        private void NewTask()
        {
            if (IsEnteringTaskName) IsEnteringTaskName = false;
            else IsEnteringTaskName = true;
        }

        [RelayCommand]
        private void RenameGroup()
        {
            if (IsRenamingGroup) IsRenamingGroup = false;
            else IsRenamingGroup = true;
        }

        [RelayCommand]
        private void RenameTask(int taskId)
        {
            if (IsRenamingTask) IsRenamingTask = false;
            else IsRenamingTask = true;

            TaskIdToRename = taskId;
            NewTaskName = _taskService.GetTaskById(TaskIdToRename).Content;
        }

        [RelayCommand]
        private void StartSearch()
        {
            IsSearching = !IsSearching;
        }

        [RelayCommand]
        private void ClearSearchField()
        {
            SearchVal = string.Empty;
        }

        public void UpdateGroupDate()
        {
            if (CurrentGroup == null) return;
            _groupService.RefreshLastChanges(CurrentGroup.Id);

            CurrentGroup.LastChangeAt = DateTime.Now;
        }
    }
}
