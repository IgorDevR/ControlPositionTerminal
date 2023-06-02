using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlPositionTerminal.Common;
using ControlPositionTerminal.Common.Model;
using ControlPositionTerminal.Common.UserSettings;
using ControlPositionTerminal.Properties;
using ControlPositionTerminal.Util;
using ControlPositionTerminal.Util.Enums;
using Io.Gate.GateApi.Model;

namespace ControlPositionTerminal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel viewModel = new ViewModel();

        public MainWindow()
        {

            InitializeComponent();
            LoadWindowPositions();
            SetTypeExchangesTo_cBox_SelectServer();
            AppSettings.GetInstance().LoadObjectFieldsFromFile();
            handleChangeSettings();
            SetTextFieldFromLoadSettings();
            AddColumnInTables();

            SymbolAndLeverageDataGrid.SelectionChanged += SymbolAndLeverageDataGrid_SelectionChanged;
            this.DataContext = viewModel;

        }

        #region work with open position tab

        /// <summary>
        /// Получить список позиций и установить их в таблицу для отображения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async Task RetrieveAndPaintOpenPositionAsync()
        {
            LogList.AddLog("Выполняется обновление всех позиций. Ожидайте...");
            await Task.Delay(2000);
            OpenPositionDataGrid.ItemsSource = null;
            IExchangeServiceWrapper exchangeServiceWrapper = AppSettings.Instance.GetExchangeService();
            List<PositionData> openPositions = await exchangeServiceWrapper.RetrieveOpenPositionsAsync();
            openPositions = PositionUtils.CheckAndAddEmptyPosition(openPositions);

            ObservableCollection<PositionData> positionDatas = new ObservableCollection<PositionData>(openPositions);
            OpenPositionDataGrid.ItemsSource = positionDatas;
        }
        /// <summary>
        /// Получить список позиций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RetrieveOpenPosition_Click(object sender, RoutedEventArgs e)
        {
            RetrieveAndPaintOpenPositionAsync();
        }

        private async void CloseAll_Click(object sender, RoutedEventArgs e)
        {
            await Close(sender, "0");
        }

        private async void ClosePart_Click(object sender, RoutedEventArgs e)
        {
            await Close(sender, SizePartClosePositionTextBox.Text);
        }

        private async Task Close(object sender, string size)
        {
            LogList.AddLog("Выполняется зыкрытие позиции. Ожидайте...");
            var button = (Button)sender;
            var positionData = (PositionData)button.DataContext;
            IExchangeServiceWrapper exchangeServiceWrapper = AppSettings.GetInstance().GetExchangeService();
            await exchangeServiceWrapper.CloseSelectPositionAsync(positionData, size);
            await RetrieveAndPaintOpenPositionAsync();
        }

        #endregion

        #region work with leverage tab
        /// <summary>
        ///Обработчик выбора строки в таблицы плечей SelectLeverageSymbolTextBlock
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SymbolAndLeverageDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid == null)
                return;

            var selectedRow = dataGrid.SelectedItem as InitialLeverage;
            if (selectedRow != null)
            {
                var symbol = selectedRow.Symbol;
                var leverage = selectedRow.Leverage_limit;
                SelectLeverageSymbolTextBlock.Text = symbol + "-" + leverage;
            }
        }
        /// <summary>
        /// Получить все символы и плечи при нажатии на кнопку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetAllSymbolCoinsAndLeverage_Click(object sender, RoutedEventArgs e)
        {
            await RetrieveSymbolAndLeverageAllCoinsAsync();
        }
        /// <summary>
        /// Получить список всех монет и их настройки плечей
        /// </summary>
        private async Task RetrieveSymbolAndLeverageAllCoinsAsync()
        {
            LogList.AddLog("Выполняется обновление всех позиций и плечей. Ожидайте...");
            await Task.Delay(2000);
            SymbolAndLeverageDataGrid.ItemsSource = null;
            IExchangeServiceWrapper exchangeServiceWrapper = AppSettings.GetInstance().GetExchangeService();
            List<InitialLeverage> symbolAndLeverage = (List<InitialLeverage>)await exchangeServiceWrapper.RetrieveAllLeverageCoinsAsync();
            var initialLeveragesSorted = new ObservableCollection<InitialLeverage>(symbolAndLeverage.OrderBy(item => item.Leverage_limit));
            SymbolAndLeverageDataGrid.ItemsSource = initialLeveragesSorted;
        }

        /// <summary>
        /// Установить значение плеча у выбраного символа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetNewValueLeverageSelectSymbol_Click(object sender, RoutedEventArgs e)
        {
            string[] selectSymbolAndLeverageArr = SelectLeverageSymbolTextBlock.Text.Split('-');
            List<InitialLeverage> selectSymbolAndLeverages = new List<InitialLeverage>();

            if (selectSymbolAndLeverageArr.Length == 2)
            {
                selectSymbolAndLeverages.Add(new InitialLeverage(selectSymbolAndLeverageArr[0],
                    int.Parse(selectSymbolAndLeverageArr[1])));
                await SetNewValueAsync(selectSymbolAndLeverages);
                return;
            }
            LogList.AddLog($"Ошибка при выбое символа.");

        }
        /// <summary>
        /// Установить значение плеча у всех символов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetNewValueLeverageAllSymbol_Click(object sender, RoutedEventArgs e)
        {
            ObservableCollection<InitialLeverage>? initialLeverages = SymbolAndLeverageDataGrid.ItemsSource as ObservableCollection<InitialLeverage>;
            List<InitialLeverage> symbolAndLeverage = new List<InitialLeverage>(initialLeverages);
            await SetNewValueAsync(symbolAndLeverage);
        }
        /// <summary>
        /// Установить значекние плеча
        /// </summary>
        /// <param name="selectSymbol"></param>
        private async Task SetNewValueAsync(List<InitialLeverage> selectSymbol)
        {
            LogList.AddLog("Выполняется изменение плечей. Ожидайте...");
            IExchangeServiceWrapper exchangeServiceWrapper = AppSettings.GetInstance().GetExchangeService();
            await exchangeServiceWrapper.SetNewValueLeverage(selectSymbol, NewLeverageValueTextBlock.Text);
            SymbolAndLeverageDataGrid.SelectedItem = null;
            SelectLeverageSymbolTextBlock.Text = "";
            await RetrieveSymbolAndLeverageAllCoinsAsync();
        }
        #endregion

        #region setting change
        /// <summary>
        /// Обработчик изменения настроек и других параметров
        /// </summary>
        private void handleChangeSettings()
        {
            HandleComboBox();
        }

        /// <summary>
        /// Обработка изменений в текстовых комбо боксах
        /// </summary>
        private void HandleComboBox()
        {

            SelectedServerType.SelectionChanged += (sender, args) =>
            {
                //if ((TypesServerEnum)SelectedServerType.SelectedItem != TypesServerEnum.None)
                {

                    var selected = (TypesServerEnum)SelectedServerType.SelectedItem;
                    AppSettings.Instance.SelectedServerType = selected;
                }
            };

            SelectedMarketType.SelectionChanged += (sender, args) =>
            {
                //if ((TypesMarketEnum)SelectedMarketType.SelectedItem != TypesMarketEnum.None)
                {
                    var selected = (TypesMarketEnum)SelectedMarketType.SelectedItem;
                    AppSettings.Instance.SelectedMarketType = selected;
                }
            };

            SelectedCoinType.SelectionChanged += (sender, args) =>
            {
                //if ((TypesMarketCoinEnum)SelectedCoinType.SelectedItem != TypesMarketCoinEnum.None)
                {
                    var selected = (TypesMarketCoinEnum)SelectedCoinType.SelectedItem;
                    AppSettings.Instance.SelectedCoinType = selected;
                }
                ;
            };
        }

        private void SaveOrUpdateSettings_Click(object sender, RoutedEventArgs e)
        {
            SetConfigKeysValueField();
            AppSettings.GetInstance().SaveObjectFieldsToFile();
        }

        /// <summary>
        /// Установить значение полей AppSettings, при нажатии кнопки сохранить
        /// </summary>
        private void SetConfigKeysValueField()
        {
            var thisFields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var configFields = typeof(AppSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var thisField in thisFields)
            {

                foreach (var configField in configFields)
                {

                    if (configField.Name.Contains(thisField.Name))
                    {
                        try
                        {
                            if (thisField.FieldType == typeof(TextBox))
                            {
                                var textFieldValue = ((TextBox)thisField.GetValue(this)).Text;
                                configField.SetValue(AppSettings.Instance, textFieldValue);

                            }
                            else if (thisField.Name == "SelectedServerType")
                            {
                                dynamic comboBox = thisField.GetValue(this);
                                var selectedItem = comboBox.SelectedItem;
                                configField.SetValue(AppSettings.Instance, selectedItem);
                            }
                            else if (thisField.Name == "SelectedMarketType")
                            {
                                dynamic comboBox = thisField.GetValue(this);
                                var selectedItem = comboBox.SelectedItem;
                                configField.SetValue(AppSettings.Instance, selectedItem);
                            }
                            else if (thisField.Name == "SelectedCoinType")
                            {
                                dynamic comboBox = thisField.GetValue(this);
                                var selectedItem = comboBox.SelectedItem;
                                configField.SetValue(AppSettings.Instance, selectedItem);
                            }
                        }
                        catch (Exception e)
                        {

                        }
                    }
                }
            }
        }
        #endregion

        #region work with column
        private void AddColumnInTables()
        {
            AddAndSetColumnInTableOpenPositionDataGrid();
            AddAndSetColumnInTableSymbolAndLeverageDataGrid();
        }
        private void AddAndSetColumnInTableOpenPositionDataGrid()
        {
            // Удаляем все существующие колонки
            OpenPositionDataGrid.Columns.Clear();

            // Создаем колонки для каждого свойства в PositionData
            foreach (PropertyInfo property in typeof(PositionData).GetProperties())
            {
                // Пропускаем свойства, которые не должны отображаться в DataGrid
                if (property.Name == "HiddenProperty")
                    continue;

                var column = new DataGridTextColumn
                {
                    Header = property.Name,
                    Binding = new Binding(property.Name)
                };
                OpenPositionDataGrid.Columns.Add(column);
            }

            // Добавляем колонку с кнопкой "Закрыть"
            var closeColumn = new DataGridTemplateColumn
            {
                Header = "Закрыть",
                CellTemplate = new DataTemplate()
            };
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.SetValue(Button.ContentProperty, "Закрыть");
            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(CloseAll_Click));
            closeColumn.CellTemplate.VisualTree = buttonFactory;
            OpenPositionDataGrid.Columns.Add(closeColumn);

            // Добавляем колонку с кнопкой "Закрыть частично"
            var closePartColumn = new DataGridTemplateColumn
            {
                Header = "Закрыть частично",
                CellTemplate = new DataTemplate()
            };
            FrameworkElementFactory buttonPartFactory = new FrameworkElementFactory(typeof(Button));
            buttonPartFactory.SetValue(Button.ContentProperty, "Закрыть");
            buttonPartFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(ClosePart_Click));
            closePartColumn.CellTemplate.VisualTree = buttonPartFactory;
            OpenPositionDataGrid.Columns.Add(closePartColumn);


        }
        private void AddAndSetColumnInTableSymbolAndLeverageDataGrid()
        {
            // Удаляем все существующие колонки
            SymbolAndLeverageDataGrid.Columns.Clear();

            // Создаем колонки для каждого свойства в PositionData
            foreach (PropertyInfo property in typeof(InitialLeverage).GetProperties())
            {
                // Пропускаем свойства, которые не должны отображаться в DataGrid
                if (property.Name == "HiddenProperty")
                    continue;

                var column = new DataGridTextColumn
                {
                    Header = property.Name.Replace("_", " "),
                    Binding = new Binding(property.Name),
                    Width = 100,
                    IsReadOnly = true,
                };
                SymbolAndLeverageDataGrid.Columns.Add(column);
            }
            SymbolAndLeverageDataGrid.Columns[1].SortDirection = ListSortDirection.Ascending;
        }
        #endregion

        #region load 
        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.WindowLocation = new System.Drawing.Point((int)this.Left, (int)this.Top);
            Properties.Settings.Default.WindowSize = new System.Drawing.Size((int)this.Width, (int)this.Height);
            Properties.Settings.Default.Save();
        }
        private void LoadWindowPositions()
        {
            this.Left = Properties.Settings.Default.WindowLocation.X;
            this.Top = Properties.Settings.Default.WindowLocation.Y;
            this.Height = Properties.Settings.Default.WindowSize.Height;
            this.Width = Properties.Settings.Default.WindowSize.Width;
        }


        /// <summary>
        /// Установить значения в начальные поля после загрузки из файла при старте
        /// </summary>
        private void SetTextFieldFromLoadSettings()
        {
            var thisFields = this.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            var configFields = typeof(AppSettings).GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

            foreach (var thisField in thisFields)
            {
                foreach (var configField in configFields)
                {
                    if (configField.Name.Contains(thisField.Name))
                    {
                        try
                        {
                            if (thisField.FieldType == typeof(TextBox))
                            {
                                var textFieldValue = (string)configField.GetValue(AppSettings.GetInstance());
                                var textField = (TextBox)thisField.GetValue(this);
                                textField.Text = textFieldValue;
                            }
                            else if (thisField.Name == "SelectedServerType")
                            {
                                SelectedServerType.SelectedItem = AppSettings.GetInstance().SelectedServerType;
                                viewModel.SelectedServerType = AppSettings.GetInstance().SelectedServerType;
                            }
                            else if (thisField.Name == "SelectedMarketType")
                            {
                                SelectedMarketType.SelectedItem = AppSettings.GetInstance().SelectedMarketType;
                                viewModel.SelectedMarketType = AppSettings.GetInstance().SelectedMarketType;
                            }
                            else if (thisField.Name == "SelectedCoinType")
                            {
                                SelectedCoinType.SelectedItem = AppSettings.GetInstance().SelectedCoinType;
                                viewModel.SelectedCoinType = AppSettings.GetInstance().SelectedCoinType;
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Установить доступные типы в комбо бокс при старте
        /// </summary>
        private void SetTypeExchangesTo_cBox_SelectServer()
        {
            SelectedServerType.ItemsSource = Enum.GetValues(typeof(TypesServerEnum));
            SelectedMarketType.ItemsSource = Enum.GetValues(typeof(TypesMarketEnum));
            SelectedCoinType.ItemsSource = Enum.GetValues(typeof(TypesMarketCoinEnum));
        }



        #endregion
    }
}
