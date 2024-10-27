using System;
using System.Collections.Generic;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class ExerciciosToNotaPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        bool isbusy = false;
        public ExerciciosToNotaPage(List<Exercicio> exercicios)
        {
            InitializeComponent();
            List.ItemsSource = exercicios;
        }

        async void Handle_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                try
                {
                    Exercicio exercicio = (sender as ListView).SelectedItem as Exercicio;
                    var resposta = await client.GetStringAsync(server.Servidor + "/api/Nota/byusuarioexercicio/" + UsuarioAtual.ID + "/" + exercicio.ID);
                    List<Nota> notas = JsonConvert.DeserializeObject<List<Nota>>(resposta);
                    await Navigation.PushAsync(new NotasPage(notas));
                }
                catch
                {
                    await DisplayAlert("ERRO", "Erro de conexão", "OK");
                }
                isbusy = false;
            }
        }
    }
}
