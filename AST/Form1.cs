using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AST
{
    public partial class Form1 : Form
    {
        internal OpenFileDialog OpenDialog;

        public Form1()
        {
            InitializeComponent();
            OpenDialog = new OpenFileDialog();
            OpenDialog.DefaultExt = "c";
            OpenDialog.Filter = "C- source files (*.cm)|*.cm|All files (*.*)|*.*";
        }

        private void menuFileQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void menuFileLoad_Click(object sender, EventArgs e)
        {
            DialogResult result = OpenDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                SourceReader reader = new SourceReader(OpenDialog.FileName);
                List<Token> source;
                List<string> items = new List<string>();

                try
                {
                    source = reader.lex();
                }
                catch (SyntaxError syntaxError)
                {
                    MessageBox.Show(syntaxError.Message);
                    return;
                }

                foreach (Token token in source)
                {
                    items.Add(token.ToString());
                }
                listBox1.DataSource = items;
            }
        }
    }
}
