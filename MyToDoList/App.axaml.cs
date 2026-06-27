using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyToDoList.DB;
using MyToDoList.Models;
using MyToDoList.Services;
using MyToDoList.ViewModels;
using MyToDoList.Views;
using System;
using System.Linq;

namespace MyToDoList
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                using (var db = new AppDbContext())
                {
                    db.Database.EnsureCreated();

                    if (!db.TaskGroups.Any())
                    {
                        db.TaskGroups.AddRange(
                            new TaskGroup { Name = "Группа 1" },
                            new TaskGroup { Name = "Группа 2" },
                            new TaskGroup { Name = "Группа 3" }
                        );
                        db.SaveChanges();
                    }
                }

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainViewModel()
                };
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string dbPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "MyToDoList",
                "todolist.db"
            );

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath));

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite($"Data Source={dbPath}"));

            services.AddTransient<ITaskService, TaskService>();

            services.AddTransient<MainViewModel>();

            services.AddTransient<MainWindow>();
        }

    }
}