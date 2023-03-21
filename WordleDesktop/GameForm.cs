using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WordleDesktop
{
    public partial class GameForm : Form
    {
        public int iLetterSize = 40;
        public int iGridSize = 44;
        public int iXInset = 20;
        public int iYInset = 20;
        public string sAnswer = "";
        public string sGuess = "";
        public int iTurn = 0;
        public int iLetter = 0;
        public Label[,] LC = new Label[6, 5];

        public GameForm()
        {
            InitializeComponent();
        }

        public void GameForm_Load(object sender, EventArgs e)
        {
            //sAnswer = "LEARN";
            sAnswer = "GLOVE";

            string fontName = "Clear Sans";
            Font testFont = new Font(fontName, 18);
            if (testFont.Name != fontName)
            {
                Console.WriteLine($"could not find font {fontName}");
                fontName = "Helvetica Neue";
                testFont = new Font(fontName, 18);
                if (testFont.Name != fontName)
                {
                    Console.WriteLine($"could not find font {fontName}");
                    fontName = "Arial";
                    testFont = new Font(fontName, 18);
                }
            }

            for (int i = 0; i < LC.GetLength(0); i++)
            {
                for (int j = 0; j < LC.GetLength(1); j++)
                {
                    LC[i, j] = new Label();
                    this.Controls.Add(LC[i, j] as Control);
                    LC[i, j].Location = new Point(iXInset + j * iGridSize, iYInset + i * iGridSize);
                    LC[i, j].Visible = true;
                    LC[i, j].ForeColor = Color.White;
                    LC[i, j].BackColor = ColorTranslator.FromHtml(ColorConst.Indeterminate);
                    LC[i, j].Height = iLetterSize;
                    LC[i, j].Width = iLetterSize;
                    LC[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    LC[i, j].Font = testFont;
                }
            }
            
        }

        public string GetGuessWord(int Which)
        {
            return LC[Which, 0].Text + LC[Which, 1].Text + LC[Which, 2].Text + LC[Which, 3].Text + LC[Which, 4].Text;
        }

        public int EvaluateGuess(int WhichTurn)
        {
            //Console.WriteLine($"current guess to evaluate is {LC[WhichTurn, 0].Text}{LC[WhichTurn, 1].Text}{LC[WhichTurn, 2].Text}{LC[WhichTurn, 3].Text}{LC[WhichTurn, 4].Text}");
            //sGuess = LC[WhichTurn, 0].Text + LC[WhichTurn, 1].Text + LC[WhichTurn, 2].Text + LC[WhichTurn, 3].Text + LC[WhichTurn, 4].Text;
            sGuess = GetGuessWord(WhichTurn);
            //Console.WriteLine($"current guess to evaluate is {sGuess}");

            // first, check for any letters that are correct
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                //Console.WriteLine($"evaluating character {i}: guess={LC[WhichTurn, i].Text}, answer={sAnswer.Substring(i, 1)}, match? {LC[WhichTurn, i].Text.Equals(sAnswer.Substring(i, 1))}");
                if (LC[WhichTurn, i].Text.Equals(sAnswer.Substring(i, 1)))
                {
                    LC[WhichTurn, i].BackColor = ColorTranslator.FromHtml(ColorConst.Right);
                }
            }
            // after highlighting, if this is completely correct, just stop
            if (sGuess == sAnswer) { return 0; }
            // second, check for any letters that are absent from the answer entirely
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                //Console.WriteLine($"evaluating character {i}: guess={LC[WhichTurn, i].Text}, (full answer={sAnswer}), wrong? {sAnswer.IndexOf(LC[WhichTurn, i].Text) == -1}");
                if (sAnswer.IndexOf(LC[WhichTurn, i].Text) == -1)
                {
                    LC[WhichTurn, i].BackColor = ColorTranslator.FromHtml(ColorConst.Wrong);
                }
            }
            // third, figure out any misplaced letters
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                if (LC[WhichTurn, i].BackColor == ColorTranslator.FromHtml(ColorConst.Indeterminate))
                {
                    if (DecideMismatched(i) == 0)
                    {
                        LC[WhichTurn, i].BackColor = ColorTranslator.FromHtml(ColorConst.Misplaced);
                    }
                    else
                    {
                        LC[WhichTurn, i].BackColor = ColorTranslator.FromHtml(ColorConst.Wrong);
                    }
                }
            }

            return 1;
        }

        private int DecideMismatched(int iWhich)
        {
            List<int> MismatchGuess = new List<int>();
            List<int> MismatchAnswer = new List<int>();
            for (int i = 0; i < sGuess.Length; i++)
            {
                // add to MismatchGuess any index that equals the target one but isn't a correct letter against the answer
                if (sGuess.Substring(iWhich, 1).Equals(sGuess.Substring(i, 1)) && !sGuess.Substring(i, 1).Equals(sAnswer.Substring(i, 1)))
                {
                    MismatchGuess.Add(i);
                }
                // add to MismatchAnswer any index that equals the target one but isn't a correct letter against the guess
                if (sGuess.Substring(iWhich, 1).Equals(sAnswer.Substring(i, 1)) && !sAnswer.Substring(i, 1).Equals(sGuess.Substring(i, 1)))
                {
                    MismatchAnswer.Add(i);
                }
            }
            string OutputAns = "";
            if (MismatchAnswer.Count > 0)
            {
                for (int i = 0; i < MismatchAnswer.Count; i++) { OutputAns += (MismatchAnswer[i] + " "); }
            }
            string OutputGus = "";
            if (MismatchGuess.Count > 0)
            {
                for (int i = 0; i < MismatchGuess.Count; i++) { OutputGus += (MismatchGuess[i] + " "); }
            }
            Console.WriteLine($"MismatchAnswer for #{iWhich} ({sGuess.Substring(iWhich, 1)}): {OutputAns}");
            Console.WriteLine($"MismatchGuess  for #{iWhich} ({sGuess.Substring(iWhich, 1)}): {OutputGus}");

            int MaxMismatches = MismatchAnswer.Count;
            if (MismatchGuess.Count < MismatchAnswer.Count) { MaxMismatches = MismatchGuess.Count; }

            int iRet = -1;
            for (int i = 0; i < MaxMismatches; i++)
            {
                if (MismatchGuess[i] == iWhich) { iRet = 0; }
            }

            Console.WriteLine($"DecideMismatched returning {iRet}");

            return iRet;

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            /*
            base.OnPaint(e);

            Graphics g = e.Graphics;
            using (Pen selPen = new Pen(ColorTranslator.FromHtml(ColorConst.Indeterminate)))
            {
                
                for (int i = 0; i < LC.GetLength(0); i++)
                {
                    for (int j = 0; j < LC.GetLength(1); j++)
                    {
                        //g.DrawRectangle(selPen, iInset + j * iGridSize, iInset + i * iGridSize, iLetterSize, iLetterSize);
                        //SolidBrush selBrush = new SolidBrush(ColorTranslator.FromHtml(ColorConst.Right));
                        //Console.WriteLine($"LC[{i}, {j}].LetterColor = {LC[i, j].LetterColor}");
                        //Console.WriteLine($"i = {i}, j = {j}");
                        
                        //SolidBrush selBrush = new SolidBrush(ColorTranslator.FromHtml(LC[i, j].LetterColor));
                        //g.FillRectangle(selBrush, new Rectangle(iXInset + j * iGridSize, iYInset + i * iGridSize, iLetterSize, iLetterSize));
                        
                        LC[i, j].Letter.BringToFront();
                        LC[i, j].Letter.Text = "X";
                        //Point somePoint = LC[i, j].Letter.Location;
                        //Console.WriteLine($"should have just moved label [{i}, {j}] - its location is {somePoint.X}, {somePoint.Y}");
                    }
                }
                Console.WriteLine($"data for label [0, 0]: \n" +
                                  $"left = {LC[0, 0].Letter.Left} \n" +
                                  $"top  = {LC[0, 0].Letter.Top} \n" +
                                  $"visible = {LC[0, 0].Letter.Visible} \n" +
                                  $"forecolor = {LC[0, 0].Letter.ForeColor} \n");
            }
            */
        }

        public static class ColorConst
        {
            public const string Right = "#538D4E";
            public const string Wrong = "#3A3A3C";
            public const string Misplaced = "#B59F3B";
            public const string Indeterminate = "#121213";
        }
        
        public class LetterCell
        {
            /*
            private Rectangle InternalRectangle;
            private TextBox InternalLetter;

            public Rectangle Rect
            {
                get { return InternalRectangle; }
                set { InternalRectangle = value; }
            }
            public TextBox Letter
            {
                get { return InternalLetter; }
                set { InternalLetter = value; }
            }
            */

            private string IntlLetterColor = ColorConst.Indeterminate;
            private Label IntlLetter = new Label();

            public string LetterColor { get { return IntlLetterColor; } set {  IntlLetterColor = value; } }
            public Label Letter { get { return IntlLetter; } set { IntlLetter = value; } }

        }
    }
}
