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
            this.HardModeBox = new System.Windows.Forms.CheckBox();
            this.HardModeLabel = new System.Windows.Forms.Label();
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
            // HardModeBox
            // 
            this.HardModeBox.AutoSize = true;
            this.HardModeBox.Checked = true;
            this.HardModeBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.HardModeBox.Location = new System.Drawing.Point(33, 78);
            this.HardModeBox.Name = "HardModeBox";
            this.HardModeBox.Size = new System.Drawing.Size(15, 14);
            this.HardModeBox.TabIndex = 2;
            this.HardModeBox.UseVisualStyleBackColor = true;
            this.HardModeBox.CheckedChanged += new System.EventHandler(this.HardModeBox_CheckedChanged);
            // 
            // HardModeLabel
            // 
            this.HardModeLabel.AutoSize = true;
            this.HardModeLabel.Location = new System.Drawing.Point(13, 59);
            this.HardModeLabel.Name = "HardModeLabel";
            this.HardModeLabel.Size = new System.Drawing.Size(60, 13);
            this.HardModeLabel.TabIndex = 3;
            this.HardModeLabel.Text = "Hard Mode";
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(373, 409);
            this.Controls.Add(this.HardModeLabel);
            this.Controls.Add(this.HardModeBox);
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
            this.PerformLayout();

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

                // if hard mode, check if a rule if being broken and abort
                if (bHard == true)
                {
                    // first check alphabet for ruled-out letters
                    for ( int i = 0; i < LC.GetLength(1); i++)
                    {
                        if (GetLetterStatus(LC[iTurn, i].Text) == ColorConst.Wrong)
                        {
                            MessageBox.Show($"{LC[iTurn, i].Text} is known to be absent from the answer!");
                            return;
                        }
                    }
                    // then check if a known misplacement is tried again
                    for (int j = 0; j < LC.GetLength(1); j++)
                    {
                        for (int i = 0; i < iTurn; i++)
                        {
                            if (LC[i, j].Text == LC[iTurn, j].Text && LC[i, j].BackColor == ColorConst.Misplaced)
                            {
                                MessageBox.Show($"{LC[i, j].Text} can't go in position {j + 1}!");
                                return;
                            }
                        }
                    }
                    // not only literal .Misplaced, but also known bad repeats, like 2nd A in AWARD for answer WATCH
                    for (int j = 0; j < LC.GetLength(1); j++)
                    {
                        for (int i = 0; i < iTurn; i++)
                        {
                            if (LC[i, j].Text == LC[iTurn, j].Text && LC[i, j].BackColor == ColorConst.Wrong)
                            {
                                MessageBox.Show($"{LC[i, j].Text} can't go in position {j + 1} - this is probably a bad repeat!");
                                return;
                            }
                        }
                    }
                    // then check if a known position is not reused
                    for (int j = 0; j < LC.GetLength(1); j++)
                    {
                        for (int i = 0;i < iTurn; i++)
                        {
                            if (LC[i, j].Text != LC[iTurn, j].Text && LC[i, j].BackColor == ColorConst.Right)
                            {
                                MessageBox.Show($"Failed to use {LC[i, j].Text} in position {j + 1}!");
                                return;
                            }
                        }
                    }

                    // TBD: add check that known repeat is not being used, e.g. if AWARD says the second A is not in, putting an A there is disallowed
                    // maybe literally check LC for it by color?
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
        private CheckBox HardModeBox;
        private Label HardModeLabel;
    }
}

