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
using System.IO;
using System.Reflection;
using FilePath = System.IO.Path;



namespace AlchemyGame
{
    //Хино
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    class Element
    {
        string name;
        bool thereIs;
        BitmapImage image;
        StackPanel elementStackPanel;
        public string Name { get { return name; } set { name = value; } }
        public bool ThereIs { get { return thereIs; } set { thereIs = value; } }
        public BitmapImage ImageElement { get { return image; } set { image = value; } }
        public StackPanel ElementStackPanel { get { return elementStackPanel; } set { elementStackPanel = value; } }
        public Element(StreamReader strReader)
        {
            string[] file = strReader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            name = file[0];
            thereIs = Convert.ToBoolean(file[1]);
            string path = FilePath.Combine(FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "ElementsImage/" + file[2]);
            Uri uri = new Uri(path, UriKind.RelativeOrAbsolute);
            ImageElement = new BitmapImage(uri);
            Image img = new Image()
            {
                Source = ImageElement,
                Width = 100,
                Height = 100
            };
            TextBlock txb = new TextBlock()
            {
                FontSize = 16,
                Text = Name,
                TextAlignment = TextAlignment.Center
            };
            StackPanel stk = new StackPanel() { };
            stk.Children.Add(img);
            stk.Children.Add(txb);
            elementStackPanel = stk;
        }

    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            CountsElements();
        }
        private void CountsElements()
        {
            int OpenElements = 0, ClosedElements = 0;
            FileStream fs = new FileStream("elements.txt", FileMode.Open, FileAccess.Read);
            using (StreamReader countReader = new StreamReader(fs))
            {
                while (countReader.Peek() != -1)
                {
                    elem = new Element(countReader);
                    if (elem.ThereIs)
                        OpenElements++;
                    else
                        ClosedElements++;
                }
            }
            CountElements.Text = OpenElements.ToString() + "/" + (OpenElements + ClosedElements).ToString();
        }
        Element elem;
        bool press;
        int zdx = 0;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (allElements.Visibility == Visibility.Hidden)
            {
                FileStream fs = new FileStream("elements.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader strReader = new StreamReader(fs))
                {
                    allElements.Children.Clear();
                    while (strReader.Peek() != -1)
                    {
                        elem = new Element(strReader);
                        if (elem.ThereIs)
                        {
                            elem.ElementStackPanel.MouseDown += ElementMouseDown;
                            allElements.Children.Add(elem.ElementStackPanel);
                        }
                    }
                }
                allElements.Visibility = Visibility.Visible;
                allElements2.Visibility = Visibility.Hidden;
            }
            else
                allElements.Visibility = Visibility.Hidden;

        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (allElements2.Visibility == Visibility.Hidden)
            {
                FileStream fs = new FileStream("elements.txt", FileMode.Open, FileAccess.Read);
                using (StreamReader strReader = new StreamReader(fs))
                {
                    allElements2.Children.Clear();
                    while (strReader.Peek() != -1)
                    {
                        elem = new Element(strReader);
                        allElements2.Children.Add(elem.ElementStackPanel);
                    }
                }
                allElements2.Visibility = Visibility.Visible;
                allElements.Visibility = Visibility.Hidden;
            }
            else
                allElements2.Visibility = Visibility.Hidden;

        }
        private void ElementMouseDown(object sender, MouseButtonEventArgs e)
        {
            StackPanel temp = (StackPanel)sender;
            var parent = (WrapPanel)temp.Parent;
            parent.Children.Remove(temp);
            temp.MouseDown -= ElementMouseDown;
            temp.Margin = new Thickness((int)(field.ActualWidth / 2) - 32, (int)(field.ActualHeight / 2) - 32, 0, 0);
            temp.MouseMove += elementMove;
            temp.MouseLeftButtonDown += ElementLeftButtonDown;
            temp.MouseLeave += ElementLeave;
            temp.MouseLeftButtonUp += ElementLeftButtonUp;
            field.Children.Add(temp);
            allElements.Visibility = Visibility.Hidden;
        }
        private void elementMove(object sender, MouseEventArgs e)
        {
            if (press)
            {
                StackPanel temp = ((StackPanel)sender);
                temp.Margin = new Thickness(e.GetPosition(this).X - 32, e.GetPosition(this).Y - 64, 0, 0);
            }
        }
        private void ElementLeftButtonDown(object sender, MouseEventArgs e)
        {
            press = true;
            Panel.SetZIndex(((StackPanel)sender), ++zdx);
        }
        private void ElementLeave(object sender, MouseEventArgs e)
        {
            press = false;
        }
        private void ElementLeftButtonUp(object sender, MouseEventArgs e)
        {
            press = false;
            //StackPanel temp = (StackPanel)sender;
            var parent = (Canvas)((StackPanel)sender).Parent;
            foreach (var item in parent.Children)
            {
                StackPanel temp = (StackPanel)item;
                if (temp != (StackPanel)sender &&
                    (temp.Margin.Left >= ((StackPanel)sender).Margin.Left - 32 && temp.Margin.Left <= ((StackPanel)sender).Margin.Left + 32) &&
                    temp.Margin.Top >= ((StackPanel)sender).Margin.Top - 32 && temp.Margin.Top <= ((StackPanel)sender).Margin.Top + 32)
                {
                    string option1 = exemptionTextBlock(temp).Text + " + " + exemptionTextBlock((StackPanel)sender).Text;
                    string option2 = exemptionTextBlock((StackPanel)sender).Text + " + " + exemptionTextBlock(temp).Text;
                    FileStream fs = new FileStream("recipes.txt", FileMode.Open, FileAccess.Read);
                    using (StreamReader strReader = new StreamReader(fs))
                    {
                        while (strReader.Peek() != -1)
                        {
                            string recipes = strReader.ReadLine();
                            if (recipes.IndexOf(option1) > -1 || recipes.IndexOf(option2) > -1)
                            {
                                recipes = recipes.Substring(0, recipes.IndexOf('=') - 1);
                                FileStream fs2 = new FileStream("elements.txt", FileMode.Open, FileAccess.Read);
                                using (StreamReader strReader2 = new StreamReader(fs2))
                                {
                                    while (strReader2.Peek() != -1)
                                    {
                                        elem = new Element(strReader2);
                                        if (elem.Name == recipes)
                                        {
                                            elem.ElementStackPanel.Margin = new Thickness(((StackPanel)sender).Margin.Left, ((StackPanel)sender).Margin.Top, 0, 0);
                                            elem.ElementStackPanel.MouseMove += elementMove;
                                            elem.ElementStackPanel.MouseLeftButtonDown += ElementLeftButtonDown;
                                            temp.MouseLeave += ElementLeave;
                                            elem.ElementStackPanel.MouseLeftButtonUp += ElementLeftButtonUp;
                                            field.Children.Add(elem.ElementStackPanel);
                                            string temp_str = elem.Name;
                                            break;
                                        }
                                    }
                                }
                                parent.Children.Remove(temp);
                                parent.Children.Remove((StackPanel)sender);
                                string el = elem.Name;
                                elementAppearance(elem.Name);
                                CountsElements();
                                break;
                            }
                        }
                    }
                    break;
                }

            }

        }
        private TextBlock exemptionTextBlock(StackPanel stackPanel)
        {
            TextBlock tb = new TextBlock();
            foreach (var item in stackPanel.Children)
                if (item.GetType() == (new TextBlock()).GetType())
                    tb = (TextBlock)item;
            return tb;
        }
        private void elementAppearance(string name)
        {
            string str = File.ReadAllText("elements.txt", Encoding.UTF8);
            if (str.IndexOf(name + " false") > -1)
                str = str.Remove(str.IndexOf(name + " false"), (name + " false").Length).Insert(str.IndexOf(name + " false"), name + " true");
            using (StreamWriter file = new StreamWriter("elements.txt"))
                file.Write(str);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            field.Children.Clear();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dlrslt = MessageBox.Show("Вы уверены, что хотите начать новую игру?\n\nВесь прогресс будет сброшен!\n", "Новая игра", MessageBoxButton.YesNo);
            if (dlrslt == MessageBoxResult.Yes)
            {
                string path = FilePath.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                File.Delete(path + "\\elements.txt");
                File.Copy(path + "\\ng\\elements.txt", path + "\\elements.txt");
                field.Children.Clear();
                CountsElements();
            }
        }
    }
}
