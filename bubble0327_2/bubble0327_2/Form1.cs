using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace bubble0327_2
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		Socket T;
		Thread Th;
		string User;

		Panel panel1 = new Panel();
		Graphics g;
		Graphics gpanel;
		Pen pRed;
		Pen pOrange;
		Pen pYellow;
		Pen pGreen;
		Pen pPurple;
		SolidBrush RedBrush;
		SolidBrush OrangeBrush;
		SolidBrush YellowBrush;
		SolidBrush GreenBrush;
		SolidBrush PurpleBrush;
		Random colorchoose;

		Pen pBlue;
		SolidBrush BlueBrush;
		Pen pLine;
		Pen pClear;
		SolidBrush ClearBrush;

		int ballx;
		int bally;
		int ballx1;
		int bally1;
		int tmp;
		int previous;

		int mouseX;
		int mouseY;

		int rr = 0, nn = 0; //
		int timeleft;
		System.Timers.Timer aTimer = new System.Timers.Timer();
		//以宣告timer代替直接拉出一個timer元件，避免timer元件被thread影響的情況發生
		delegate void timeHandler1(object source, System.Timers.ElapsedEventArgs e);
		//解決跨執行緒作業無效問題

		//存放泡泡顏色及座標(row,泡泡編號,顏色及座標)，0~4存放原螢幕上的泡泡，5~9存放射擊上去的泡泡(如未消除)
		int[,,] coordi = new int[10, 8, 3];
		int[] edge = new int[15];
		double[] edgeslope = new double[15];
		int tmpn;//相連泡泡個數(不包括涉及的泡泡)
		int tmpi, tmpj;//泡泡位置暫存器
		bool[,] tmpco = new bool[10, 8];//存放泡泡是否相連
		int sign;//相連泡泡個數(不包括涉及的泡泡)
		int sigi, sigj;//泡泡位置暫存器
		bool[,] sigco = new bool[10, 8];//存放泡泡是否相連
		int points = 0;//我的分數(相連顆數^2/3)

		private void Form1_Load(object sender, EventArgs e)
		{
			button1.Enabled = false;//Ready
			button2.Select();
			label1.Text = "我的暱稱";

			g = this.pictureBox1.CreateGraphics();

			colorchoose = new Random();
			pRed = new Pen(Color.Red);
			pOrange = new Pen(Color.DarkOrange);
			pYellow = new Pen(Color.Yellow);
			pGreen = new Pen(Color.Green);
			pPurple = new Pen(Color.Purple);
			RedBrush = new SolidBrush(Color.LightCoral);
			OrangeBrush = new SolidBrush(Color.Orange);
			YellowBrush = new SolidBrush(Color.Yellow);
			GreenBrush = new SolidBrush(Color.LightGreen);
			PurpleBrush = new SolidBrush(Color.Violet);

			pBlue = new Pen(Color.Blue);
			BlueBrush = new SolidBrush(Color.Blue);
			pLine = new Pen(Color.Black);
			pClear = new Pen(Color.PowderBlue);
			ClearBrush = new SolidBrush(Color.PowderBlue);
		}
		void Paint(int color, int x, int y)//畫出泡泡
		{
			x -= 15;
			y -= 15;
			switch (color)
			{
				case 1:
					g.DrawEllipse(pRed, x, y, 30, 30);
					g.FillEllipse(RedBrush, x, y, 30, 30);
					break;
				case 2:
					g.DrawEllipse(pYellow, x, y, 30, 30);
					g.FillEllipse(YellowBrush, x, y, 30, 30);
					break;
				case 3:
					g.DrawEllipse(pGreen, x, y, 30, 30);
					g.FillEllipse(GreenBrush, x, y, 30, 30);
					break;
				case 4:
					g.DrawEllipse(pOrange, x, y, 30, 30);
					g.FillEllipse(OrangeBrush, x, y, 30, 30);
					break;
				case 5:
					g.DrawEllipse(pPurple, x, y, 30, 30);
					g.FillEllipse(PurpleBrush, x, y, 30, 30);
					break;
				default:
					g.DrawEllipse(pClear, x, y, 30, 30);
					g.FillEllipse(ClearBrush, x, y, 30, 30);
					break;

			}

		}
		bool Search(int i, int pos)                                                                       //Search(i, line[i])
		{

			/*this.textBox6.Text = coordi[4, 0, 0].ToString();*/
			if (i % 2 == 0 && i > 0 && pos >= 0 && pos <= 6 && coordi[i - 1, pos, 0] != 0)                 //1
			{
				this.textBox11.Text = (rr).ToString();
				this.textBox12.Text = nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 != 0 && i > 0 && pos >= 0 && pos <= 6 && coordi[i - 1, pos + 1, 0] != 0)                 //1
			{
				this.textBox11.Text = (rr).ToString();
				this.textBox12.Text = (nn).ToString();
				this.textBox13.Text = coordi[i , pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 != 0 && pos <= 5 && coordi[i, pos + 1, 0] != 0)                            //2
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text = (nn).ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 == 0 && pos <= 6 && coordi[i, pos + 1, 0] != 0)
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text = (nn).ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 != 0 && i < 9 && pos <= 6 && coordi[i + 1, pos + 1, 0] != 0)                              //3
			{
				this.textBox11.Text = (rr).ToString();
				this.textBox12.Text = (nn).ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 == 0 && i < 9 && pos <= 6 && coordi[i + 1, pos, 0] != 0)                              //3
			{
				this.textBox11.Text = (rr).ToString();
				this.textBox12.Text = nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 == 0 && i < 9 && pos >= 1 && coordi[i + 1, pos - 1, 0] != 0)							 //4 八個一排的 =基數排
			{
				this.textBox11.Text = (rr).ToString();
				this.textBox12.Text = (nn).ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 != 0 && i < 9 && pos >= 0 && coordi[i + 1, pos, 0] != 0)
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text =nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (pos >= 1 && coordi[i, pos - 1, 0] != 0)                                      //5
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text = nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 == 0 && i != 0 && pos >= 1 && coordi[i-1, pos - 1, 0] != 0)          //6
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text =nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if (i % 2 != 0 && pos >= 0 && coordi[i-1, pos, 0] != 0)                    //6
			{
				this.textBox11.Text = rr.ToString();
				this.textBox12.Text = nn.ToString();
				this.textBox13.Text = coordi[i, pos, 0].ToString();
				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else if(i==0 && coordi[i, pos, 0] == 0)
            {

				coordi[i, pos, 0] = previous;
				Paint(coordi[i, pos, 0], coordi[i, pos, 1], coordi[i, pos, 2]);
				return true;
			}
			else
			{
				rr = i;
				nn = pos;
				return false;
			}

		}
		private void chks_bon(int i, int j)
		{
			//偵測六個方向
			if (chks_left(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
			if (chks_right(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
			if (chks_ul(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
			if (chks_ur(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
			if (chks_dl(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
			if (chks_dr(i, j))
			{
				chks_bon(tmpi, tmpj);
			}
		}
		//偵測上方是否有同色相鄰
		private bool chks_ul(int i, int j)
		{
			//-->偵測斜上，相鄰位置(row-1 , num or num+row%2*2-1)
			if (i - 1 >= 0 && j <= 7 - i % 2 && coordi[i - 1, j, 0] != 0 && tmpco[i - 1, j] == false)
			{
				tmpi = i - 1;
				tmpj = j;
				tmpco[i - 1, j] = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測上方是否有同色相鄰
		private bool chks_ur(int i, int j)
		{
			if (i - 1 >= 0 && j + i % 2 * 2 - 1 >= 0 && j + i %2 * 2 - 1 <= 7 - i % 2 && coordi[i - 1, j + i %2 * 2 - 1, 0] != 0 && tmpco[i - 1, j + i %2 * 2 - 1] == false)
			{
				tmpi = i - 1;
				tmpj = j + i %2 * 2 - 1;
				tmpco[i - 1, j + i %2 * 2 - 1] = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測下方是否有同色相鄰
		private bool chks_dl(int i, int j)
		{
			//偵測斜下，相鄰位置(row+1 , num)
			if (i + 1 <= 9 && j <= 7 - i % 2 && coordi[i + 1, j, 0] != 0 && tmpco[i + 1, j] == false)
			{
				tmpi = i + 1;
				tmpj = j;
				tmpco[i + 1, j] = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測下方是否有同色相鄰
		private bool chks_dr(int i, int j)
		{
			if (i + 1 <= 9 && j + i %2 * 2 - 1 >= 0 && j + i % 2 * 2 - 1 <= 7 - i % 2 && coordi[i + 1, j + i %2 * 2 - 1, 0] != 0 && tmpco[i + 1, j + i %2 * 2 - 1] == false)
			{
				//偵測斜下，相鄰位置(row+1 , num+row%2*2-1)
				tmpi = i + 1;
				tmpj = j + i %2 * 2 - 1;
				tmpco[i + 1, j + i %2 * 2 - 1] = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測左邊是否有同色相鄰
		private bool chks_left(int i, int j)
		{
			if (j - 1 >= 0 && coordi[i, j - 1, 0] != 0 && tmpco[i, j - 1] == false)
			{
				tmpi = i;
				tmpj = j - 1;
				tmpco[i, j - 1] = true;
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測右邊是否有同色相鄰
		private bool chks_right(int i, int j)
		{
			if (j + 1 <= 7 - i % 2 && coordi[i, j + 1, 0] != 0 && tmpco[i, j + 1] == false)
			{
				tmpi = i;
				tmpj = j + 1;
				tmpco[i, j + 1] = true;
				return true;
			}
			else
			{
				return false;
			}

		}


		//偵測相連數目
		private void chk_bon(int ci, int cj)
		{

			//偵測六個方向
			if (chk_left(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
			if (chk_right(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
			if (chk_ul(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
			if (chk_ur(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
			if (chk_dl(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
			if (chk_dr(ci, cj))
			{
				tmpn++;
				chk_bon(tmpi, tmpj);
			}
		}

		//偵測上方是否有相鄰
		private bool chk_ul(int ci, int cj)
		{
			//-->偵測斜上，相鄰位置(row-1 , num or num+row%2*2-1)
			if (ci - 1 >= 0 && cj >= 0 && cj <= 7 - ci % 2 && coordi[ci - 1, cj, 0] == coordi[ci, cj, 0] && tmpco[ci - 1, cj] == false)
			{
				tmpi = ci - 1;
				tmpj = cj;
				tmpco[ci - 1, cj] = true;
				return true;
			}
			else
			{
				return false;
			}
		}

		//偵測上方是否有相鄰
		private bool chk_ur(int i, int j)
		{
			if (i - 1 >= 0 && j + i %2 * 2 - 1 >= 0 && j + i %2 * 2 - 1 <= 7 - i % 2 && coordi[i - 1, j + i %2 * 2 - 1, 0] == coordi[i, j, 0] && tmpco[i - 1, j + i %2 * 2 - 1] == false)
			{
				tmpi = i - 1;
				tmpj = j + i %2 * 2 - 1;
				tmpco[i - 1, j + i %2 * 2 - 1] = true;
				//Paint(0, i - 1, j + i %2 * 2 - 1);
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測下方是否有相鄰
		private bool chk_dl(int i, int j)
		{
			//偵測斜下，相鄰位置(row+1 , num)
			if (i + 1 <= 9 && j >= 0 && j <= 7 - i % 2 && coordi[i + 1, j, 0] == coordi[i, j, 0] && tmpco[i + 1, j] == false)
			{
				tmpi = i + 1;
				tmpj = j;
				tmpco[i + 1, j] = true;
				//Paint(0, i + 1, j);
				return true;
			}
			else
			{
				return false;
			}
		}

		//偵測下方是否有相鄰
		private bool chk_dr(int i, int j)
		{
			if (i + 1 <= 9 && j + i %2 * 2 - 1 >= 0 && j + i %2 * 2 - 1 <= 7 - i % 2 && coordi[i + 1, j + i %2 * 2 - 1, 0] == coordi[i, j, 0] && tmpco[i + 1, j + i %2 * 2 - 1] == false)
			{
				//偵測斜下，相鄰位置(row+1 , num+row%2*2-1)
				tmpi = i + 1;
				tmpj = j + i %2 * 2 - 1;
				tmpco[i + 1, j + i %2 * 2 - 1] = true;
				//Paint(0, i + 1, j + i %2 * 2 - 1);
				return true;
			}
			else
			{
				return false;
			}
		}

		//偵測左邊相連個數的子方法
		private bool chk_left(int i, int j)
		{
			if (j - 1 >= 0 && coordi[i, j - 1, 0] == coordi[i, j, 0] && tmpco[i, j - 1] == false)
			{
				tmpi = i;
				tmpj = j - 1;
				tmpco[i, j - 1] = true;
				//Paint(0, i ,j-1);
				return true;
			}
			else
			{
				return false;
			}
		}
		//偵測右邊相連個數的子方法
		private bool chk_right(int i, int j)
		{
			if (j + 1 <= 7 - i % 2 && coordi[i, j + 1, 0] == coordi[i, j, 0] && tmpco[i, j + 1] == false)
			{
				tmpi = i;
				tmpj = j + 1;
				tmpco[i, j + 1] = true;
				//Paint(0, i, j + 1);
				return true;
			}
			else
			{
				return false;
			}

		}
		private void tmpco_clr()
		{
			for (int row = 0; row < 10; row++)
			{
				for (int num = 0; num < 8 - row % 2; num++)
				{
					tmpco[row, num] = false;
				}
			}
		}

		private bool distinguish()
        {
			int n=0;
			for(int i = 0; i < 7; i++)
            {
				if(coordi[9, i, 0] != 0)
                {
					n++;
                }
            }
            if (n == 7)
            {
				return true;
            }
				return false;
        }
		private void Die()
        {
			Send("your competitor died." + listBox1.SelectedItem);
        }
		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			button1.Enabled = true;
			button1.Select(); //轉移焦點到button2 
		}
		private void Send(string Str)
		{
			byte[] B = Encoding.Default.GetBytes(Str);//轉成byte陣列
			T.Send(B, 0, B.Length, SocketFlags.None);//傳送訊息給伺服器
		}
		//監聽server訊息
		private void Listen()
		{
			EndPoint ServerEP = (EndPoint)T.RemoteEndPoint;
			byte[] B = new byte[1023];//接收用的陣列
			int inLen = 0;//接收的位元組數目
			string Msg;//接收到的完整訊息
			string St;//命令碼
			string Str;//訊息內容
			while (true)//監聽迴圈
			{
				try
				{
					inLen = T.ReceiveFrom(B, ref ServerEP);//收聽資訊、取得位元組數
				}
				catch (Exception)
				{
					T.Close();//關閉通訊器
					listBox1.Items.Clear();//清除線上名單
					MessageBox.Show("伺服器斷線了!");
					button1.Enabled = true;//連線按鈕可使用
					Th.Abort();//刪除執行緒
				}
				Msg = Encoding.Default.GetString(B, 0, inLen);//解讀完整訊息
				St = Msg.Substring(0, 1);//取出命令碼的第一個字
				Str = Msg.Substring(1);//取出命令碼之後的訊息
				switch (St)//依命令碼執行功能
				{
					case "L"://接收線上名單
						listBox1.Items.Clear();//清除名單
						string[] M = Str.Split(',');//拆解名單成陣列
						for (int i = 0; i < M.Length; i++) listBox1.Items.Add(M[i]);//加入名單
						break;
					//-->對手的動作
					case "4":
					case "5":
						string[] P = Str.Split('|');
						textBox10.Text = P[0];//更新對手的得分
						break;
				}
			}
		}

		private void Form1_FormClosing(object sender, FormClosedEventArgs e)
		{
			if (button2.Enabled == false)
			{
				Send("9" + User);//傳送離線訊息給伺服器
				T.Close();//關閉網路通訊器
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			button1.Enabled = false;
			
			timeleft = 120; //倒計時初始值
			label4.Text = timeleft + "seconds";
			label1.Text = "我的得分";
			textBox3.Text = "0";
			textBox10.Text = "0";
			aTimer.Elapsed += new ElapsedEventHandler(theout);
			aTimer.Interval = 1000;
			aTimer.AutoReset = true;
			aTimer.Enabled = true;

			int[] array = new int[5];

			ballx = 120;//發射小泡座標
			bally = 375;
			bally1 = 0 + 15;

			tmp = colorchoose.Next(1, 6);//用亂數隨機選泡泡顏色
			previous = tmp;//為使其能以同顏色射擊到反射點所設的暫時變數
			Paint(tmp, 120, 375);

			for (int row = 0; row < 5; row++)//欲射擊泡泡
			{
				if (row % 2 == 0)                                                             //八個一排
				{
					ballx1 = 0 + 15;

					for (int i = 0; i < 8; i++)
					{
						tmp = colorchoose.Next(1, 6);//用亂數隨機選泡泡顏色,共五色,亂數取1~5(將背景色代號設為0)
						Paint(tmp, ballx1, bally1);

						coordi[row, i, 0] = tmp;
						coordi[row, i, 1] = ballx1;
						coordi[row, i, 2] = bally1;

						ballx1 += 35;

					}
				}
				else                                                                           //七個一排
				{
					ballx1 = 17 + 15;
					for (int j = 0; j < 7; j++)
					{
						tmp = colorchoose.Next(1, 6);
						Paint(tmp, ballx1, bally1);

						coordi[row, j, 0] = tmp;
						coordi[row, j, 1] = ballx1;
						coordi[row, j, 2] = bally1;

						ballx1 += 35;

					}
				}
				bally1 += 30;
			}
			for (int row = 5; row < 10; row++)
			{
				if (row % 2 == 0)
				{
					ballx1 = 0 + 15;

					for (int i = 0; i < 8; i++)
					{
						Paint(0, ballx1, bally1);

						coordi[row, i, 0] = 0;
						coordi[row, i, 1] = ballx1;
						coordi[row, i, 2] = bally1;

						ballx1 += 35;
					}
				}
				else
				{
					ballx1 = 17 + 15;
					for (int j = 0; j < 7; j++)
					{
						Paint(0, ballx1, bally1);

						coordi[row, j, 0] = 0;
						coordi[row, j, 1] = ballx1;
						coordi[row, j, 2] = bally1;

						ballx1 += 35;
					}
				}
				bally1 += 30;
			}
		}

		private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//滑鼠按下-->發射泡泡(找到滑鼠座標、斜率-->沿瞄準路徑，從最底層尋找是否有相鄰泡泡(Search(i, line[i])，是否碰強反射))
				double jz = 0, tmpy = 0, tmpx = -30;
				int j, ff = 0, t = 0, y = 0, x = 0;
				int[] line = new int[10];
				int xs = (e.X) - 120;
				int ys = 375 - (e.Y);
				
				/*float mouse = ys / xs;
				/*float mouse = ys / xs;
				/*float mouse = ys / xs;
				float inver = xs / ys;
				this.textBox4.Text = mouse.ToString();
				this.textBox5.Text = inver.ToString();
				if (xs != 0)
                {
					//mouse = ys / xs;
                }
                else
                {
					jz = 1;
                }
				*/
				int painted = 0;
				for (int i = 9; i >= 0; i--)
				{
					if (painted == 1)
					{
						break;
					}
					if (ff == 0)
					{
						y = (12 - i) * 30;                                                        //以發射圓心(120, 375)為原點的座標
						if (jz == 0)
						{
							x = (int)(y * xs / ys);
							//t = i;
							if (i % 2 != 0)                                             //七個一排
							{
								if (x > 2)                                              //右半邊
								{
									if (x >= 155)                                        //右反射
									{
										ff = 1;
										x = 155;                                                    //反射點座標，以(120, 375)為原點
										y = (int)(x * ys / xs);
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										t = (int)((375 - y) / 30);
										if (t > 9)
										{
											if (distinguish())
											{
												Die();
												break;
											}
											else
											{
												t = 9;
											}
										}
										i = t;
										ys = -ys;
										tmpy = (375 - y) - i * 30 + 15;
										tmpx = (int)(tmpy * xs / ys);                             //負數
										i--;
										line[i] = (int)(7 + (tmpx - 5) / 35);
										if (coordi[i, line[i], 0] != 0)
										{
											i++;
											line[i] = 6;
										}
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													if (coordi[0, num, 0] != 0)
													{
														tmpco[0, num] = true;
														chks_bon(0, num);
													}

												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();
											}
											painted = 1;
											break;
										}
									}//射出撞右牆反射
									 //射向右半邊 不撞牆
									else
									{
										line[i] = 3 + (x - 2) / 35;
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}
												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											} //if tmpn>=2 end
											painted = 1;
											break;
										}//if Search(i, line[i]) end
									} ////射向右半邊 不撞牆 end
								} // if x>2 end ***2 是(八個一排對齊七個一排 球之間交錯的誤差)
								else//射向左半邊(else以下)                                              
								{
									if (x <= -120)                                           //左反射  ***-120是發射球中心到左牆的長度
									{
										ff = 1;
										x = -120;                                            //反射點座標，以(120, 375)為原點
										y = (int)(x * ys / xs);
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										t = (375 - y) / 30;
										if (t > 9)
										{
											if (distinguish())
											{
												Die();
												painted = 1;
												break;
											}
											else
											{
												t = 9;
											}
										}
										i = t;
										ys = -ys;
										tmpy = (375 - y) - i * 30 + 15;                      //tmpy = 第i-1排球中心到x的距離，tmpx = coordi(i-1, line[i-1], 1)
										tmpx = (tmpy * xs / ys);                              //tmpx為正(tmpx是第i-1排球反射後的位置)
										i -= 1;                                              //因為反射點在七個一排(i)最左邊的縫隙上，所以找反射後第一個落點(i-1排上)
										line[i] = (int)(tmpx / 35);
										if (coordi[i, line[i], 0] != 0)                   //確定反射後第一個落點上沒有球
										{
											i++;
											line[i] = 0;
										}
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											}//if tmpn>=2 end
											painted = 1;
											break;
										}//if Search(i, line[i]) end
									}//射出碰左牆 反射
									 //射向左半邊 且不碰牆
									else
									{
										y = (12 - i) * 30;                                                        //以發射圓心(120, 375)為原點的座標
										if (jz == 0)//mouse!=0
										{
											x = (int)(y * xs / ys);
										}
										else
										{
											x = 0;
										}
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										line[i] = 2 + x / 35;
										if ((2 + x / 35) < 0)
										{
											line[i] = 0;

										}
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											} //if tmp>=2 
											painted = 1;
											break;
										}// if Search(i, line[i]) end
									}//射向左半邊 且不碰牆
								}
							}
							else                                                        //八個一排
							{
								if (x > 0)                                              //右半邊
								{
									if (x >= 155)                                        //右反射
									{
										ff = 1;
										x = 155;                                                    //反射點座標，以(120, 375)為原點
										y = (int)(x * ys / xs);
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										t = (375 - y) / 30;
										if (t > 9)
										{
											if (distinguish())
											{
												Die();
												break;
											}
											else
											{
												t = 9;
											}
										}
										i = t;
										ys = -ys;                           //負數
										line[i] = 7;
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											}//if tmpn>=2 end
											painted = 1;
											break;
										}//Search end
									}//右反射 end
									else
									{
										y = (12 - i) * 30;                                                        //以發射圓心(120, 375)為原點的座標
										if (jz == 0)//mouse!=0
										{
											x = (int)(y * xs / ys);
										}
										else
										{
											x = 0;
										}
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										line[i] = 3 + x / 35;
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											} // if tmpn>=2 end
											painted = 1;
											break;
										}//Search(i, line[i]) end
									}//在右半邊射出，不碰牆
								}//右半邊 end
								else                                                    //左半邊
								{
									if (x <= -120)                                        //左反射
									{
										ff = 1;
										ys = -ys;
										x = -120;                                                    //反射點座標，以(120, 375)為原點
										y = (int)(x * -ys / xs);
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										t = (375 - y) / 30;
										if (t > 9)
										{
											if (distinguish())
											{
												Die();
												break;
											}
											else
											{
												t = 9;
											}
										}
										i = t;
										line[i] = 0;
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											}
											painted = 1;
											break;
										}//if Search end
									}//左反射 end
									else
									{
										y = (12 - i) * 30;                                                        //以發射圓心(120, 375)為原點的座標
										if (jz == 0)// mouse!=0
										{
											x = (int)(y * xs / ys);
										}
										else
										{
											x = 0;
										}
										this.textBox8.Text = x.ToString();
										this.textBox9.Text = y.ToString();
										line[i] = 2 + x / 35;
										if (2 + x / 35 < 0)
										{
											line[i] = 0;

										}
										if (Search(i, line[i]))
										{
											Thread.Sleep(500);
											tmpn = 0;
											tmpi = i;
											tmpj = line[i];
											tmpco_clr();
											tmpco[i, line[i]] = true;
											chk_bon(i, line[i]);//偵測相連泡泡數目
											if (tmpn >= 2)
											{
												points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																			 //相連的的泡泡要消失
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (tmpco[row, num] == true)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
														}
													}
												}
												this.textBox6.Text = tmpn.ToString();
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
												}
												Thread.Sleep(1000);//延遲1秒
												tmpn = 0;
												tmpco_clr();
												//尋找最終與頂排泡泡相連的所有泡泡
												for (int num = 0; num < 8; num++)
												{
													tmpco[0, num] = true;
													chks_bon(0, num);
												}
												//刪除懸空的泡泡
												for (int row = 0; row < 10; row++)
												{
													for (int num = 0; num < 8 - row % 2; num++)
													{
														if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
														{
															coordi[row, num, 0] = 0;
															Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
															tmpn++;
														}
													}
												}
												this.textBox7.Text = tmpn.ToString();
												points += (tmpn + 1) ^ 3 / 3;//紀錄分數
												if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
												{
													Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
												}

												this.textBox3.Clear();
												this.textBox3.Text = points.ToString();

											}//if tmpn>=2
											painted = 1;
											break;
										}//Search end
									}//左半邊 不反射
								}//左半邊
							}//八個一排
						}//jz ==0
						else
						{
							for (int r = 9; r >= 0; r--)
							{
								if (coordi[r, 3 - r % 2, 0] == 0)
								{
									Paint(previous, coordi[r, 3 - r % 2, 1], coordi[r, 3 - r % 2, 2]);
									coordi[r, 3 - r % 2, 0] = previous;
									painted = 1;
									break;
								}
							}
						}


					}//ff=0 未碰牆
					 //ff=1已碰牆，開始反射
					else
					{
						if (x > 0)                                                  //右牆反射
						{
							tmpy = (375 - y) - i * 30 + 15;
							tmpx = (int)(tmpy * xs / ys);                             //負數

							if (i % 2 == 0)
							{
								line[i] = (int)(7 + (tmpx - 5) / 35);
							}
							else
							{
								line[i] = (int)(6 + (tmpx + 13) / 35);
							}
							//------>呼叫消泡判斷式Search
							if (Search(i, line[i]))
							{
								Thread.Sleep(500);
								tmpn = 0;
								tmpi = i;
								tmpj = line[i];
								tmpco_clr();
								tmpco[i, line[i]] = true;
								chk_bon(i, line[i]);//偵測相連泡泡數目
								if (tmpn >= 2)
								{
									points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																 //相連的的泡泡要消失
									for (int row = 0; row < 10; row++)
									{
										for (int num = 0; num < 8 - row % 2; num++)
										{
											if (tmpco[row, num] == true)
											{
												coordi[row, num, 0] = 0;
												Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
											}
										}
									}
									this.textBox6.Text = tmpn.ToString();
									if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
									{
										Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
									}
									Thread.Sleep(1000);//延遲1秒
									tmpn = 0;
									tmpco_clr();
									//尋找最終與頂排泡泡相連的所有泡泡
									for (int num = 0; num < 8; num++)
									{
										tmpco[0, num] = true;
										chks_bon(0, num);
									}
									//刪除懸空的泡泡
									for (int row = 0; row < 10; row++)
									{
										for (int num = 0; num < 8 - row % 2; num++)
										{
											if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
											{
												coordi[row, num, 0] = 0;
												Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
												tmpn++;
											}
										}
									}
									this.textBox7.Text = tmpn.ToString();
									points += (tmpn + 1) ^ 3 / 3;//紀錄分數
									if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
									{
										Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
									}

									this.textBox3.Clear();
									this.textBox3.Text = points.ToString();


								}
								painted = 1;
								break;
							}
						}
						else                                                        //左牆反射後
						{
							tmpy = (375 - y) - i * 30 + 15;
							tmpx = (int)(tmpy * xs / ys);                             //正數

							if (i % 2 == 0)
							{
								line[i] = (int)(tmpx / 35);
							}
							else
							{
								if (tmpx < 17)
								{
									tmpx = 17;
								}
								line[i] = (int)((tmpx - 17) / 35);
							}
							//------>呼叫消泡判斷式Search
							if (Search(i, line[i]))
							{
								Thread.Sleep(500);
								tmpn = 0;
								tmpi = i;
								tmpj = line[i];
								tmpco_clr();
								tmpco[i, line[i]] = true;
								chk_bon(i, line[i]);//偵測相連泡泡數目
								if (tmpn >= 2)
								{
									points += (tmpn + 1) ^ 2 / 3;//紀錄分數
																 //相連的的泡泡要消失
									for (int row = 0; row < 10; row++)
									{
										for (int num = 0; num < 8 - row % 2; num++)
										{
											if (tmpco[row, num] == true)
											{
												coordi[row, num, 0] = 0;
												Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
											}
										}
									}
									this.textBox6.Text = tmpn.ToString();
									if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
									{
										Send("4" + points + "|" + listBox1.SelectedItem);//-->傳送消除泡泡訊息
									}
									Thread.Sleep(1000);//延遲1秒
									tmpn = 0;
									tmpco_clr();
									//尋找最終與頂排泡泡相連的所有泡泡
									for (int num = 0; num < 8; num++)
									{
										tmpco[0, num] = true;
										chks_bon(0, num);
									}
									//刪除懸空的泡泡
									for (int row = 0; row < 10; row++)
									{
										for (int num = 0; num < 8 - row % 2; num++)
										{
											if (coordi[row, num, 0] != 0 && tmpco[row, num] == false)
											{
												coordi[row, num, 0] = 0;
												Paint(0, coordi[row, num, 1], coordi[row, num, 2]);
												tmpn++;
											}
										}
									}
									this.textBox7.Text = tmpn.ToString();
									points += (tmpn + 1) ^ 3 / 3;//紀錄分數
									if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
									{
										Send("5" + points + "|" + listBox1.SelectedItem);//-->傳送掉落泡泡訊息
									}

									this.textBox3.Clear();
									this.textBox3.Text = points.ToString();

								}
								painted = 1;
								break;
							}
						}
					} //if x>2
				}
				ballx = 120;//發射小泡座標
				bally = 375;
				tmp = colorchoose.Next(1, 6);//用亂數隨機選泡泡顏色
				Paint(tmp, 120, 375);
				previous = tmp;

				/*int pointss = 5;
				if (listBox1.SelectedIndex >= 0)//有選取遊戲對手，上線遊戲中
				{
					Send("5" + pointss + "|" + listBox1.SelectedItem);//-->傳送得分訊息
				}*/
			}
		}

        private void button2_Click(object sender, EventArgs e)
        {
			Control.CheckForIllegalCrossThreadCalls = false;
			User = textBox3.Text;
			string IP = textBox4.Text;
			int Port = int.Parse(textBox5.Text);
			try
			{
				IPEndPoint EP = new IPEndPoint(IPAddress.Parse(IP), Port);
				T = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
				T.Connect(EP);
				Th = new Thread(Listen);
				Th.IsBackground = true;
				Th.Start();
				textBox6.Text = "以連線伺服器!" + "\r\n";
				Send("0" + User);
				button2.Enabled = false;
			}
			catch
			{
				textBox4.Text = "無法連上伺服器!" + "\r\n";
			}
		}

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
		{
			this.textBox1.Text = (e.X).ToString();
			this.textBox2.Text = (e.Y).ToString();
			/*mouseX = e.X + this.pictureBox1.Left;
			mouseY = e.Y + this.pictureBox1.Top;*/
			PointF Mouse = new PointF(e.X, e.Y);
			PointF ShootedBubble = new PointF(120, 375);//發射小泡中心點座標
														//g.DrawLine(pLine, Mouse, ShootedBubble);
		}

		private void theout(object source, System.Timers.ElapsedEventArgs e)
		{
			if (label9.InvokeRequired)
			{
				timeHandler1 TA = new timeHandler1(theout);
				this.Invoke(TA, new object[] { source, e });
			}
			else
			{
				if (timeleft > 1)
				{
					timeleft -= 1;
					label9.Text = timeleft + "seconds";
				}
				else
				{
					label9.Text = "Time's up!";
					//button1.Enabled = true;
					pictureBox1.Enabled = false;
				}
			}
		}
	}
}
	


