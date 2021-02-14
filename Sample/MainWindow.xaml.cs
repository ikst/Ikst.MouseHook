using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ikst.MouseHook;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly MouseHook mh = new MouseHook();

        public MainWindow()
        {
            InitializeComponent();

            mh.LeftButtonDown += (st) =>
            {
                this.Title = $"X:{st.pt.x} Y:{st.pt.y}";
            };

            mh.Start();
        }
    }
}
