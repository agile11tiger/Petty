using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace PettySQLite.Models
{
    public partial class Settings : ObservableObject, IDatabaseItem
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int Lol { get; set; }

        [ObservableProperty] private string _languageType = CultureInfo.CurrentCulture.Name;
        private double _slider3Value;
        public bool? UseFrontCamera
        {
            get => false;
            set => value = false;
        }

        public bool? TryHarder
        {
            get => true;
            set => value = true;
        }

        public bool? TryInverted
        {
            get => false;
            set => value = false;
        }

        public string Slider3Text
        {
            get => "GGG";//LocalizationService.Get(nameof(Slider3Text), Slider3Value);
        }

        public double Slider3Value
        {
            get => _slider3Value;
            set
            {
                if (SetProperty(ref _slider3Value, value))
                    OnPropertyChanged(nameof(Slider3Text));
            }
        }
    }
}
