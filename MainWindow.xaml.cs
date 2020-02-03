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
    public class bullets
    {
        public Image player;
        private Grid background;
        bullet[] childrens=new bullet[20];
        Queue<int> empty_queue = new Queue<int>();
        public bullets(Grid back)
        {
            background = back;
            for (int i = 0; i < 20; i++)
            {
                empty_queue.Enqueue(i);
                childrens[i] = new bullet(background);
            }
        }
        public void add(int x,int y,int stepx,int stepy)
        {
            if (empty_queue.Count() >= 1) {
                int i = empty_queue.Dequeue();
                childrens[i].x = x;
                childrens[i].y = y;
                childrens[i].step_x = stepx;
                childrens[i].step_y = stepy;

                Image en = new Image();
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri("/bullet.png", UriKind.Relative);
                bimage.EndInit();
                en.Source = (System.Windows.Media.ImageSource)bimage;
                Thickness mov = player.Margin;
                mov.Left = x;
                mov.Top = y;
                en.Margin = mov;
                en.Width = 30;
                en.Height = 20;
                background.Children.Add(en);
                childrens[i].img = en;
            }
        }
        public void timer_tick()
        {
            int i = 0;
            foreach(bullet b in childrens)
            {
                if (!b.move())
                {
                    childrens[i] = new bullet(background);
                    empty_queue.Enqueue(i);
                }
                i++;
            }
        }
    }
    public class bullet
    {
        public int x, y;
        public int step_x, step_y;
        public Image img;
        private Grid background;
        public bullet(Grid back)
        {
            background = back;
        }
        private bool check_bullet()
        {
            return true;
        }
        public bool move()
        {
            if (img == null)
                return true;
            x += step_x;
            y += step_y;
            if (check_bullet())
            {
                Thickness mov = img.Margin;
                mov.Left = x;
                mov.Top = y;
                img.Margin = mov;
                return true;
            }
            else
            {
                background.Children.Remove(img);
                return false;
             }
                
        }   
    }

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
        private Label hplabel;
        private bullets player_bullets, enemy_bullets;
        private int attack_time=0;
        public player(int _x, int _y, int hp, int Mhp,Image img,Label hpb,bullets playerb) : base(_x, _y, hp, Mhp,img)
        {
            hplabel = hpb;
            player_bullets = playerb;
        }
        public override void move(Image player_1)
        {
            int step = 5;
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
        public override void attack()
        {
            base.attack();
            attack_time = (attack_time + 1) % 50;
            if(attack_time==1)
            {
                player_bullets.add(x,y-20,0,-10);
            }
        }
        public override void timer_Tick(object sender, EventArgs e)
        {
            base.timer_Tick(sender, e);
            player_bullets.timer_tick();
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
        public void debug(object sender, EventArgs e)
        {
            temp.Text = player_1.Margin.ToString();
            Image en = new Image();
            BitmapImage bimage = new BitmapImage();
            bimage.BeginInit();
            bimage.UriSource = new Uri("/plane.png", UriKind.Relative);
            bimage.EndInit();
            en.Source = (System.Windows.Media.ImageSource)bimage;
            Thickness mov = player_1.Margin;
            mov.Top -= 10;
            en.Margin = mov;
            en.Width = 30;
            en.Height = 30;
            background.Children.Add(en);
        }
        public MainWindow()
        {
            System.Windows.Threading.DispatcherTimer timer;
            InitializeComponent();
            bullets player_bullets = new bullets(background);
            player_bullets.player = player_1;
            player_1.Tag = "N";
            player player1 = new player((int)player_1.Margin.Left, (int)player_1.Margin.Top, 10, 10,player_1,HPinfo,player_bullets);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0,0,1);   //间隔1秒
            timer.Tick += new EventHandler(player1.timer_Tick );
            //timer.Tick += new EventHandler(debug);
            timer.Start();

            /*
            Image en = new Image();
            BitmapImage bimage = new BitmapImage();
            bimage.BeginInit();
            bimage.UriSource = new Uri("/plane.png", UriKind.Relative);
            bimage.EndInit();
            en.Source = (System.Windows.Media.ImageSource)bimage;
            Thickness mov = new Thickness();
            mov.Left = 0;
            mov.Top = 0;
            en.Margin = mov;
            en.Width = 30;
            en.Height = 30;
            background.Children.Add(en);
            */


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
        }
    }
}