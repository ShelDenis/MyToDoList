using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyToDoList.Models
{
    public partial class TaskGroup: ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public string _name = string.Empty;

        [ObservableProperty]
        public DateTime _createdAt = DateTime.Now;

        [ObservableProperty]
        public DateTime _lastChangeAt = DateTime.Now;

        public ICollection<Task> Tasks { get; set; } = new List<Task>();
    }
}
