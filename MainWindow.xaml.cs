using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace StockManagment
{
    public partial class MainWindow : Window
    {
        StockDal _stockDal = new StockDal();

        public MainWindow()
        {
            InitializeComponent();
            _stockDal.CheckAndCreateDatabaseAndTable();
            LoadStock();
        }

        public void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            dgStock.ItemsSource = _stockDal.GetAll();
        }

        public void LoadStock()
        {
            dgStock.ItemsSource = _stockDal.GetAll();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            _stockDal.Add(new Stock
            {
                ProductName = tbxProductName.Text,
                StockQuantity = Convert.ToInt32(tbxStockQuantity.Text),
                Categorie = tbxCategorie.Text
            });
            LoadStock();
            MessageBox.Show("Product Added!");
        }

        private void btnsAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dgStock.SelectedCells.Count > 0)
            {
                DataGridCellInfo cellInfo = dgStock.SelectedCells[0];

                if (cellInfo.Item is Stock selectedStock)
                {
                    Stock stock = new Stock
                    {
                        Id = selectedStock.Id,
                        ProductName = tbxsProductName.Text,
                        StockQuantity = Convert.ToInt32(tbxsStockQuantity.Text),
                        Categorie = tbxsCategorie.Text
                    };

                    _stockDal.Update(stock);
                    LoadStock();
                    MessageBox.Show("Updated!");
                }
            }
        }

        private void dgStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStock.SelectedItem is Stock selectedStock)
            {
                tbxsProductName.Text = selectedStock.ProductName;
                tbxsStockQuantity.Text = selectedStock.StockQuantity.ToString();
                tbxsCategorie.Text = selectedStock.Categorie;
            }
        }

        private void btndAdd_Click(object sender, RoutedEventArgs e)
        {
            if (dgStock.SelectedItem is Stock selectedStock)
            {
                int id = selectedStock.Id;
                _stockDal.Delete(id);
                LoadStock();
                MessageBox.Show("Deleted!");
            }
        }

        private void GeneratePdf(List<Stock> stocks)
        {
            try
            {
                // SaveFileDialog'u başlat
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PDF Dosyaları|*.pdf";
                saveFileDialog.Title = "PDF Dosyasını Kaydet";
                saveFileDialog.FileName = "DataGridToPdf.pdf"; // Varsayılan dosya adı

                if (saveFileDialog.ShowDialog() == true)
                {
                    // PDF dosyasını oluştur
                    using (var stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        // PDF Writer'ı başlat
                        using (var document = new Document())
                        {
                            PdfWriter.GetInstance(document, stream);
                            document.Open();

                            // DataGrid sütun başlıklarını PDF tablosuna ekle
                            PdfPTable table = new PdfPTable(dgStock.Columns.Count);
                            foreach (var column in dgStock.Columns)
                            {
                                table.AddCell(new Phrase(column.Header.ToString()));
                            }
                            table.CompleteRow();

                            // DataGrid verilerini PDF tablosuna ekle
                            foreach (var item in stocks)
                            {
                                table.AddCell(item.Id.ToString());
                                table.AddCell(item.ProductName);
                                table.AddCell(item.StockQuantity.ToString());
                                table.AddCell(item.Categorie);
                                table.CompleteRow();
                            }

                            // PDF tablosunu PDF dokümanına ekle
                            document.Add(table);
                        }
                    }

                    MessageBox.Show($"PDF dosyası başarıyla kaydedildi: {saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF oluşturma hatası: " + ex.Message);
            }
        }

        private void btnExportToPdf_Click(object sender, RoutedEventArgs e)
        {
            List<Stock> stocks = _stockDal.GetAll();
            GeneratePdf(stocks);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = searchBox.Text.ToLower();

            List<Stock> filteredStocks = _stockDal.GetAll().Where(stock =>
                stock.ProductName.ToLower().Contains(searchText) ||
                stock.Categorie.ToLower().Contains(searchText)
            ).ToList();

            dgStock.ItemsSource = filteredStocks;
        }

       
    }
}
