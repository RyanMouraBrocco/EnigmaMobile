using System;
using System.Collections.Generic;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class DesempenhoViewPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        int materia = 0;
        bool isbusy = false;

        public DesempenhoViewPage(Desempenho desempenho)
        {
            InitializeComponent();
            this.materia = desempenho.Materia.ID;
            lblmateria.Text = desempenho.Materia.Nome;
            lbldesempenho.Text = (desempenho.Porcentagem * 100).ToString() + "%";
            string horas = string.Empty;
            string minutos = string.Empty;
            horas = ((int)(desempenho.HorasEstudadas)).ToString();
            minutos = ((int)((desempenho.HorasEstudadas - Convert.ToInt32(horas)) * 60)).ToString();
            lblhoras.Text = horas + " horas e "+ minutos +" minutos";

        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                Navigation.PopModalAsync();
            }
        }

        async void Notas_ClickedAsync (object sender, System.EventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                try
                {
                    List<Conteudo> Conteudos = new List<Conteudo>();
                    var resposta = await client.GetStringAsync(server.Servidor + "/api/Conteudo/bymateria/" + materia);
                    Conteudos = JsonConvert.DeserializeObject<List<Conteudo>>(resposta);
                    await Navigation.PushAsync(new ConteudosListPage(Conteudos));
                }
                catch
                {
                    await DisplayAlert("Erro", "Erro de conexão", "OK");
                }
                isbusy = false;
            }
        }
    }
}
