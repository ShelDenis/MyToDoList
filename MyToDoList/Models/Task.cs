using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace MyToDoList.Models
{
    public partial class Task : ObservableObject  
    {
        public int Id { get; set; }

        [ObservableProperty]  
        private string _content = string.Empty;

        [ObservableProperty]
        private bool _isCompleted;

        [ObservableProperty]
        private DateTime _createdAt = DateTime.Now;

        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }

        public TaskGroup? Group { get; set; }
    }
}
