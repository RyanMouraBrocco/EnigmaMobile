using System;
using System.Collections.Generic;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class ConteudosListPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        bool isbusy = false;
        public ConteudosListPage(List<Conteudo> conteudos)
        {
            InitializeComponent();
            List.ItemsSource = conteudos;
        }

        async void Handle_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                try
                {
                    Conteudo conteudo = (sender as ListView).SelectedItem as Conteudo;
                    var resposta = await client.GetStringAsync(server.Servidor + "/api/Exercicio/all/" + conteudo.ID);
                    List<Exercicio> exercicios = JsonConvert.DeserializeObject<List<Exercicio>>(resposta);
                    await Navigation.PushAsync(new ExerciciosToNotaPage(exercicios));
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
