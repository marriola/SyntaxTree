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
            OpenDialog.Filter = "C source files (*.c)|*.c|C header files (*.h)|*.h|All files (*.*)|*.*";
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
                List<Token> source = reader.lex();
                List<string> items = new List<string>();
                foreach (Token token in source)
                {
                    if (token.type == TokenType.ID)
                    {
                        items.Add("ID " + ((IdentifierToken)token).stringValue);
                    }
                    else if (token.type == TokenType.INTEGER)
                    {
                        items.Add("INTEGER " + ((IntegerToken)token).intValue);
                    }
                    else if (token.type == TokenType.COMMENT)
                    {
                        items.Add("COMMENT " + ((CommentToken)token).stringValue);
                    }
                    else
                    {
                        items.Add(token.type.ToString());
                    }
                }
                listBox1.DataSource = items;
            }
        }
    }
}
