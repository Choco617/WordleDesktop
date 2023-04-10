using System;
using System.Drawing;
using System.Windows.Forms;

namespace WordleDesktop
{
    partial class GameForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.GiveUpButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // GiveUpButton
            // 
            this.GiveUpButton.Location = new System.Drawing.Point(12, 12);
            this.GiveUpButton.Name = "GiveUpButton";
            this.GiveUpButton.Size = new System.Drawing.Size(57, 23);
            this.GiveUpButton.TabIndex = 1;
            this.GiveUpButton.TabStop = false;
            this.GiveUpButton.Text = "Give Up";
            this.GiveUpButton.UseVisualStyleBackColor = true;
            this.GiveUpButton.Click += new System.EventHandler(this.GiveUpButton_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 409);
            this.Controls.Add(this.GiveUpButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WordleDesktop";
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GameForm_KeyDown);
            this.ResumeLayout(false);

        }

        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
            {
                //MessageBox.Show(e.KeyCode.ToString());

                
                // update the corresponding LetterCell to show this letter
                if (iLetter < LC.GetLength(1))
                {
                    LC[iTurn, iLetter].Text = e.KeyCode.ToString();
                    //Console.WriteLine($"should have just set LC[{iTurn}, {iLetter}] to {e.KeyCode.ToString()} - LC[iTurn, iLetter].Letter.Text = {LC[iTurn, iLetter].Text}");
                    iLetter++;

                    // if this completes a word, check that it's valid, Quordle-style
                    if (iLetter == LC.GetLength(1))
                    {
                        if (CheckValidWord(GetGuessWord(iTurn)) == -1) { RedWord(); }
                    }
                }
            }
            else if(e.KeyCode == Keys.Enter)
            {
                // validation
                // too short
                if (LC[iTurn, 4].Text == "")
                {
                    MessageBox.Show("Too short!");
                    return;
                }
                // already guessed
                for (int i = 0; i < iTurn; i++)
                {
                    if (GetGuessWord(i) == GetGuessWord(iTurn))
                    {
                        MessageBox.Show("Already guessed that!");
                        ClearGuess();
                        return;
                    }
                }
                // not in the dictionary
                if (CheckValidWord(GetGuessWord(iTurn)) == -1)
                {
                    MessageBox.Show("Not in word list");
                    return;
                }

                // evaluate the guess
                int iResult = EvaluateGuess(iTurn);
                if (iResult == 0)
                {
                    MessageBox.Show($"Congratulations! You got it in {iTurn + 1}!");
                    Reset();
                }
                else
                {
                    iTurn++;
                    iLetter = 0;
                    if (iTurn > 5)
                    {
                        GiveUp();
                    }
                }
            }
            else if(e.KeyCode == Keys.Back)
            {
                UnRedWord();
                if (iLetter > 0)
                {
                    // clear the LetterCell and decrement the counter
                    iLetter--;
                    LC[iTurn, iLetter].Text = "";
                }
                return;
            }
            else if(e.KeyCode == Keys.Escape)
            {
                if (iTurn == 0 && iLetter == 0) { this.Close(); }
                ClearGuess();
                return;
            }
        }

        private void ClearGuess()
        {
            // clear the entire guess and reset the counter
            while (iLetter >= LC.GetLength(1)) { iLetter--; }
            for (; iLetter >= 0; iLetter--) { LC[iTurn, iLetter].Text = ""; }
            iLetter = 0;
            UnRedWord();
        }

        private void Reset()
        {
            UnRedWord();
            iLetter = 0;
            iTurn = 0;
            for (int i = 0; i < LC.GetLength(0); i++)
            {
                for (int j = 0; j < LC.GetLength(1); j++)
                {
                    LC[i, j].Text = "";
                    LC[i, j].BackColor = ColorConst.Indeterminate;
                }
            }
            for (int i = 0; i < KeyboardLC.GetLength(0); i++)
            {
                KeyboardLC[i].BackColor = ColorConst.IndeterminateKB;
            }
            RandomizeAns();
        }

        public void RedWord()
        {
            for (int j = 0; j < LC.GetLength(1); j++)
            {
                LC[iTurn, j].ForeColor = Color.Red;
            }
        }

        public void UnRedWord()
        {
            for (int j = 0; j < LC.GetLength(1); j++)
            {
                LC[iTurn, j].ForeColor = Color.White;
            }
        }


        #endregion

        private Button GiveUpButton;
    }
}

