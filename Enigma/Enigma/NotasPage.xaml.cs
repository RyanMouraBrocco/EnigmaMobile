using System;
using System.Collections.Generic;
using EnigmaClass;
using Xamarin.Forms;

namespace Enigma
{
    public partial class NotasPage : ContentPage
    {
        public NotasPage(List<Nota> notas)
        {
            InitializeComponent();
            List.ItemsSource = notas;
        }

        void Handle_ItemTapped(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            List.SelectedItem = null;
        }
    }
}
