using System;
using System.Collections.Generic;
using iText.Kernel.Geom;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Extgstate;
using iText.Layout;
using iText.Layout.Properties;
using iText.Layout.Element;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Kernel.Font;
//attempt changing fonts using IO fonts
using iText.IO.Font;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigurationSearchUtility
{
    class MyPDFmaker
    {
        public static readonly string IMG = @"C:\Users\LENOVO\Desktop\Test\pdfTests\newLogo.png";

        public void addWatermark(String dest, string src)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            Document doc = new Document(pdfDoc);

            ImageData img = ImageDataFactory.Create(IMG);

            float w = img.GetWidth();
            float h = img.GetHeight();

            PdfExtGState gs1 = new PdfExtGState().SetFillOpacity(0.3f);

            for (int i = 1; i < pdfDoc.GetNumberOfPages(); i++)
            {
                PdfPage pdfpage = pdfDoc.GetPage(i);
                Rectangle pageSize = pdfpage.GetPageSizeWithRotation();

                pdfpage.SetIgnorePageRotationForContent(true);

                float x = (pageSize.GetLeft() + pageSize.GetRight()) / 2;
                float y = (pageSize.GetTop() + pageSize.GetBottom()) / 2;
                PdfCanvas over = new PdfCanvas(pdfDoc.GetPage(i));
                over.SaveState();
                over.SetExtGState(gs1);
                if (i != 0)
                {
                    over.AddImageWithTransformationMatrix(img, w, 0, 0, h, x - (w / 2), y - (h / 2), false);
                }
                over.RestoreState();
            }
            doc.Close();
        }

        public void AddPageNumbers(string dest, string src)
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(src), new PdfWriter(dest));
            Document doc = new Document(pdfDoc);

            int numberOfPages = pdfDoc.GetNumberOfPages();

            for (int i = 1; i <= numberOfPages; i++)
            {
                doc.ShowTextAligned(new Paragraph("page " + i + " of " + numberOfPages),
                    800, 50, i, iText.Layout.Properties.TextAlignment.RIGHT, iText.Layout.Properties.VerticalAlignment.TOP, 0);
            }
            doc.Close();
        }
    }
}
