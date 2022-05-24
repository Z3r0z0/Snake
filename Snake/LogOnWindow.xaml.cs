using Snake.Data;
using Snake.Services;
using System.Windows;

namespace Snake
{
    /// <summary>
    /// Interaction logic for LogOnWindow.xaml
    /// </summary>
    public partial class LogOnWindow : Window
    {
        private readonly LogOnService _logOnService;

        public LogOnWindow(SnakeContext context)
        {
            InitializeComponent();

            _logOnService = LogOnService.GetInstance(context);
        }

        private async void submitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(usernameTextBox.Text) || string.IsNullOrWhiteSpace(usernameTextBox.Text))
                MessageBox.Show("Invalid Username");
            else if (string.IsNullOrEmpty(passwordTextBox.Password) || string.IsNullOrWhiteSpace(passwordTextBox.Password))
                MessageBox.Show("Invalid Password");

            await _logOnService.Login(usernameTextBox.Text, passwordTextBox.Password);

            if (_logOnService.IsLoggedIn)
            {
                MessageBox.Show("Login was successfull");
                this.Close();
            }
            else
            {
                MessageBox.Show("Login was not successfull");
            }
        }
    }
}
