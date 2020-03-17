using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Thunder
{
    public class Booms
    {
        public static int num = 4;
        public Grid background;
        public Boom[] childrens = new Boom[num];
        public Queue<int> empty_queue = new Queue<int>();
        public Booms(Grid back)
        {
            background = back;
            for(int i=0;i<num;i++)
            {
                empty_queue.Enqueue(i);
            }
        }
        public void Add(Thickness p)
        {
            if (empty_queue.Count() >= 1)
            {
                int i = empty_queue.Dequeue();
                childrens[i] = new Boom(p,background,this,i);
            }
        }
        public void Timer_tick()
        {
            for (int i = 0; i < num; i++)
            {
                if (!empty_queue.Contains(i))
                {
                    childrens[i].Timer_tick();
                }
            }
        }
    }
    public class Boom
    {
        private int time = 0;
        private int tag;
        private Thickness pos;
        private Grid background;
        private Booms father;
        private Image en;
        public Boom(Thickness p,Grid back,Booms _father,int t)
        {
            tag = t;
            pos = p;
            background = back;
            father = _father;
        }
        public virtual void Timer_tick()
        {
            time++;
            string png = "/" + (time / 2 + 1).ToString() + ".png";
            if(time==1)
            {
                en = new Image();
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(png, UriKind.Relative);
                bimage.EndInit();
                en.Source = (System.Windows.Media.ImageSource)bimage;
                Thickness mov = pos;
                en.Margin = mov;
                en.Width = 80;
                en.Height = 80;
                background.Children.Add(en);
            }
            if(time>1&&time<13)
            {
                background.Children.Remove(en);
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(png, UriKind.Relative);
                bimage.EndInit();
                en.Source = (System.Windows.Media.ImageSource)bimage;
                background.Children.Add(en);
            }
            if(time==13)
            {
                background.Children.Remove(en);
                father.empty_queue.Enqueue(tag);
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
                en.Height = 15;
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
            if (y<800&&y>-600)
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
        public virtual void Boom_an()//爆炸动画
        {

        }
        public virtual void  Timer_Tick()
        {
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            if (Check_plane())
                Be_attack();
            Move(IMG);
            Attack();
            x = (int)IMG.Margin.Left;
            y = (int)IMG.Margin.Top;
            
        }
    }
    public class Enemies
    {
        public int score = 0;
        TextBlock scoreboard;
        public static int num = 8;
        public Player player;
        Random ran = new Random();
        public Grid background;
        public Enemy[] childrens = new Enemy[num];
        public Bullets[] childrens_bullets = new Bullets[num];
        public Queue<int> empty_queue = new Queue<int>();
        int add_time = 0;
        public Enemies(Grid back, Player _player,TextBlock sb)
        {
            scoreboard = sb;
            player = _player;
            background = back;
            for (int i = 0; i < num; i++)
            {
                empty_queue.Enqueue(i);
                childrens_bullets[i] = new Bullets(background, null);
                childrens[i] = new Enemy(2, 2, null, childrens_bullets[i], player, background, i);

            }
        }
        public void Add()
        {
            if (empty_queue.Count() >= 1)
            {
                int i = empty_queue.Dequeue();
                Image en = new Image();
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri("enemy.png", UriKind.Relative);
                bimage.EndInit();
                en.Source = bimage;
                Thickness mov = new Thickness(ran.Next(-600, 600), ran.Next(-800, -700), 0, 0);

                en.Margin = mov;
                en.Width = 60;
                en.Height = 60;
                background.Children.Add(en);
                childrens_bullets[i] = new Bullets(background, en);
                childrens[i] = new Enemy(2, 2, en, childrens_bullets[i], player, background, i);
            }
        }
        public void Timer_tick()
        {
            add_time = (add_time + 1) % 80;
            if (add_time == 1)
                Add();
            for (int i = 0; i < num; i++)
            {
                if (!empty_queue.Contains(i))
                {
                    childrens[i].Timer_Tick();
                }

                childrens_bullets[i].Timer_tick();
            }
            scoreboard.Text = "Score " + score.ToString();
        }

        public void Destroy()
        {
            for (int i = 0; i < num; i++)
            {
                if (!empty_queue.Contains(i))
                {
                    childrens[i].End();
                }

                for (int j = 0; j < Bullets.num; j++)
                {
                    if (!childrens_bullets[i].empty_queue.Contains(j))
                    {
                        childrens_bullets[i].childrens[j].Destroy();
                    }
                }
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
                endx = -endx+ran.Next(-150,150);
                endy = -System.Math.Abs((int)(2 * y));
            }
            if(y<-900)
            {
                End();
            }
        }
        public override bool Check_plane()
        {
            if(player.thunder_active!=0&&((System.Math.Abs(player.x - x) < 40 && player.y > y)))
            {
                player.enemies.score += 10;
                return true;
            }
            for (int i = 0; i < Bullets.num; i++)
            {
                if(!player.player_bullets.empty_queue.Contains(i))
                {
                    Bullet b = player.player_bullets.childrens[i];
                    if((System.Math.Abs(b.x - x) < 80 && System.Math.Abs(b.y - y) < 10) || (System.Math.Abs(b.x - x) < 5 && System.Math.Abs(b.y - y) < 40))
                    {
                        b.Destroy();
                        if (HP == 2)
                        {
                            background.Children.Remove(IMG);
                            BitmapImage bit = new BitmapImage();
                            bit.BeginInit();
                            bit.UriSource = new Uri("/enemy_d.png", UriKind.Relative);
                            bit.EndInit();
                            IMG.Source = (System.Windows.Media.ImageSource)bit;
                            background.Children.Add(IMG);
                        }
                        player.enemies.score += 10;
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
        public override void Boom_an()
        {
            player.booms.Add(IMG.Margin);
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
                if(player.x>x)
                    enemy_bullets.Add(x, y + 40, ran.Next(0,4), 10);
                if(player.x<x)
                    enemy_bullets.Add(x, y + 40, ran.Next(-4,0), 10);
            }
        }
        public override void Timer_Tick()
        {
            base.Timer_Tick();
            if (HP == 0)
            {
                player.enemies.score += 20;
                Boom_an();
                End();
                return;
            }
        }
    }
    public class Player : Plane
    {
        Image thunder;
        private Label hplabel;
        public Bullets player_bullets;
        public Enemies enemies;
        public Booms booms;
        private int attack_time=0;
        private int thunder_time = 0;
        public int thunder_active=0;
        private int thunder_score = 0;
        private int gap = 300;
        public Player( int hp, int Mhp,Image img,Label hpb,Bullets playerb,Enemies enemyb,Booms b) : base( hp, Mhp,img)
        {
            booms = b;
            hplabel = hpb;
            player_bullets = playerb;
            enemies = enemyb;
            
        }
        public void Thunder_act()
        {
            if (thunder_active == 0 )
                return;
            thunder_time++;
            string png = "";
            Thickness thpos = IMG.Margin;
            thpos.Left += -20;
            thpos.Top += -565;
            png = "1 (" + (thunder_time).ToString() + ").png";
            if (thunder_time == 1)
            {
                if (HP > MAXHP-3)
                    HP = MAXHP;
                else
                {
                    HP += 3;
                }
                thunder = new Image();
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(png, UriKind.Relative);
                bimage.EndInit();
                thunder.Source = (System.Windows.Media.ImageSource)bimage;
                thunder.Margin = thpos;
                thunder.Width = 312;
                thunder.Height = 716;
                enemies.background.Children.Add(thunder);
             }
            if (thunder_time > 1 && thunder_time < 40)
            {
                enemies.background.Children.Remove(thunder);
                thunder.Margin = thpos;
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(png, UriKind.Relative);
                bimage.EndInit();
                thunder.Source = (System.Windows.Media.ImageSource)bimage;
                enemies.background.Children.Add(thunder);
            }
            if (thunder_time > 40 && thunder_time < 400)
             {
                png = "/after/1 (" + (thunder_time%40+16).ToString() + ").png";
                enemies.background.Children.Remove(thunder);
                thunder.Margin = thpos;
                BitmapImage bimage = new BitmapImage();
                bimage.BeginInit();
                bimage.UriSource = new Uri(png, UriKind.Relative);
                bimage.EndInit();
                thunder.Source = (System.Windows.Media.ImageSource)bimage;
                enemies.background.Children.Add(thunder);
             }
            if (thunder_time > 400)
            {
                enemies.background.Children.Remove(thunder);
                thunder_active = 0;
                thunder_time = 0;
                thunder_score = enemies.score;
            }
            if(HP==0)
                enemies.background.Children.Remove(thunder);
        }
        public override void Move(Image player_1)
        {
            int step = 5;
            foreach (char i in player_1.Tag.ToString())
            {
                if(i=='T' && enemies.score-thunder_score>=gap)
                {
                    thunder_active = 1;
                }
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
            attack_time = (attack_time + 1) % 25;
            if(attack_time==1&&thunder_active==0)
            {
                player_bullets.Add(x,y-20,0,-20);
            }
        }
        public override void End()
        {
            enemies.background.Children.Remove(IMG);
            enemies.background.Tag = 0;

        }
        public override void Timer_Tick()
        {
            base.Timer_Tick();
            hplabel.Content = "HP:" + HP.ToString() + "/" + MAXHP.ToString();
            if((enemies.score - thunder_score) <gap)
                hplabel.Content += "   雷电充能" + ((enemies.score - thunder_score) / 3).ToString() + "%";
            else
            {
                hplabel.Content += "  按T释放雷电";
            }
            if(HP==0)
            {
                End();
            }
        }

        public override bool Check_plane()
        {
            if (thunder_active != 0)
                return false;
            for (int i = 0; i < Enemies.num; i++)
            {
                if (!enemies.empty_queue.Contains(i))
                {
                    if(System.Math.Abs(enemies.childrens[i].x-x)<60&& System.Math.Abs(enemies.childrens[i].y - y)<40)
                    {
                        booms.Add(IMG.Margin);
                        return true;
                    }
                }
                    for(int j=0;j<Bullets.num;j++)
                    {
                        if(!enemies.childrens_bullets[i].empty_queue.Contains(j) )
                        {
                            Bullet b = enemies.childrens_bullets[i].childrens[j];
                            
                            if ((System.Math.Abs(b.x-x)<70&&System.Math.Abs(b.y-y)<15)|| (System.Math.Abs(b.x - x) < 5 && System.Math.Abs(b.y - y) < 80))
                            {
                                b.Destroy();
                                booms.Add(b.img.Margin);
                                return true;
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
        Timer timer;
        Player player1;
        Enemies enemies1;
        Bullets player_bullets;
        Booms booms;
        TextBlock endmessage;
        private delegate void TimerDispatcherDelegate();
        public void Backimgmove()
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
        public void Check_Death()
        {
            if ((int)background.Tag == 0)
            {

                background.Children.Add(Start);
                background.Children.Add(Exit);
                endmessage = new TextBlock();
                endmessage.Text = "你的得分为： " + enemies1.score.ToString();
                endmessage.Height = 100;
                endmessage.Width = 400;
                Thickness mar = new Thickness(0, 0, 0, 0);
                endmessage.Margin = mar;
                endmessage.FontSize = 30;
                endmessage.TextAlignment = TextAlignment.Center;
                endmessage.Foreground = new SolidColorBrush(Colors.White);
                enemies1.background.Children.Add(endmessage);
                Start.Tag = 1;
                Start.Content = "重新开始";
                //timer.Enabled = false;

            }
        }

        public MainWindow()
        {
            Timeline.DesiredFrameRateProperty.OverrideMetadata(
               typeof(Timeline),
               new FrameworkPropertyMetadata { DefaultValue = 90 }
               );
            InitializeComponent();
            Start.Tag = 0;
            background.Children.Remove(player_1);
            //timer.Start();
        }


        private void Timer_Tick()
        {
            object A = new object();
            EventArgs e = new EventArgs();
            if ((int)background.Tag == 1)
            {
                player1.Timer_Tick();
                player1.enemies.Timer_tick();
                Backimgmove();
                Check_Death();
                player1.Thunder_act();
            }
            player1.booms.Timer_tick();
            player_bullets.Timer_tick();

        }

    
    private void Button_Click(object sender, RoutedEventArgs e)
        {
            if((int)Start.Tag==1)
            {
                background.Children.Remove(endmessage);
                player_1.Margin = new Thickness(0, 483, 0, 0);
                enemies1.Destroy();
                timer.Dispose();
            }
            background.Children.Add(player_1);
            player_bullets = new Bullets(background, player_1);
            player_1.Tag = "N";
            enemies1 = new Enemies(background, null, scoreboard);
            booms = new Booms(background);
            background.Tag = 1;
            player1 = new Player(10, 10, player_1, HPinfo, player_bullets, enemies1, booms);
            enemies1.player = player1;

            background.Children.Remove(Start);
            background.Children.Remove(Exit);
            background.Children.Remove(Picture);

            timer = new Timer(10);
            timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            timer.Interval = 10;
            timer.Enabled = true;

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new TimerDispatcherDelegate(Timer_Tick));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            player_1.Tag = "";
            if (Keyboard.IsKeyDown(Key.T))
                player_1.Tag += "T";
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
            Window_KeyDown_1(sender,e);
        }
    }
}