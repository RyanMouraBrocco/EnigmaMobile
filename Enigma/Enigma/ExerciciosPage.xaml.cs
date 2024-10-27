using System;
using System.Collections.Generic;
using System.Net.Http;
using EnigmaClass;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace Enigma
{
    public partial class ExerciciosPage : ContentPage
    {
        Server server = new Server();
        HttpClient client = new HttpClient();
        List<Exercicio> exercicios = new List<Exercicio>();
        int idconteudo;
        bool isbusy = false;
        public  ExerciciosPage(int idconteudo)
        {
            InitializeComponent();
            this.idconteudo = idconteudo;
        }
         async void CarregarGridAsync()
        {
            Lista.ItemsSource = null;
            try
            {
                var resposta = await client.GetStringAsync(server.Servidor + "/api/Exercicio/all/" + idconteudo);
                exercicios = JsonConvert.DeserializeObject<List<Exercicio>>(resposta);
                Lista.ItemsSource = exercicios;
            }
            catch 
            {
                await DisplayAlert("ERRO", "Falta de Conexão", "OK");
            }
        }
        protected override void OnAppearing()
        {
            frm_carregando.IsVisible = true;
            base.OnAppearing();
            CarregarGridAsync();
            frm_carregando.IsVisible = false;
        }

        async void Handle_ItemTappedAsync(object sender, Xamarin.Forms.ItemTappedEventArgs e)
        {
            if (isbusy==false)
            {
                isbusy = true;
                try
                {
                    frm_carregando.IsVisible = true;
                    Exercicio exercicio = new Exercicio();
                    exercicio = (sender as ListView).SelectedItem as Exercicio;
                    var resposta = await client.GetStringAsync(server.Servidor + "/api/Exercicio/byid/" + exercicio.ID);
                    exercicio = JsonConvert.DeserializeObject<Exercicio>(resposta);
                    frm_carregando.IsVisible = false;
                    await Navigation.PushAsync(new RealizarExercicioPage(exercicio));
                    isbusy = false;
                }
                catch
                {
                    frm_carregando.IsVisible = false;
                    isbusy = false;
                    await DisplayAlert("ERRO", "Falta de Conexão", "OK");
                }
            }
        }
    }
}
