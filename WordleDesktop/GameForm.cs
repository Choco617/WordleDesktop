using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace WordleDesktop
{
    public partial class GameForm : Form
    {
        public string sAnswer = "";
        public string sGuess = "";
        public int iTurn = 0;
        public int iLetter = 0;
        public Label[,] LC = new Label[6, 5];
        public List<string> Answers;
        public Label[] KeyboardLC = new Label[26];

        public GameForm()
        {
            InitializeComponent();
        }

        public void GameForm_Load(object sender, EventArgs e)
        {
            //sAnswer = "LEARN";
            //sAnswer = "GLOVE";

            Answers = File.ReadAllLines("valid-wordle-words.txt").ToList();
            RandomizeAns();
            //Console.WriteLine($"picked random answer {sAnswer}");
            int iLetterSize = 40;
            int iGridSize = iLetterSize + 4;
            int iYInset = 20;
            int iXInset = this.Width / 2 - 2 * iGridSize - iLetterSize / 2 - (iGridSize - iLetterSize) / 2;

            string fontName = "Clear Sans";
            Font GuessFont = new Font(fontName, 18);
            if (GuessFont.Name != fontName)
            {
                Console.WriteLine($"could not find font {fontName}");
                fontName = "Helvetica Neue";
                GuessFont = new Font(fontName, 18);
                if (GuessFont.Name != fontName)
                {
                    Console.WriteLine($"could not find font {fontName}");
                    fontName = "Arial";
                    GuessFont = new Font(fontName, 18);
                }
            }
            Font KBfont = new Font(fontName, 12);

            for (int i = 0; i < LC.GetLength(0); i++)
            {
                for (int j = 0; j < LC.GetLength(1); j++)
                {
                    LC[i, j] = new Label();
                    this.Controls.Add(LC[i, j] as Control);
                    LC[i, j].Location = new Point(iXInset + j * iGridSize, iYInset + i * iGridSize);
                    LC[i, j].Visible = true;
                    LC[i, j].ForeColor = Color.White;
                    LC[i, j].BackColor = ColorConst.Indeterminate;
                    LC[i, j].Height = iLetterSize;
                    LC[i, j].Width = iLetterSize;
                    LC[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    LC[i, j].Font = GuessFont;
                }
            }

            string[] QwertyLetters = new string[] { "Q", "W", "E", "R", "T", "Y", "U", "I", "O", "P", "A", "S", "D", "F", "G", "H", "J", "K", "L", "Z", "X", "C", "V", "B", "N", "M" };
            int iKBletterSize = 30;
            int iKBgridSize = iKBletterSize + 4;
            int iKBX = this.Width / 2 - iKBgridSize * 5;
            int iKBY = LC[5, 4].Top + iKBgridSize + 20;
            for (int i = 0; i < KeyboardLC.Length; i++)
            {
                KeyboardLC[i] = new Label();
                this.Controls.Add(KeyboardLC[i] as Control);
                KeyboardLC[i].Location = new Point(iKBX, iKBY);
                KeyboardLC[i].Visible = true;
                KeyboardLC[i].ForeColor = Color.White;
                KeyboardLC[i].BackColor = ColorConst.IndeterminateKB;
                KeyboardLC[i].Height = iKBletterSize;
                KeyboardLC[i].Width = iKBletterSize;
                KeyboardLC[i].TextAlign = ContentAlignment.MiddleCenter;
                KeyboardLC[i].Font = KBfont;
                KeyboardLC[i].Text = QwertyLetters[i];

                // tee up next location
                iKBX += iKBgridSize;
                if (i == 9)
                {
                    iKBX = this.Width / 2 - (int)(iKBgridSize * 4.5);
                    iKBY += iKBgridSize;
                }
                if (i == 18)
                {
                    iKBX = this.Width / 2 - (int)(iKBgridSize * 3.5);
                    iKBY += iKBgridSize;
                }
            }

        }

        private void RandomizeAns()
        {
            Random RNG = new Random();
            sAnswer = Answers[RNG.Next(Answers.Count - 1)].ToUpper();
        }

        public string GetGuessWord(int Which)
        {
            return LC[Which, 0].Text + LC[Which, 1].Text + LC[Which, 2].Text + LC[Which, 3].Text + LC[Which, 4].Text;
        }

        public int EvaluateGuess(int WhichTurn)
        {
            sGuess = GetGuessWord(WhichTurn);

            // first, check for any letters that are correct
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                if (LC[WhichTurn, i].Text.Equals(sAnswer.Substring(i, 1)))
                {
                    LC[WhichTurn, i].BackColor = ColorConst.Right;
                }
            }
            // second, check for any letters that are absent from the answer entirely
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                if (sAnswer.IndexOf(LC[WhichTurn, i].Text) == -1)
                {
                    LC[WhichTurn, i].BackColor = ColorConst.Wrong;
                }
            }
            // third, figure out any misplaced letters
            for (int i = 0; i < LC.GetLength(1); i++)
            {
                if (LC[WhichTurn, i].BackColor == ColorConst.Indeterminate)
                {
                    if (DecideMismatched(i) == 0)
                    {
                        LC[WhichTurn, i].BackColor = ColorConst.Misplaced;
                    }
                    else
                    {
                        LC[WhichTurn, i].BackColor = ColorConst.Wrong;
                    }
                }
            }

            // after dealing with guess itself, update the visual keyboard
            for (int i = 0; i < KeyboardLC.Length; i++)
            {
                KeyboardLC[i].BackColor = GetLetterStatus(KeyboardLC[i].Text);
            }

            // deal with correct answer after keyboard updates
            if (sGuess == sAnswer) { return 0; }

            return -1;
        }

        private int DecideMismatched(int iWhich)
        {
            List<int> MismatchGuess = new List<int>();
            List<int> MismatchAnswer = new List<int>();
            for (int i = 0; i < sGuess.Length; i++)
            {
                // add to MismatchGuess any index that equals the target one but isn't a correct letter against the answer (e.g. checking index #1 in guess GLEAN (L) against answer SLYLY, MismatchGuess == {} (empty))
                if (sGuess.Substring(iWhich, 1).Equals(sGuess.Substring(i, 1)) && !sGuess.Substring(i, 1).Equals(sAnswer.Substring(i, 1)))
                {
                    MismatchGuess.Add(i);
                }
                // add to MismatchAnswer any index that equals the target one but isn't a correct letter against the guess (e.g. checking index #1 in answer SLYLY (L), MismatchAnswer == {3})
                if (sGuess.Substring(iWhich, 1).Equals(sAnswer.Substring(i, 1)) && !sAnswer.Substring(i, 1).Equals(sGuess.Substring(i, 1)))
                {
                    MismatchAnswer.Add(i);
                }
            }

            int MaxMismatches = MismatchAnswer.Count;
            if (MismatchGuess.Count < MismatchAnswer.Count) { MaxMismatches = MismatchGuess.Count; }

            int iRet = -1;
            for (int i = 0; i < MaxMismatches; i++)
            {
                if (MismatchGuess[i] == iWhich) { iRet = 0; }
            }

            return iRet;
        }

        public int CheckValidWord(string Word)
        {
            for (int i = 0; i < Answers.Count; i++)
            {
                if (Word.Equals(Answers[i],StringComparison.OrdinalIgnoreCase)) { return 0; }
            }
            return -1;
        }

        private Color GetLetterStatus(string sLetter)
        {
            Color cRet = ColorConst.IndeterminateKB;

            for (int i = 0; i < LC.GetLength(0); i++)
            {
                for (int j = 0; j < LC.GetLength(1); j++)
                {
                    if (LC[i, j].Text == sLetter)
                    {
                        if (LC[i, j].BackColor == ColorConst.Right)
                        {
                            return ColorConst.Right;
                        }
                        else if (LC[i, j].BackColor == ColorConst.Wrong)
                        {
                            return ColorConst.Wrong;
                        }
                        else if (LC[i, j].BackColor == ColorConst.Misplaced)
                        {
                            cRet = ColorConst.Misplaced;
                        }
                    }
                }
            }
            return cRet;
        }

        public void GiveUp()
        {
            MessageBox.Show($"So close! The answer was {sAnswer}. Thanks for playing.");
            Reset();
        }

        public static class ColorConst
        {
            public static Color Right = ColorTranslator.FromHtml("#538D4E");
            public static Color Wrong = ColorTranslator.FromHtml("#3A3A3C");
            public static Color Misplaced = ColorTranslator.FromHtml("#B59F3B");
            public static Color Indeterminate = ColorTranslator.FromHtml("#121213");
            public static Color IndeterminateKB = ColorTranslator.FromHtml("#818384");
        }

        private void GiveUpButton_Click(object sender, EventArgs e)
        {
            GiveUp();
            LC[0,0].Focus();
        }
    }
}
