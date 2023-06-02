using System.ComponentModel;
using ControlPositionTerminal.Util.Enums;

namespace ControlPositionTerminal;

public class ViewModel : INotifyPropertyChanged
{
    private TypesMarketCoinEnum _selectedCoinType;

    public TypesMarketCoinEnum SelectedCoinType
    {
        get { return _selectedCoinType; }
        set
        {
            _selectedCoinType = value;
            OnPropertyChanged("SelectedCoinType");
        }
    }

    private TypesMarketEnum _selectedMarketType;

    public TypesMarketEnum SelectedMarketType
    {
        get { return _selectedMarketType; }
        set
        {
            _selectedMarketType = value;
            OnPropertyChanged("SelectedMarketType");
        }
    }
    private TypesServerEnum _selectedServerType;

    public TypesServerEnum SelectedServerType
    {
        get { return _selectedServerType; }
        set
        {
            _selectedServerType = value;
            OnPropertyChanged("SelectedServerType");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}