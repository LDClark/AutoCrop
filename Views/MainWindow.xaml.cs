using System.Windows;
using System.Windows.Interactivity;

namespace AutoRing_SIB
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Interaction.GetBehaviors(this);
            InitializeComponent();
        }
    }
}
