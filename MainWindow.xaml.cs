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
        bullet[] childrens=new bullet[5];
        Queue<int> empty_queue = new Queue<int>();
        public bullets(Grid back)
        {
            background = back;
            for (int i = 0; i < 5; i++)
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
        public plane( int hp, int Mhp,Image img)
        {
            IMG = img;
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            HP = hp;
            MAXHP = Mhp;
            
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
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
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
        int attack_time = 0;
        int move_time = 0;
        int endx, endy;
        private bullets enemy_bullets;
        
        public enemy(int hp,int Mhp,Image img,bullets enemyb):base(hp,Mhp,img)
        {
            enemy_bullets = enemyb;
            endx = -x;
            endy = y+430;
        }
        public override void move(Image img)
        {
            move_time = (move_time + 1) %3;
            if (move_time!=1) {
                Thickness mov = img.Margin;
                mov.Left -= (x - endx) / 25;
                mov.Top -= (y - endy) / 50;
                img.Margin = mov;
            }
            if(System.Math.Abs(x - endx) <50)
            {
                endx = -endx;
                endy = -System.Math.Abs((int)(2 * y));
            }
        }
        public override void be_attack()
        {
            base.be_attack();
        }
        public override void attack()
        {
            base.attack();
            attack_time = (attack_time + 1) % 60;
            if (attack_time == 1)
            {
                enemy_bullets.add(x, y + 40, 0, 10);
            }
        }
        public override void timer_Tick(object sender, EventArgs e)
        {
            base.timer_Tick(sender, e);
        }
    }
    public class player : plane
    {
        private Label hplabel;
        private bullets player_bullets, enemy_bullets;
        private int attack_time=0;
        public player( int hp, int Mhp,Image img,Label hpb,bullets playerb,bullets enemyb) : base( hp, Mhp,img)
        {
            hplabel = hpb;
            player_bullets = playerb;
            enemy_bullets = enemyb;
        }
        public override void move(Image player_1)
        {
            int step = 5;
            foreach (char i in player_1.Tag.ToString())
            {
                if (i == 'L'&&player_1.Margin.Left>-550)
                {
                    Thickness mov = player_1.Margin;
                    mov.Left -= step;
                    player_1.Margin = mov;
                }
                if (i== 'R'&& player_1.Margin.Left < 550)
                {
                    Thickness mov = player_1.Margin;
                    mov.Left += step;
                    player_1.Margin = mov;
                }
                if (i == 'U'&& player_1.Margin.Top > -400)
                {
                    Thickness mov = player_1.Margin;
                    mov.Top -= step;
                    player_1.Margin = mov;
                }
                if (i == 'D' && player_1.Margin.Top < 450)
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
            enemy_bullets.timer_tick();
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
           
        }
        public void backimgmove(object sender, EventArgs e)
        {
            Thickness mov = backimg1.Margin;
            if (mov.Top == 657)
            {
                mov.Top = -2004;
                mov.Bottom = 1328;
                mov.Top += 10;
                mov.Bottom -= 10;
            }
            else
            {
                mov.Top += 1;
                mov.Bottom -= 1;
            }
            backimg1.Margin = mov;
            mov = backimg2.Margin;
            if (mov.Top == 657)
            {
                mov.Top = -2004;
                mov.Bottom = 1328;
                mov.Top += 10;
                mov.Bottom -= 10;
            }
            else
            {
                mov.Top += 1;
                mov.Bottom -= 1;
            }
            backimg2.Margin = mov;
            

        }
        public MainWindow()
        {
            System.Windows.Threading.DispatcherTimer timer;
            InitializeComponent();
            bullets player_bullets = new bullets(background);
            player_bullets.player = player_1;
            player_1.Tag = "N";
            bullets enemy_bullets = new bullets(background);
            player player1 = new player(10,10,player_1,HPinfo,player_bullets,enemy_bullets);
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0,0,1);   //间隔1秒
            timer.Tick += new EventHandler(player1.timer_Tick );
            timer.Tick += new EventHandler(backimgmove);

            Image en = new Image();
            BitmapImage bimage = new BitmapImage();
            bimage.BeginInit();
            bimage.UriSource = new Uri("enemy.png", UriKind.Relative);
            bimage.EndInit();
            en.Source = (System.Windows.Media.ImageSource)bimage;
            Thickness mov = player_1.Margin;
            mov.Left = -400;
            mov.Top = -400;
            en.Margin = mov;
            en.Width = 60;
            en.Height = 60;
            background.Children.Add(en);
            
            enemy enemy1 = new enemy( 3, 3, en, enemy_bullets);
            enemy_bullets.player = en;
            timer.Tick += new EventHandler(enemy1.timer_Tick);
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