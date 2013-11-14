using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;
using System.Data.OleDb;

namespace Imprimir_Codebar_Monbube
{
    public partial class Main : Form
    {
        List<Impresion> impresiones = new List<Impresion>();
        string[] allCodes = new string[0];
        string[] allDesc = new string[0];
        int numItems;
        int curItem = 0;

        Font fCode = new Font(
            Properties.Settings.Default.fCodebarFamily,
            Properties.Settings.Default.fCodebarSize);

        Font fDesc = new Font(Properties.Settings.Default.fDescFamily, Properties.Settings.Default.fDescSize);
        public Main()
        {
            InitializeComponent();
        }


        private void importarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                using (OleDbConnection c =
                    new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + openFile.FileName))
                {
                    c.Open();
                    OleDbCommand cmd = new OleDbCommand("SELECT Code,Cantidad,Desc,NombreProducto,ValorNeto,Variante from SKU", c);
                    OleDbDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Impresion imp = new Impresion((string)reader["Code"],(int)reader["Cantidad"],(string)reader["Desc"],(string)reader["NombreProducto"],(int)reader["ValorNeto"],(string)reader["Variante"]);

                        impresiones.Add(imp);
                    };
                    c.Close();
                }

            }
            gv.DataSource = impresiones.ToArray();

            gv.AutoResizeColumns();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            gv.DataSource = impresiones;
        }


        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float minWidth = Properties.Settings.Default.minusWidth;
            float gapWidth = Properties.Settings.Default.gapWidth;

            float w = (e.PageSettings.PrintableArea.Size.Width - minWidth - 2 * gapWidth) / 3;
            float h = e.PageSettings.PrintableArea.Size.Height - fCode.Height - minWidth;
            float[] x = new float[] {
                e.PageSettings.PrintableArea.X,
                e.PageSettings.PrintableArea.X + w + gapWidth,
                (e.PageSettings.PrintableArea.X + w + gapWidth) * 2
            };
            float y = e.PageSettings.PrintableArea.Y;

            for (int i = 0; i < 3; i++)
            {
                if (curItem < numItems)
                {
                    if (!string.IsNullOrEmpty(allDesc[curItem]))
                    {
                        SizeF descSize = e.Graphics.MeasureString(allDesc[curItem], fDesc);

                        e.Graphics.DrawString(allDesc[curItem], fDesc, Brushes.Black, x[i] + (w - descSize.Width) / 2, y);

                        Code39 codebar = new Code39(allCodes[curItem], w, h - descSize.Height);
                        codebar.Draw(e.Graphics, x[i], y + descSize.Height);

                        float tWidth = e.Graphics.MeasureString(allCodes[curItem], fCode).Width;
                        e.Graphics.DrawString(allCodes[curItem], fCode, Brushes.Black, x[i] + (w - tWidth) / 2, y + h);
                    }
                    else
                    {
                        Code39 codebar = new Code39(allCodes[curItem], w, h);
                        codebar.Draw(e.Graphics, x[i], y);

                        float tWidth = e.Graphics.MeasureString(allCodes[curItem], fCode).Width;
                        e.Graphics.DrawString(allCodes[curItem], fCode, Brushes.Black, x[i] + (w - tWidth) / 2, y + h);
                    }

                    curItem++;

                }
            }

            e.HasMorePages = curItem < numItems;
        }

        private void imprimirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (printDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                numItems = impresiones.Sum(i => i.Num);
                allCodes = new string[numItems];
                allDesc = new string[numItems];

                int curI = 0;
                foreach (Impresion imp in impresiones)
                {
                    for (int j = 0; j < imp.Num; j++)
                    {
                        allCodes[curI] = imp.Codebar;
                        allDesc[curI++] = imp.SKU;
                    }
                }
                curItem = 0;
                printDocument1.Print();
            }
        }

        private void exportarBSaleProductos(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                sw.WriteLine("Nombre Producto,Controla Stock,Valor Neto,Permite Decimal,Variante,Código Barras,SKU");
                foreach (Impresion impresion in impresiones)
                {
                    /* Nombre Producto  Controla Stock	Valor Neto	Permite Decimal	Variante	Código Barras	SKU */
                    sw.WriteLine(
                        string.Join(Properties.Settings.Default.ExportBsaleSep,
                        new string[] { impresion.NombreProducto, "1", impresion.ValorNeto.ToString(), "0", impresion.Variante, impresion.Codebar, impresion.SKU })
                        );
                }
                sw.Flush();
                sw.Close();
                
            }
        }

        private void exportarBsaleStock(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(saveFileDialog1.FileName);
                sw.WriteLine("Sucursal,SKU,Stock,Costo Neto");
                foreach (Impresion impresion in impresiones)
                {
                    sw.WriteLine(
                        string.Join(Properties.Settings.Default.ExportBsaleSep,
                        new string[] { Properties.Settings.Default.BsaleSucursal, impresion.SKU, impresion.Num.ToString(), impresion.ValorNeto.ToString() })
                        );
                }
                sw.Flush();
                sw.Close();

            }
        }
    }
}
