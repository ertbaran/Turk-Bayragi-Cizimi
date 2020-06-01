using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace TurkBayragi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics formmGraphics;  // Grafik Miras Nesnesi
        float ekstra;   // Kenar Boşlukları İçin Ekstra Yer
        float ekleX = 0;    // Bayrağı X ekseninde ortaya çekmek için
        float ekleY = 0;    // Bayrağı Y ekseninde ortaya çekmek için
        float g;

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Form1_Resize(sender, e);
        }
        private void GradyanArkaplan()  // Renk Geçişli Arkaplan
        {
            LinearGradientBrush linGrBrush = new LinearGradientBrush(
            new Point(ClientRectangle.Width / 2, 0),      // Gradyan başlangıç noktası
            new Point(ClientRectangle.Width / 2, ClientRectangle.Height),    // Gradyan bitiş noktası
            Color.FromArgb(255, 255, 255, 255),   // Birincil renk
            Color.FromArgb(255, 255, 0, 0));  // İkincil renk

            Pen pen = new Pen(linGrBrush);

            formmGraphics.FillRectangle(linGrBrush, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height));
        }

        private PointF[] YildizNokralari(float noktaMiktar, RectangleF rect)   // Yıldızın Noktaları Method
        {
            // Noktalar Dizisi
            PointF[] pts = new PointF[Convert.ToInt32(noktaMiktar)];

            double rx = rect.Width / 2;
            double ry = rect.Height / 2;
            double mx = rect.X + rx;
            double my = rect.Y + ry;


            double teta = -Math.PI / 1;     // Yıldızın Eğim Açısı
            double dteta = 4 * Math.PI / noktaMiktar;
            for (int i = 0; i < noktaMiktar; i++)
            {
                pts[i] = new PointF(
                    (float)(mx + rx * Math.Cos(teta)),
                    (float)(my + ry * Math.Sin(teta)));
                teta += dteta;
            }

            return pts;
        }

        private void DrawBezier(PointF ilk, PointF control1, PointF control2, PointF son)   // Kıvrım İçin Method
        {
            // Bezier Kalemi
            Pen siyahKalem = new Pen(Color.Black, 1);

            // Kıvrımı Çiz
            formmGraphics.DrawBezier(siyahKalem, ilk, control1, control2, son);
        }

        private void Form1_DoubleClick(object sender, EventArgs e) // Çift Tıklayınca Jpg Olarak Kaydet
        {
            FormBorderStyle = FormBorderStyle.None; // Tam boy resim alabilmek için kenarlıkları kapat. (anlık)
            Bitmap bmp = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);    // Resmin ebatları
            Graphics memoryGraphics = Graphics.FromImage(bmp);  // bmp ebatında resimlik hafızaya al
            memoryGraphics.CopyFromScreen(Location.X, Location.Y,   // Form'un ekrandaki konumunu hafızadaki yere kopyala
                ClientRectangle.Location.X, ClientRectangle.Location.Y, Size);
            bmp.Save("bayrak.jpg", ImageFormat.Jpeg);   // isim ve format belirle
            FormBorderStyle = FormBorderStyle.Sizable; // Kenarlıkları geri aç.
        }

        private void Form1_Resize(object sender, EventArgs e)   // Boyut Değişiminde Tekrar Hesapla, Çiz
        {
            if (ClientRectangle.Width <= ClientRectangle.Height)    // Y ekseninde ortalamak için
            {
                ekleY = Math.Abs(ClientRectangle.Height - ClientRectangle.Width) / 2;
                g = ClientRectangle.Width / 2;
            }
            else        // X ekseninde ortalamak için
            {
                ekleX = Math.Abs(ClientRectangle.Height - ClientRectangle.Width) / 2;
                g = ClientRectangle.Height / 2;
            }

            ekstra = g / 4;

            formmGraphics = CreateGraphics();   // Grafik Kontrol Atama

            // ÇİZİM ARAÇLARI
            Brush formmBrush = new SolidBrush(Color.Red);   // Fırça Rengi
            Pen formmPen = new Pen(Color.Black); // Kalem Rengi

            // GRADYAN Arkaplan Method
            GradyanArkaplan();

            // BAYRAK Kenar Çizgileri
            //Bayrak Sol Çizgi Noktaları
            PointF bayrakSolUst = new PointF(ekstra + ekleX, ekstra + ekleY);
            PointF bayrakSolAlt = new PointF(ekstra + ekleX, g + ekstra + ekleY);
            //Bayrak Sağ Çizgi Noktaları
            PointF bayrakSagUst = new PointF(g * 3 / 2 + ekstra + ekleX, ekstra + ekleY);
            PointF bayrakSagAlt = new PointF(g * 3 / 2 + ekstra + ekleX, g + ekstra + ekleY);

            //Bayrak Sol Çizgi
            formmGraphics.DrawLine(formmPen, bayrakSolUst, bayrakSolAlt);
            formmGraphics.DrawLine(formmPen, bayrakSolUst, bayrakSolAlt);
            //Bayrak Sağ Çizgi
            formmGraphics.DrawLine(formmPen, bayrakSagUst, bayrakSagAlt);
            formmGraphics.DrawLine(formmPen, bayrakSagUst, bayrakSagAlt);

            // Kıvrım için noktalar.
            //Bayrak Üst Kıvrım(Bezier)
            PointF ilkNoktaUst = new PointF(ekstra + ekleX, ekstra + ekleY);
            PointF kontrolBirUst = new PointF(g * 3 / 8 + ekstra + ekleX, -g / 4 + ekstra + ekleY);
            PointF kontroIkiUst = new PointF(g * 9 / 8 + ekstra + ekleX, g / 4 + ekstra + ekleY);
            PointF sonNoktaUst = new PointF(g * 3 / 2 + ekstra + ekleX, ekstra + ekleY);
            DrawBezier(ilkNoktaUst, kontrolBirUst, kontroIkiUst, sonNoktaUst);

            //Bayrak Alt Kıvrım(Bezier)
            PointF ilkNoktaAlt = new PointF(ekstra + ekleX, g + ekstra + ekleY);
            PointF kontrolBirAlt = new PointF(g * 3 / 8 + ekstra + ekleX, g * 3 / 4 + ekstra + ekleY);
            PointF kontroIkiAlt = new PointF(g * 9 / 8 + ekstra + ekleX, g * 5 / 4 + ekstra + ekleY);
            PointF sonNoktaAlt = new PointF(g * 3 / 2 + ekstra + ekleX, g + ekstra + ekleY);
            DrawBezier(ilkNoktaAlt, kontrolBirAlt, kontroIkiAlt, sonNoktaAlt);

            // YILDIZ                               
            RectangleF yildizKonum = new RectangleF(g * 167 / 240 + ekstra + ekleX, g * 3 / 8 + ekstra + ekleY, g / 4, g / 4); // Yıldızın Dikdörtgensel Ölçüsü Konumu
            PointF[] pts = YildizNokralari(5, yildizKonum); // Yıldız konum noktaları
            formmGraphics.DrawPolygon(Pens.Black, pts); // Yıldızı çiz

            // HİLAL
            formmGraphics.DrawArc(formmPen, new RectangleF(g / 4 + ekstra + ekleX, g / 4 + ekstra + ekleY, g / 2, g / 2), 30, 300);   // Hilal Dış Çember
            formmGraphics.DrawArc(formmPen, new RectangleF(g * 29 / 80 + ekstra + ekleX, g * 3 / 10 + ekstra + ekleY, g * 2 / 5, g * 2 / 5), 45, 270);   // Hilal İç Çember
        }
    }
} 
