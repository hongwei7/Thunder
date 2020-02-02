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

namespace Thunder
{
    public class plane
    {
        public int HP, MAXHP;
        public int x, y;
        public Image IMG;
        public plane(int _x, int _y, int hp, int Mhp,Image img)
        {
            x = _x;
            y = _y;
            HP = hp;
            MAXHP = Mhp;
            IMG = img;
        }
        public virtual void move(Image player_1)//移动函数
        {

        }
        public virtual void attack()//攻击函数
        {

        }
        public virtual bool check_plane()//检查是否被攻击
        {
            return false;
        }
        public virtual void be_attack() //被攻击后执行函数
        {
            HP--;
            if (HP == 0)
                IMG.Source=null;
        }
        public virtual void  timer_Tick(object sender, EventArgs e)
        {
            if (check_plane())
                be_attack();
            move(IMG);
            attack();
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
        }
    }
    public class enemy:plane
    {
        enemy(int _x,int _y,int hp,int Mhp,Image img):base(_x,_y,hp,Mhp,img)
        { }
        public override void move(Image img)
        {
            base.move(img);
        }
        public override void be_attack()
        {
            base.be_attack();
        }
        public override void attack()
        {
            base.attack();
        }
    }
    public class player : plane
    {
        Label hplabel;
        public player(int _x, int _y, int hp, int Mhp,Image img,Label hpb) : base(_x, _y, hp, Mhp,img)
        {
            hplabel = hpb;
        }
        public override void move(Image player_1)
        {
            int step = 3;
            foreach (char i in player_1.Tag.ToString())
            {
                if (i == 'L')
                {
                    Thickness mov = player_1.Margin;
                    mov.Left -= step;
                    player_1.Margin = mov;
                }
                if (i== 'R')
                {
                    Thickness mov = player_1.Margin;
                    mov.Left += step;
                    player_1.Margin = mov;
                }
                if (i == 'U')
                {
                    Thickness mov = player_1.Margin;
                    mov.Top -= step;
                    player_1.Margin = mov;
                }
                if (i == 'D')
                {
                    Thickness mov = player_1.Margin;
                    mov.Top += step;
                    player_1.Margin = mov;
                }
            }
        }
         public override void timer_Tick(object sender, EventArgs e)
        {
            base.timer_Tick(sender, e);
            hplabel.Content = "HP:" + HP.ToString() + "/" + MAXHP.ToString();
        }
    
        public override void be_attack()
        {
            base.be_attack();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            System.Windows.Threading.DispatcherTimer timer;
            InitializeComponent();
            player_1.Tag = "N";
            player player1 = new player((int)player_1.Margin.Left, (int)player_1.Margin.Top, 10, 10,player_1,HPinfo);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0,0,1);   //间隔1秒
            timer.Tick += new EventHandler(player1.timer_Tick );
            timer.Start();

        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            player_1.Tag = "";
            if (Keyboard.IsKeyDown(Key.Left))
                player_1.Tag += "L";
            if (Keyboard.IsKeyDown(Key.Right))
                player_1.Tag += "R";
            if (Keyboard.IsKeyDown(Key.Up))
                player_1.Tag += "U";
            if (Keyboard.IsKeyDown(Key.Down))
                player_1.Tag += "D";
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            Window_KeyDown(sender, e);
            if (player_1.Tag.ToString() == "")
                player_1.Tag = "";
        }
    }
}