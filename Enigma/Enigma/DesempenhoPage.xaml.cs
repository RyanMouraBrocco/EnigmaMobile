using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class DesempenhoPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        bool isbusy = false;
        public DesempenhoPage()
        {
            InitializeComponent();
            TapGestureRecognizer tap = new TapGestureRecognizer();
            tap.Tapped+= (object sender, EventArgs e) => 
            {
                Navigation.PopModalAsync();
            };
            btnsair.GestureRecognizers.Add(tap);
            CarregarListAsync();
        }
        public async void CarregarListAsync()
        {
            try
            {
                Lv_Materias.ItemsSource = null;
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Materia/all");
                List<Materia> materias = JsonConvert.DeserializeObject<List<Materia>>(resposta);
                Lv_Materias.ItemsSource = materias;
            }
            catch 
            {
                await DisplayAlert("Erro", "Erro de Conexão", "OK");
            }
            Lv_Materias.IsRefreshing = false;
        }

        void Handle_Refreshing(object sender, System.EventArgs e)
        {
            CarregarListAsync();
        }

        async void Handle_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                try
                {
                    Materia materia = (sender as ListView).SelectedItem as Materia;
                    Desempenho desempenho = new Desempenho
                    {
                        Materia = new Materia { ID = materia.ID },
                        Usuario = new Usuario { ID = UsuarioAtual.ID },
                        HorasEstudadas = 0,
                        Porcentagem = 0
                    };
                    var valor = JsonConvert.SerializeObject(desempenho);
                    var conteudo = new StringContent(valor, Encoding.UTF8, "application/json");
                    HttpResponseMessage resp = null;
                    resp = await client.PostAsync(server.Servidor + "/api/Desempenho/atualizar", conteudo);
                    if (!resp.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Erro", "Algo deu ERRADO!", "OK");
                    }
                    else
                    {
                        var resposta = await client.GetStringAsync(server.Servidor + "/api/Desempenho/bymateriaandusuario/" + materia.ID + "/" + UsuarioAtual.ID);
                        desempenho = JsonConvert.DeserializeObject<Desempenho>(resposta);
                        desempenho.Materia = new Materia { ID = materia.ID, Nome = materia.Nome };
                        await Navigation.PushModalAsync(new NavigationPage(new DesempenhoViewPage(desempenho)) { BarTextColor = Color.White, BarBackgroundColor = Color.FromHex("#000449") });
                    }
                }
                catch
                {
                    await DisplayAlert("Erro", "Erro de Conexão", "OK");
                }
                isbusy = false;
            }
        }
    }
}
