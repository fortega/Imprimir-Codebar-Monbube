using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Imprimir_Codebar_Monbube
{
    public class Code39
    {
        private float wLine = 2, heigth = 100;
        private Dictionary<char, short[]> d;
        private short[] parsedText;
        public Code39(string text)
        {
            #region Dictionary
            d = new Dictionary<char, short[]>();
            d.Add('0', new short[] { 0, 0, 0, 1, 1, 0, 1, 0, 0 });
            d.Add('1', new short[] { 1, 0, 0, 1, 0, 0, 0, 0, 1 });
            d.Add('2', new short[] { 0, 0, 1, 1, 0, 0, 0, 0, 1 });
            d.Add('3', new short[] { 1, 0, 1, 1, 0, 0, 0, 0, 0 });
            d.Add('4', new short[] { 0, 0, 0, 1, 1, 0, 0, 0, 1 });
            d.Add('5', new short[] { 1, 0, 0, 1, 1, 0, 0, 0, 0 });
            d.Add('6', new short[] { 0, 0, 1, 1, 1, 0, 0, 0, 0 });
            d.Add('7', new short[] { 0, 0, 0, 1, 0, 0, 1, 0, 1 });
            d.Add('8', new short[] { 1, 0, 0, 1, 0, 0, 1, 0, 0 });
            d.Add('9', new short[] { 0, 0, 1, 1, 0, 0, 1, 0, 0 });
            d.Add('A', new short[] { 1, 0, 0, 0, 0, 1, 0, 0, 1 });
            d.Add('B', new short[] { 0, 0, 1, 0, 0, 1, 0, 0, 1 });
            d.Add('C', new short[] { 1, 0, 1, 0, 0, 1, 0, 0, 0 });
            d.Add('D', new short[] { 0, 0, 0, 0, 1, 1, 0, 0, 1 });
            d.Add('E', new short[] { 1, 0, 0, 0, 1, 1, 0, 0, 0 });
            d.Add('F', new short[] { 0, 0, 1, 0, 1, 1, 0, 0, 0 });
            d.Add('G', new short[] { 0, 0, 0, 0, 0, 1, 1, 0, 1 });
            d.Add('H', new short[] { 1, 0, 0, 0, 0, 1, 1, 0, 0 });
            d.Add('I', new short[] { 0, 0, 1, 0, 0, 1, 1, 0, 0 });
            d.Add('J', new short[] { 0, 0, 0, 0, 1, 1, 1, 0, 0 });
            d.Add('K', new short[] { 1, 0, 0, 0, 0, 0, 0, 1, 1 });
            d.Add('L', new short[] { 0, 0, 1, 0, 0, 0, 0, 1, 1 });
            d.Add('M', new short[] { 1, 0, 1, 0, 0, 0, 0, 1, 0 });
            d.Add('N', new short[] { 0, 0, 0, 0, 1, 0, 0, 1, 1 });
            d.Add('O', new short[] { 1, 0, 0, 0, 1, 0, 0, 1, 0 });
            d.Add('P', new short[] { 0, 0, 1, 0, 1, 0, 0, 1, 0 });
            d.Add('Q', new short[] { 0, 0, 0, 0, 0, 0, 1, 1, 1 });
            d.Add('R', new short[] { 1, 0, 0, 0, 0, 0, 1, 1, 0 });
            d.Add('S', new short[] { 0, 0, 1, 0, 0, 0, 1, 1, 0 });
            d.Add('T', new short[] { 0, 0, 0, 0, 1, 0, 1, 1, 0 });
            d.Add('U', new short[] { 1, 1, 0, 0, 0, 0, 0, 0, 1 });
            d.Add('V', new short[] { 0, 1, 1, 0, 0, 0, 0, 0, 1 });
            d.Add('W', new short[] { 1, 1, 1, 0, 0, 0, 0, 0, 0 });
            d.Add('X', new short[] { 0, 1, 0, 0, 1, 0, 0, 0, 1 });
            d.Add('Y', new short[] { 1, 1, 0, 0, 1, 0, 0, 0, 0 });
            d.Add('Z', new short[] { 0, 1, 1, 0, 1, 0, 0, 0, 0 });
            d.Add('-', new short[] { 0, 1, 0, 0, 0, 0, 1, 0, 1 });
            d.Add('.', new short[] { 1, 1, 0, 0, 0, 0, 1, 0, 0 });
            d.Add(' ', new short[] { 0, 1, 1, 0, 0, 0, 1, 0, 0 });
            d.Add('*', new short[] { 0, 1, 0, 0, 1, 0, 1, 0, 0 });
            d.Add('$', new short[] { 0, 1, 0, 1, 0, 1, 0, 0, 0 });
            d.Add('/', new short[] { 0, 1, 0, 1, 0, 0, 0, 1, 0 });
            d.Add('+', new short[] { 0, 1, 0, 0, 0, 1, 0, 1, 0 });
            d.Add('%', new short[] { 0, 0, 0, 1, 0, 1, 0, 1, 0 });
            #endregion

            this.parsedText = parse(
                string.Concat('*', text, '*'));
        }

        public Code39(string text, float heigth)
            : this(text)
        {
            this.heigth = heigth;
        }

        public Code39(string text, float width, float heigth)
            : this(text, heigth)
        {
            this.Width = width;
        }

        public void Draw(Graphics g, float x, float y)
        {
            float left = x;
            bool white;
            float size;

            for (int i = 0; i < parsedText.Length; i++)
            {
                white = ((i % 2) == 1);
                size = (parsedText[i] == 0) ? wLine : wLine * 2;

                if (!white)
                    g.FillRectangle(Brushes.Black, left, y, size, heigth);

                left += size;
            }
        }

        public int Lines
        {
            get
            {
                int r = 0;
                foreach (short i in parsedText)
                {
                    if (i == 0)
                        r += 1;
                    else
                        r += 2;
                }

                return r;
            }
        }

        public float Width
        {
            get
            {
                return Lines * wLine;
            }

            set
            {
                wLine = value / Lines;
            }
        }

        public float LineWidth
        {
            get { return wLine; }
            set { wLine = value; }
        }

        public float Heigth
        {
            get { return heigth; }
        }

        private short[] parse(string text)
        {
            short[] r = new short[(text.Length * 10) - 1];
            short[] t;
            for (int i = 0; i < text.Length; i++)
            {
                t = getChar(text[i]);

                for (int j = 0; j < 9; j++)
                    r[i * 10 + j] = t[j];

                if (i + 1 < text.Length)
                    r[(i + 1) * 10] = 0;
            }

            return r;
        }

        private short[] getChar(char c)
        {
            return d[c];
        }
    }
}
