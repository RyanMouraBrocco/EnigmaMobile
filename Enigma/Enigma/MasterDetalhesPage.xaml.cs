using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Enigma
{
    public partial class MasterDetalhesPage : MasterDetailPage
    {
        public MasterDetalhesPage()
        {
            InitializeComponent();
            Master = new ConfigPage();
            Detail = new NavigationPage(new AbasPage()){ BarBackgroundColor = Color.FromHex("#000449"), Title="menu", BarTextColor=Color.White};

        }
    }
}
