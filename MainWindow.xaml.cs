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
    public class Enemies
    {
        public static int num = 10;
        public Player player;
        Random ran = new Random();
        public Grid background;
        public Enemy[] childrens = new Enemy[num];
        public Bullets[] childrens_bullets = new Bullets[num];
        public Queue<int> empty_queue = new Queue<int>();
        int add_time = 0;
        public Enemies(Grid back,Player _player)
        {
            player = _player;
            background = back;
            for (int i = 0; i < num; i++)
            {
                empty_queue.Enqueue(i);
                childrens_bullets[i] = new Bullets(background, null);
                childrens[i] = new Enemy(2, 2, null, childrens_bullets[i],player,background,i);

            }
        }
        public  void Add()
        {
            if (empty_queue.Count() >= 1)
            {
                int i = empty_queue.Dequeue();
                Image en = new Image();
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri("enemy.png", UriKind.Relative);
                bimage.EndInit();
                en.Source = (System.Windows.Media.ImageSource)bimage;
                Thickness mov = new Thickness(ran.Next(-600,600),ran.Next(-800,-700),0,0);

                en.Margin = mov;
                en.Width = 60;
                en.Height = 60;
                background.Children.Add(en);
                childrens_bullets[i] = new Bullets(background,en);
                childrens[i] = new Enemy(2, 2, en, childrens_bullets[i],player,background,i);
            }
        }
        public void Timer_tick(object sender, EventArgs e)
        {
            add_time = (add_time + 1) % 100;
            if(add_time==1)
                Add();
            for (int i = 0; i < 10; i++)
            {
                if(!empty_queue.Contains(i))
                {
                    childrens[i].Timer_Tick(sender,e);
                }

                childrens_bullets[i].Timer_tick();
            }
        }
    }
    public class Bullets
    {
        public static int num = 3;
        public Image player;
        private Grid background;
        public Bullet[] childrens = new Bullet[num];
        public Queue<int> empty_queue = new Queue<int>();
        public Bullets(Grid back, Image p)
        {
            background = back;
            player = p;
            for (int i = 0; i < num; i++)
            {
                empty_queue.Enqueue(i);
                childrens[i] = new Bullet(background, i, this);
            }
        }
        public void Add(int x, int y, int stepx, int stepy)
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
        public void Timer_tick()
        {
            for (int i = 0; i < num; i++)
            {
                if(!empty_queue.Contains(i))
                {
                    childrens[i].Move();
                }
            }
    
        }
    }
    public class Bullet
    {
        public int tag;
        public int x, y;
        private Bullets father;
        public int step_x, step_y;
        public Image img;
        private Grid background;
        public Bullet(Grid back,int t,Bullets f)
        {
            tag = t;
            father = f;
            background = back;
        }
        public void Destroy()
        {
            background.Children.Remove(img);
            father.empty_queue.Enqueue(tag);
            
        }


        public  void Move()
        {
            if (img == null)
                return ;
            x += step_x;
            y += step_y;
            if (y<800&&y>-800)
            {
                Thickness mov = img.Margin;
                mov.Left = x;
                mov.Top = y;
                img.Margin = mov;
            }
            else
            {
                Destroy();
             }
                
        }


    }

    public class Plane
    {
        public int HP, MAXHP;
        public int x, y;
        public Image IMG;
        public Plane( int hp, int Mhp,Image img)
        {
            IMG = img;
            if (IMG == null)
                return;
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            HP = hp;
            MAXHP = Mhp;
            
        }
        public virtual void Move(Image player_1)//移动函数
        {

        }
        public virtual void Attack()//攻击函数
        {

        }
        public virtual bool Check_plane()//检查是否被攻击
        {
            return false;
        }
        public virtual void Be_attack() //被攻击后执行函数
        {
            HP=HP-1;
        }
        public virtual void End()//飞机被击落
        {

        }
        public virtual void  Timer_Tick(object sender, EventArgs e)
        {
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            if (Check_plane())
                Be_attack();
            Move(IMG);
            Attack();
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            if(HP==0)
            {
                End();
                return;
            }
        }
    }
    public class Enemy:Plane
    {
        private Player player;
        private int tag;
        int attack_time = 0;
        int move_time = 0;
        int endx, endy;
        private Bullets enemy_bullets;
        private Grid background;
        Random ran = new Random();
        public Enemy(int hp,int Mhp,Image img,Bullets enemyb,Player _p,Grid back,int t):base(hp,Mhp,img)
        {
            tag = t;
            background = back;
            enemy_bullets = enemyb;
            player = _p;
            endx = -x;
            endy = y+ran.Next( 700,800);
        }
        public override void Move(Image img)
        {
            move_time = (move_time + 1) %3;
            if (move_time!=1) {
                Thickness mov = img.Margin;
                mov.Left -= (x - endx) / 30;
                mov.Top -= (y - endy) / 50;
                img.Margin = mov;
            }
            if(System.Math.Abs(x - endx) <50)
            {
                endx = -endx;
                endy = -System.Math.Abs((int)(2 * y));
            }
            if(y<-900)
            {
                End();
            }
        }
        public override bool Check_plane()
        {
            for (int i = 0; i < Bullets.num; i++)
            {
                if(!player.player_bullets.empty_queue.Contains(i))
                {
                    Bullet b = player.player_bullets.childrens[i];
                    if((System.Math.Abs(b.x - x) < 80 && System.Math.Abs(b.y - y) < 10) || (System.Math.Abs(b.x - x) < 5 && System.Math.Abs(b.y - y) < 40))
                    {
                        Image en = new Image();
                        BitmapImage bimage = new BitmapImage();
                        bimage.BeginInit();
                        bimage.UriSource = new Uri("/boom.png", UriKind.Relative);
                        bimage.EndInit();
                        en.Source = (System.Windows.Media.ImageSource)bimage;
                        Thickness mov = b.img.Margin;
                        en.Margin = mov;
                        en.Width = 30;
                        en.Height = 30;
                        background.Children.Add(en);
                        b.Destroy();
                        
                        return true;
                    }
                }
            }
            return false;
        }
        public override void End()
        {
            player.enemies.background.Children.Remove(IMG);
            player.enemies.empty_queue.Enqueue(tag);
        }
        public override void Be_attack()
        {
            base.Be_attack();
        }
        public override void Attack()
        {
            base.Attack();
            attack_time = (attack_time + 1) % 60;
            if (attack_time == 1)
            {
                enemy_bullets.Add(x, y + 40, 0, 10);
            }
        }
        public override void Timer_Tick(object sender, EventArgs e)
        {
            base.Timer_Tick(sender, e);

        }
    }
    public class Player : Plane
    {
        private Label hplabel;
        public Bullets player_bullets;
        public Enemies enemies;
        private int attack_time=0;
        public Player( int hp, int Mhp,Image img,Label hpb,Bullets playerb,Enemies enemyb) : base( hp, Mhp,img)
        {
            hplabel = hpb;
            player_bullets = playerb;
            enemies = enemyb;
        }
        public override void Move(Image player_1)
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
        public override void Attack()
        {
            base.Attack();
            attack_time = (attack_time + 1) % 50;
            if(attack_time==1)
            {
                player_bullets.Add(x,y-20,0,-10);
            }
        }
        public override void Timer_Tick(object sender, EventArgs e)
        {
            base.Timer_Tick(sender, e);
            player_bullets.Timer_tick();
            enemies.Timer_tick(sender, e);
            hplabel.Content = "HP:" + HP.ToString() + "/" + MAXHP.ToString();
        }

        public override bool Check_plane()
        {
            for(int i=0;i<Enemies.num;i++)
            {
                if(!enemies.empty_queue.Contains(i))
                {
                    for(int j=0;j<Bullets.num;j++)
                    {
                        if(!enemies.childrens_bullets[i].empty_queue.Contains(j) )
                        {
                            Bullet b = enemies.childrens_bullets[i].childrens[j];
                            
                            if ((System.Math.Abs(b.x-x)<70&&System.Math.Abs(b.y-y)<10)|| (System.Math.Abs(b.x - x) < 5 && System.Math.Abs(b.y - y) < 80))
                            {
                                
                                Image en = new Image();
                                BitmapImage bimage = new BitmapImage();
                                bimage.BeginInit();
                                bimage.UriSource = new Uri("/boom.png", UriKind.Relative);
                                bimage.EndInit();
                                en.Source = (System.Windows.Media.ImageSource)bimage;
                                Thickness mov = b.img.Margin;
                                en.Margin = mov;
                                en.Width = 30;
                                en.Height = 30;
                                enemies.background.Children.Add(en);
                                b.Destroy();
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public override void Be_attack()
        {
            base.Be_attack();
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void Debug(object sender, EventArgs e)
        {
            temp.Text = player_1.Margin.ToString();
           
        }
        public void Backimgmove(object sender, EventArgs e)
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
            Bullets player_bullets = new Bullets(background,player_1);
            player_1.Tag = "N";
            Enemies enemies1 = new Enemies(background,null);
            Player player1 = new Player(10,10,player_1,HPinfo,player_bullets,enemies1);
            enemies1.player = player1;
            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1);   //间隔1秒
            timer.Tick += new EventHandler(player1.Timer_Tick);
            timer.Tick += new EventHandler(Backimgmove);
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
        }
    }
}