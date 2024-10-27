using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using EnigmaClass;
using System.Linq;

namespace Enigma
{
    public partial class MinhasPerguntasERespostasPage : ContentPage
    {
        Server server = new Server();
        HttpClient Client = new HttpClient();
        public MinhasPerguntasERespostasPage()
        {
            InitializeComponent();
            TapGestureRecognizer tapsair = new TapGestureRecognizer();
            tapsair.Tapped += (object sender, EventArgs e) => 
            {
                Navigation.PopModalAsync();
            };
            btnsair.GestureRecognizers.Add(tapsair);
        }

        async void carregargridAsync()
        {
            try
            {
                Lv_Perguntas.ItemsSource = null;
                Lv_Respostas.ItemsSource = null;
                var resposta = await Client.GetStringAsync(server.Servidor + "/api/Pergunta/byusuario/"+UsuarioAtual.ID);
                List<Pergunta> perguntas = JsonConvert.DeserializeObject<List<Pergunta>>(resposta);
                Lv_Perguntas.ItemsSource = perguntas;
                resposta = await Client.GetStringAsync(server.Servidor + "/api/Resposta/byusuario/" + UsuarioAtual.ID);
                List<Resposta> respostas = JsonConvert.DeserializeObject<List<Resposta>>(resposta);
                Lv_Respostas.ItemsSource = respostas;
            }
            catch 
            {
                await DisplayAlert("ERRO", "Erro de conexão", "OK");
            }

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            carregargridAsync();
        }

        void Pergunta_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        { 
            Pergunta pergunta = (sender as ListView).SelectedItem as Pergunta;
            Navigation.PushModalAsync(new NavigationPage(new InfoPerguntaRespostaPage(null,pergunta)){ BarTextColor = Color.White,BarBackgroundColor = Color.FromHex("#000449"),});
        }

        void Resposta_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            Resposta Resposta = new Resposta();
            Resposta = (sender as ListView).SelectedItem as Resposta;
            Navigation.PushModalAsync(new NavigationPage(new InfoPerguntaRespostaPage(Resposta, null)) { BarTextColor = Color.White, BarBackgroundColor = Color.FromHex("#000449"), });
        }
    }
}
