using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace StoreExam
{
    public partial class App : Application
    {
        public Data.DataContext dataContext { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            dataContext = new();
        }
    }
}
